using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;

namespace BruteforceLib
{
    class TextFileEnumerator : IDisposable, IEnumerable<string>
    {
        private StreamReader sr;
        public string Name { get; private set; }

        public TextFileEnumerator(string fileName)
        {
            Name = fileName;
            sr = File.OpenText(fileName);
        }

        public void Dispose() => sr.Dispose();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerator<string> GetEnumerator()
        {
            sr.BaseStream.Position = 0;
            while (!sr.EndOfStream)
            {
                yield return sr.ReadLine();
            }
        }

        public float Percent => (float)sr.BaseStream.Position / sr.BaseStream.Length;

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
