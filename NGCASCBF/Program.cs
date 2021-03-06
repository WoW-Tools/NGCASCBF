﻿using BruteforceLib;
using BruteforceLib.Hashing;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Loader;
using System.Threading;
using System.Threading.Tasks;

namespace NGCASCBF
{
    class Program
    {
        static ConcurrentDictionary<ulong, string> knownNames = new ConcurrentDictionary<ulong, string>();
        static ConcurrentDictionary<ulong, bool> hashes = new ConcurrentDictionary<ulong, bool>();
        static ConcurrentDictionary<ulong, bool> processed = new ConcurrentDictionary<ulong, bool>();

        static List<FileNameGenerator> fileNameGenerators = new List<FileNameGenerator>();

        static ManualResetEvent reset = new ManualResetEvent(true);

        static DateTime startTime;
        static long hashCount;
        static int currentSpeed;
        static bool stop = true;

        // Folder with listfiles
        static string ListFilesFolder = @"f:\Dev\WoW\listfiles\";
        // DB2 path
        static string DB2Folder = @"f:\Dev\WoW\DBFilesClient_24500\";
        // Root file path
        static string RootFilePath = @"f:\Dev\WoW\tools\CASCExplorer\CASCExplorer\bin\Debug\root";
        // Merged listfile name
        const string finalListFile = "finallist.txt";

        static FileNameGenerator currentGenerator;

        static void Main(string[] args)
        {
            Console.WriteLine($"NumLogicalProcessors: {Environment.ProcessorCount}");

            LoadFileNameGenerators();

            // plain text hashes source
            if (File.Exists("hashes.txt"))
            {
                using (StreamReader sr = File.OpenText("hashes.txt"))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        ulong hash = ulong.Parse(line, System.Globalization.NumberStyles.HexNumber);
                        hashes[hash] = true;
                    }
                }
            }

            if (!File.Exists(RootFilePath))
                RootFilePath = "root";

            // read root file
            using (var _ = new PerfCounter())
            {
                using (var fs = File.OpenRead(RootFilePath))
                using (var br = new BinaryReader(fs))
                {
                    while (fs.Position < fs.Length)
                    {
                        uint count = br.ReadUInt32();

                        fs.Position += 8 + count * 4; // skip Locale and Content flags, filedataid's

                        for (var i = 0; i < count; ++i)
                        {
                            fs.Position += 16; // skip MD5
                            ulong hash = br.ReadUInt64();
                            hashes[hash] = true;
                        }
                    }
                }
            }

            Console.WriteLine("Loaded {0} known name hashes!", hashes.Count);

            Thread t = new Thread(StateHandler)
            {
                IsBackground = true
            };
            t.Start();

            if (!Directory.Exists(ListFilesFolder))
                ListFilesFolder = ".\\listfiles\\";

            Console.WriteLine("Data path: {0}", ListFilesFolder);

            BruteforceConfig.ListFilesFolder = ListFilesFolder;

            if (!Directory.Exists(DB2Folder))
                DB2Folder = ".\\DBFilesClient\\";

            Console.WriteLine("DB2 path: {0}", DB2Folder);

            BruteforceConfig.DB2Folder = DB2Folder;

            string finalListFilePath = Path.Combine(ListFilesFolder, finalListFile);

            if (File.Exists(finalListFilePath))
            {
                File.Delete(finalListFilePath);
            }

            Stopwatch swatch = new Stopwatch();

            startTime = DateTime.Now;

            foreach (var generator in fileNameGenerators)
            {
                Console.WriteLine($"Running {generator.Name}...");

                try
                {
                    currentGenerator = generator;

                    swatch.Start();
                    Parallel.ForEach(generator.GetFileNames(), new ParallelOptions() { MaxDegreeOfParallelism = Environment.ProcessorCount }, LocalInit, HandleLine, LocalFinal);
                    swatch.Stop();

                    Console.WriteLine();
                }
                catch (Exception exc)
                {
                    Console.WriteLine($"Generator exception: {exc.Message}");
                }
            }

            Console.WriteLine(swatch.Elapsed);

            SaveKnownNames(finalListFilePath);

            Console.WriteLine("Done!");

            //Console.ReadKey();
        }

        static readonly char[] pathInvalidChars = Path.GetInvalidPathChars();

        static Jenkins96 HandleLine(string line, ParallelLoopState state, Jenkins96 hasher)
        {
            if (string.IsNullOrWhiteSpace(line))
                return hasher;

            if (line.IndexOfAny(pathInvalidChars) != -1)
                return hasher;

            line = line.Replace('/', '\\');

            //string extLower = Path.GetExtension(line).ToLower();

            //if (extLower == ".mdx" || extLower == ".mdl")
            //{
            //    line = Path.ChangeExtension(line, ".m2");
            //    extLower = ".m2";
            //}

            ulong hash = hasher.ComputeHash(line);

            if (!CheckName(hash, line))
                return hasher;

            processed[hash] = true;

            return hasher;
        }

        static Jenkins96 LocalInit()
        {
            return new Jenkins96();
        }

        static void LocalFinal(Jenkins96 local)
        {
            int secs = (int)(DateTime.Now - startTime).TotalSeconds;

            if (secs > 0)
                currentSpeed = (int)(hashCount / secs);

            ConsoleProgressBar.DrawProgressBar(currentGenerator.Percent, 71, '#', false, currentSpeed);
        }

        static void StateHandler()
        {
            while (true)
            {
                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey(true);

                    if (key.Key == ConsoleKey.P)
                    {
                        if (stop)
                        {
                            reset.Reset();
                            ConsoleProgressBar.DrawProgressBar(currentGenerator.Percent, 71, '#', true, 0);
                        }
                        else
                        {
                            reset.Set();
                            ConsoleProgressBar.DrawProgressBar(currentGenerator.Percent, 71, '#', false, currentSpeed);
                        }

                        stop = !stop;
                    }
                }

                Thread.Sleep(50);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static bool CheckName(Jenkins96 hasher, string name)
        {
            return CheckName(hasher.ComputeHash(name), name);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static bool CheckName(ulong hash, string name)
        {
            reset.WaitOne();

            Interlocked.Increment(ref hashCount);

            if (!hashes.ContainsKey(hash))
                return false;

            if (processed.ContainsKey(hash))
                return false;

            if (knownNames.ContainsKey(hash))
                return false;

            knownNames[hash] = name;
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static bool CheckName(Jenkins96 hasher, string format, params object[] args)
        {
            string name = string.Format(format, args);

            return CheckName(hasher, name);
        }

        static void SaveKnownNames(string filename)
        {
            using (StreamWriter sw = File.CreateText(filename))
            {
                var sortedNames = knownNames.OrderBy(n => n.Value, StringComparer.OrdinalIgnoreCase);

                foreach (var name in sortedNames)
                    sw.WriteLine(name.Value);
            }
        }

        static void LoadFileNameGenerators()
        {
            string[] bruteforcers = Directory.GetFiles(".", "*.dll");

            foreach (var bruteforcer in bruteforcers)
            {
                Console.WriteLine(Path.GetFullPath(bruteforcer));

                var asmName = AssemblyLoadContext.GetAssemblyName(bruteforcer);
                Assembly asm = Assembly.Load(asmName);

                IEnumerable<TypeInfo> generators = asm.DefinedTypes.Where(t => t.IsSubclassOf(typeof(FileNameGenerator)));

                foreach (var generator in generators)
                {
                    fileNameGenerators.Add((FileNameGenerator)Activator.CreateInstance(generator.AsType()));
                }
            }

            Console.WriteLine($"Loaded {fileNameGenerators.Count} file name generators!");
        }
    }
}
