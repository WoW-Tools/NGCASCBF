using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BruteforceLib.Generators
{
    class MergeListFiles : FileNameGenerator
    {
        public override IEnumerable<string> GetFileNames()
        {
            yield break;
        }

        public override IEnumerable<string> GetFileNames(params string[] args)
        {
            string dataPath = args[0];

            var lists = Directory.GetFiles(dataPath, "*.txt").OrderBy(n => n);

            TextFileEnumerator currentFile;

            foreach (string list in lists)
            {
                currentFile = new TextFileEnumerator(list);

                foreach (string fileName in currentFile)
                    yield return fileName;
            }
        }
    }
}
