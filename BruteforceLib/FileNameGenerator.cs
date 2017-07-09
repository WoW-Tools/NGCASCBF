using System.Collections.Generic;

namespace BruteforceLib
{
    public abstract class FileNameGenerator
    {
        public virtual string Name => GetType().Name;

        public virtual IEnumerable<string> GetFileNames()
        {
            yield break;
        }

        public virtual IEnumerable<string> GetFileNames(params string[] args)
        {
            yield break;
        }
    }
}
