using System;
using System.IO;
using System.Diagnostics;
using System.Text;

namespace Core
{
    public class Disk
    {
        public double[] diskTest()
        {
            double[] result = new double[4];
            Stopwatch sw = new Stopwatch();

            sw.Start();
            File.WriteAllBytes("512MiB File", new byte[536870912]);
            File.WriteAllBytes("512MiB File2", new byte[536870912]);
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
            System.Threading.Thread.Sleep(5000);
            sw.Start();
            //byte[] read = File.ReadAllBytes("1GiB File");
            File.ReadAllText("512MiB File");
            File.ReadAllText("512MiB File2");
            sw.Stop();
            speed = 1024 / ((double)sw.Elapsed.Ticks / 10000000);
            Console.WriteLine(sw.Elapsed + "      " + Math.Round(speed, 1) + "MiB / s");
            result[2] = Math.Round(speed, 1);
            //read = new byte[0];
            
            sw.Reset();

            sw.Start();
            for (int i = 0; i < 262144; )
            {
                File.ReadAllText("small files/" + i);
                i++;
            }
            speed = 1024 / ((double)sw.Elapsed.Ticks / 10000000);
            Console.WriteLine(sw.Elapsed + "      " + Math.Round(speed, 1) + "MiB / s");
            result[3] = Math.Round(speed, 1);
            Console.WriteLine("ㅈㅁ 기다려봐 정리점");
            Directory.Delete("small files", true);
            File.Delete("512MiB File");
            File.Delete("512MiB File2");
             return result;
        }
    }
}