using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace Core
{
    class Program
    {
        int singleInt;
        int singleDouble;
        int multiInt;
        int multiDouble;
        double[] disk;
        static void Main(string[] args)
        {
            new Program().mainAsync().GetAwaiter().GetResult();
        }
        private async Task mainAsync()
        {
            Console.WriteLine("시작? (y/n)");
            string a = Console.ReadLine();
            if (a == "Y"|| a =="y" || a =="")
            {
                Console.WriteLine("벤치마크를 시작합니다...");
                Stopwatch sw = benchmark();
                Console.WriteLine(sw.Elapsed);
            }
            else
            {
                Console.WriteLine("취소되었습니다");
                await Task.Delay(1500);
                return;
            }
            int result = ((singleInt + multiInt + singleDouble + multiDouble) + ((int)(disk[0] + disk[1] + disk[2] + disk[3]) / 2)) / 5;
            Console.WriteLine("총 점수: {0}", result);
            string save = $"벤치마크 결과 (일시: {DateTime.Now})\n" + 
            $"총 점수: {result}\n" + 
            $"(CPU) 싱글 코어 정수 연산: {singleInt}\n" +
            $"(CPU) 멀티 코어 정수 연산: {multiInt}\n" + 
            $"(CPU) 싱글 코어 실수 연산: {singleDouble}\n" + 
            $"(CPU) 멀티 코어 실수 연산: {multiDouble}\n" + 
            $"(저장장치) 대형 파일 쓰기: {disk[0]} MiB/s\n" +
            $"(저장장치) 소형 파일 쓰기: {disk[1]} MiB/s\n" + 
            $"(저장장치) 대형 파일 읽기: {disk[2]} MiB/s\n" +
            $"(저장장치) 소형 파일 읽기: {disk[3]} MiB/s";
            File.WriteAllText("벤치마크 결과.txt", save);
        }
        private Stopwatch benchmark()
        {
            SingleCore singleCore = new SingleCore();
            MultiCore multiCore = new MultiCore();
            Disk disk = new Disk();
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            Console.WriteLine("0/5");
            singleInt = singleCore.@int();
            Console.WriteLine("1/5");
            multiInt = multiCore.@int();
            Console.WriteLine("2/5");
            singleDouble = singleCore.@double();
            Console.WriteLine("3/5");
            multiDouble = multiCore.@double();
            Console.WriteLine("4/5");
            this.disk = disk.diskTest();
            Console.WriteLine("5/5");
            stopwatch.Stop();
            return stopwatch;
        }
    }
}
