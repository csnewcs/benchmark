using System;
using System.IO;
using System.Diagnostics;

namespace Core
{
    public class Disk
    {
        public double[] diskTest()
        {
            double[] result = new double[4];
            Stopwatch sw = new Stopwatch();


            sw.Start();
            File.WriteAllBytes("1GiB File", new byte[1073741824]);
            sw.Stop();
            double speed = 1024 / ((double)sw.Elapsed.Ticks / 10000000);
            Console.WriteLine(sw.Elapsed + "      " + Math.Round(speed, 1) + "MiB / s");
            result[0] = Math.Round(speed, 1);

            sw.Reset();
            Directory.CreateDirectory("small files");
            sw.Start();
            for (int i = 0; i < 262144; i++)
            {
                File.WriteAllBytes("small files/" + i.ToString(), new byte[4096]);
            }
            sw.Stop();
            speed = 1024 / ((double)sw.Elapsed.Ticks / 10000000);
            Console.WriteLine(sw.Elapsed + "      " + Math.Round(speed, 1) + "MiB / s");
            result[1] = Math.Round(speed, 1);

            sw.Reset();
            sw.Start();
            byte[] read = File.ReadAllBytes("1GiB File");
            sw.Stop();
            speed = 1024 / ((double)sw.Elapsed.Ticks / 10000000);
            Console.WriteLine(sw.Elapsed + "      " + Math.Round(speed, 1) + "MiB / s");
            result[2] = Math.Round(speed, 1);
            read = new byte[0];

            sw.Reset();
            byte[][] reads = new byte[262144][];
            sw.Start();
            for (int i = 0; i < 262144; )
            {
                reads[i] = File.ReadAllBytes("small files/" + i.ToString());
                i++;
            }
            speed = 1024 / ((double)sw.Elapsed.Ticks / 10000000);
            Console.WriteLine(sw.Elapsed + "      " + Math.Round(speed, 1) + "MiB / s");
            result[3] = Math.Round(speed, 1);
            reads = new byte[0][];
            Console.WriteLine("ㅈㅁ 기다려봐 정리점");
            Directory.Delete("small files", true);
            File.Delete("1GiB File");
            return result;
        }
    }
}