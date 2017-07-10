using System.Collections.Generic;

namespace BruteforceLib
{
    public class BruteforceConfig
    {
        public static string ListFilesFolder { get; set; }
        public static string DB2Folder { get; set; }
    }

    public abstract class FileNameGenerator
    {
        public virtual string Name => GetType().Name;

        protected int Index = 1;
        protected int Count = 1;

        public float Percent => (float)Index / Count;

        public virtual IEnumerable<string> GetFileNames()
        {
            yield break;
        }
    }
}
