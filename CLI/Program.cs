using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Threading;

namespace Core
{
    class Program
    {
        int singleInt;
        int singleDouble;
        int multiInt;
        int multiDouble;
        double[] disk = new double[4];
        static void Main(string[] args)
        {
            new Program().mainAsync().GetAwaiter().GetResult();
        }
        private async Task mainAsync()
        {
            Console.WriteLine("벤치마크 시작? (y/n)");
            string isStart = Console.ReadLine();
            if (isStart == "y" || isStart == "Y" || isStart == "")
            {
                Stopwatch sw = benchmark();
                int result = ((singleInt + multiInt + singleDouble + multiDouble) / 4 + ((int)(disk[0] + disk[1] * 5 + disk[2] + disk[3] * 5) * 8)) / 5;
                string save = $"벤치마크 결과 (일시: {DateTime.Now}) (걸린 시간: {sw.Elapsed})\n" + 
                $"총 점수: {result}\n" + 
                $"(CPU) 싱글 코어 정수 연산: {singleInt}\n" +
                $"(CPU) 멀티 코어 정수 연산: {multiInt}\n" + 
                $"(CPU) 싱글 코어 실수 연산: {singleDouble}\n" + 
                $"(CPU) 멀티 코어 실수 연산: {multiDouble}\n" + 
                $"(저장장치) 대형 파일 쓰기: {disk[0]}\n" +
                $"(저장장치) 소형 파일 쓰기: {disk[1]}\n" + 
                $"(저장장치) 대형 파일 읽기: {disk[2]}\n" +
                $"(저장장치) 소형 파일 읽기: {disk[3]}";
                File.WriteAllText("벤치마크 결과.txt", save);
                Console.WriteLine("벤치마크가 완료되었습니다 (총 점수: {0}) {1}에 결과 파일이 저장되었습니다", result, Environment.CurrentDirectory + "/벤치마크 결과.txt");
                Console.WriteLine("종료하려면 아무 키나 누르세요");
                Console.ReadKey();
            }
            else
            {
                Console.WriteLine("취소됨");
                Thread.Sleep(500);
            }
        }
        private Stopwatch benchmark()
        {
            SingleCore singleCore = new SingleCore();
            MultiCore multiCore = new MultiCore();
            Disk disk = new Disk();
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            Console.WriteLine("1 / 8 벤치 시작 (단일 코어 정수 연산)");
            singleInt = singleCore.@int();
            Thread.Sleep(5000);
            Console.WriteLine("2 / 8 벤치 시작 (다중 코어 정수 연산)");
            multiInt = multiCore.@int();
            Thread.Sleep(5000);
            Console.WriteLine("3 / 8 벤치 시작 (단일 코어 실수 연산)");
            singleDouble = singleCore.@double();
            Thread.Sleep(5000);
            Console.WriteLine("4 / 8 벤치 시작 (다중 코어 실수 연산)");
            multiDouble = multiCore.@double();
            Thread.Sleep(5000);
            Console.WriteLine("5 / 8 벤치 시작 (대형 파일 쓰기)");
            this.disk[0] = disk.bigWrite();
            Thread.Sleep(5000);
            Console.WriteLine("6 / 8 벤치 시작 (소형 파일 쓰기)");
            this.disk[1] = disk.smallWrite();
            Thread.Sleep(5000);
            Console.WriteLine("7 / 8 벤치 시작 (대형 파일 읽기)");
            this.disk[2] = disk.bigRead();
            Thread.Sleep(5000);
            Console.WriteLine("8 / 8 벤치 시작 (소형 파일 읽기)");
            this.disk[3] = disk.smallRead();
            Thread.Sleep(5000);
            Console.WriteLine("마무리 중...");
            File.Delete("512MiB File");
            File.Delete("512MiB File2");
            Directory.Delete("small files", true);
            stopwatch.Stop();
            return stopwatch;
        }
    }
}
