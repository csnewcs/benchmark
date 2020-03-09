using System;
using System.IO;
using System.Diagnostics;
using System.Threading;

namespace Core
{
    public class Disk
    {
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
            byte[] small = new byte[4096];
            string smallSave = System.Text.Encoding.UTF8.GetString(small);
            sw.Start();
            for (int i = 0; i < 262144; i++)
            {
                File.WriteAllText("small files/" + i, smallSave);
            }
            sw.Stop();
            double speed = 1024 / ((double)sw.Elapsed.Ticks / 10000000);
            return Math.Round(speed, 1);
        }
        public double bigRead()
        {
            Stopwatch sw =new Stopwatch();
            sw.Start();
            File.ReadAllText("512MiB File");
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