using System;
using System.IO;
using System.Diagnostics;
using System.Threading;

namespace GUI_GTK
{
    public class Disk : IDisposable
    {
        public void Dispose()
        {
            Console.WriteLine("해제");
        }
        public double bigWrite()
        {
            Stopwatch sw = new Stopwatch();
            byte[] big = new byte[536870912];
            string bigSave = System.Text.Encoding.UTF8.GetString(big);
            sw.Start();
            File.WriteAllText("512MiB File", bigSave);
            File.WriteAllText("512MiB File2", bigSave);
            sw.Stop();
            double speed = 1024 / ((double)sw.Elapsed.Ticks / 10000000);
            return Math.Round(speed, 1);
        }
        public double smallWrite()
        {
            Stopwatch sw = new Stopwatch();
            Directory.CreateDirectory("small files");
            byte[] big = new byte[4096];
            string bigSave = System.Text.Encoding.UTF8.GetString(big);
            sw.Start();
            for (int i = 0; i < 262144; i++)
            {
                File.WriteAllText("small files/" + i, bigSave);
            }
            sw.Stop();
            double speed = 1024 / ((double)sw.Elapsed.Ticks / 10000000);
            return Math.Round(speed, 1);
        }
        public double bigRead()
        {
            Stopwatch sw = new Stopwatch();
            sw.Reset();
            sw.Start();
            File.ReadAllText("512MiB File");
            GC.Collect();
            File.ReadAllText("512MiB File2");
            sw.Stop();
            double speed = 1024 / ((double)sw.Elapsed.Ticks / 10000000);
            return Math.Round(speed, 1);
        }
        public double smallRead()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            for (int i = 0; i < 262144; )
            {
                File.ReadAllText("small files/" + i);
                i++;
            }
            double speed = 1024 / ((double)sw.Elapsed.Ticks / 10000000);
            return Math.Round(speed, 1);
        }
    }
}