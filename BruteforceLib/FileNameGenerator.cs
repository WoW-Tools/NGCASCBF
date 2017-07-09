using System.Collections.Generic;

namespace BruteforceLib
{
    public abstract class FileNameGenerator
    {
        public virtual string Name => GetType().Name;

        public static string ListFilesFolder { get; set; }
        public static string DB2Folder { get; set; }

        public virtual IEnumerable<string> GetFileNames()
        {
            yield break;
        }
    }
}
