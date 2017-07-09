using System.Collections.Generic;

namespace BruteforceLib
{
    public abstract class FileNameGenerator
    {
        public virtual string Name => GetType().Name;

        public abstract IEnumerable<string> GetFileNames();

        public abstract IEnumerable<string> GetFileNames(params string[] args);
    }
}
