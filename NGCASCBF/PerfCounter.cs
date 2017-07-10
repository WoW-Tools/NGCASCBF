using System;
using System.Diagnostics;

namespace NGCASCBF
{
    class PerfCounter : IDisposable
    {
        private Stopwatch sw;

        public PerfCounter()
        {
            sw = Stopwatch.StartNew();
        }

        public void Dispose()
        {
            sw.Stop();

            Console.WriteLine(sw.Elapsed);
        }
    }
}
