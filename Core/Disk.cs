using System;
using System.IO;
using System.Diagnostics;
using System.Drawing;

namespace Core
{
    public class Disk
    {
        public double[] diskTest()
        {
            double[] result = new double[4];
            Stopwatch sw = new Stopwatch();
            byte[] big = new byte[536870912];
            string bigSave = System.Text.Encoding.UTF8.GetString(big);
            sw.Start();
            File.WriteAllText("512MiB File", bigSave);
            File.WriteAllText("512MiB File2", bigSave);
            sw.Stop();
            double speed = 1024 / ((double)sw.Elapsed.Ticks / 10000000);
            Console.WriteLine(sw.Elapsed + "      " + Math.Round(speed, 1));
            result[0] = Math.Round(speed, 1);
            
            sw.Reset();
            Directory.CreateDirectory("small files");
            big = new byte[4096];
            bigSave = System.Text.Encoding.UTF8.GetString(big);
            sw.Start();
            for (int i = 0; i < 262144; i++)
            {
                File.WriteAllText("small files/" + i, bigSave);
            }
            sw.Stop();
            speed = 1024 / ((double)sw.Elapsed.Ticks / 10000000);
            Console.WriteLine(sw.Elapsed + "      " + Math.Round(speed, 1));
            result[1] = Math.Round(speed, 1);
            
            sw.Reset();
            sw.Start();
            bigSave = File.ReadAllText("512MiB File");
            bigSave = File.ReadAllText("512MiB File2");
            sw.Stop();
            speed = 1024 / ((double)sw.Elapsed.Ticks / 10000000);
            Console.WriteLine(sw.Elapsed + "      " + Math.Round(speed, 1));
            result[2] = Math.Round(speed, 1);
            
            sw.Reset();
            sw.Start();
            for (int i = 0; i < 262144; )
            {
                bigSave = File.ReadAllText("small files/" + i);
                i++;
            }
            speed = 1024 / ((double)sw.Elapsed.Ticks / 10000000);
            Console.WriteLine(sw.Elapsed + "      " + Math.Round(speed, 1));
            result[3] = Math.Round(speed, 1);
            Console.WriteLine("ㅈㅁ 기다려봐 정리점");
            Directory.Delete("small files", true);
            File.Delete("512MiB File");
            File.Delete("512MiB File2");
             return result;
        }
    }
}