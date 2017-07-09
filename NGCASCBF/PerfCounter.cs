using System;
using System.Diagnostics;

namespace NGCASCBF
{
    class PerfCounter : IDisposable
    {
        Stopwatch sw;

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
