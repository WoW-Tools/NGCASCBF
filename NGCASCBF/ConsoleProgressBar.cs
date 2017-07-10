using System;

namespace NGCASCBF
{
    class ConsoleProgressBar
    {
        private static object lockObj = new object();

        public static void DrawProgressBar(long complete, long maxVal, int barSize, char progressCharacter, bool paused, int speed)
        {
            double perc = (double)complete / maxVal;
            DrawProgressBar(perc, barSize, progressCharacter, paused, speed);
        }

        public static void DrawProgressBar(double percent, int barSize, char progressCharacter, bool paused, int speed)
        {
            lock (lockObj)
            {
                int chars = (int)Math.Round(percent / (1.0 / barSize));
                Console.CursorVisible = false;
                Console.ForegroundColor = ConsoleColor.Green;
                Console.CursorLeft = 0;
                Console.Write(new string(progressCharacter, chars));
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.Write(new string(progressCharacter, barSize - chars));
                Console.ResetColor();
                if (paused)
                    Console.Write(" PAUSE ");
                else
                    Console.Write(" {0:N2}% ({1} hashes/sec)", percent * 100, speed);
            }
        }
    }
}
