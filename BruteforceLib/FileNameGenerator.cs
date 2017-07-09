using System;
using System.Collections.Generic;

namespace BruteforceLib
{
    public abstract class FileNameGenerator
    {
        public virtual string Name => GetType().Name;

        public FileNameGenerator()
        {

        }

        public abstract IEnumerable<string> GetFileNames();

        public abstract IEnumerable<string> GetFileNames(string baseName);
    }
}
