using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BruteforceLib.Generators
{
    class MergeListFiles : FileNameGenerator
    {
        public override IEnumerable<string> GetFileNames()
        {
            var lists = Directory.GetFiles(ListFilesFolder, "*.txt").OrderBy(n => n);

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
