using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Threading;
using Newtonsoft.Json.Linq;
using System.Net;

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
            new Program().core();
        }
        private void core()
        {
            Console.WriteLine("벤치마크 시작은 y를 결과 보기는 s를 프로그램 종료는 n을 입력해 주세요");
            string isStart = Console.ReadLine();
            if (isStart == "y" || isStart == "Y" || isStart == "")
            {
                DateTime now = DateTime.Now;
                Stopwatch sw = benchmark();
                int result = ((singleInt + multiInt + singleDouble + multiDouble) / 4 + ((int)(disk[0] + disk[1] * 5 + disk[2] + disk[3] * 5) * 8)) / 5;
                string save = $"벤치마크 결과 (일시: {now}) (걸린 시간: {sw.Elapsed})\n" + 
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
                Console.WriteLine("벤치마크 결과를 공유하시겠습니까? (y/n)");
                string share = Console.ReadLine();
                if (share == "y" || share == "Y" || share == "")
                {
                    Console.WriteLine("닉네임을 입력해 주세요");
                    string name = Console.ReadLine();
                    up(name, now, result);
                }
                Console.WriteLine("종료하려면 아무 키나 누르세요");
                Console.ReadKey();
            }
            else if (isStart == "s" || isStart == "S")
            {
                see();
            }
            else
            {
                Console.WriteLine("취소됨");
                Thread.Sleep(500);
            }
        }
        private Stopwatch benchmark()
        {
            //클래스 선언
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
            GC.Collect();
            Thread.Sleep(5000);
            Console.WriteLine("6 / 8 벤치 시작 (소형 파일 쓰기)");
            this.disk[1] = disk.smallWrite();
            GC.Collect();
            Thread.Sleep(5000);
            Console.WriteLine("7 / 8 벤치 시작 (대형 파일 읽기)");
            this.disk[2] = disk.bigRead();
            GC.Collect();
            Thread.Sleep(5000);
            Console.WriteLine("8 / 8 벤치 시작 (소형 파일 읽기)");
            this.disk[3] = disk.smallRead();
            GC.Collect();
            Thread.Sleep(5000);
            Console.WriteLine("마무리 중...");
            File.Delete("512MiB File");
            File.Delete("512MiB File2");
            Directory.Delete("small files", true);
            stopwatch.Stop();
            GC.Collect();
            return stopwatch;
        }
        private void up(string name, DateTime now, int result)
        {
            WebClient client = new WebClient();
            string url = File.ReadAllLines("url.txt")[0];
            JObject all  = new JObject();
            try
            {
                all = JObject.Parse(client.DownloadString(url));
            }
            catch
            {
                Console.WriteLine("저런 올바른 url이 아니거나 담긴 정보가 json이 아니에요");
                return;
            }
            while (all.ContainsKey(name))
            {
                Console.WriteLine("이미 같은 이름이 있습니다. 다른 이름을 입력해 주세요");
                name = Console.ReadLine();
            }
            JObject my = new JObject();
            my.Add("all", result);
            JArray cpu = new JArray();
            cpu.Add(singleInt);
            cpu.Add(multiInt);
            cpu.Add(singleDouble);
            cpu.Add(multiDouble);
            my.Add("CPU",cpu);
            JArray disk = new JArray();
            disk.Add(this.disk[0]);
            disk.Add(this.disk[1]);
            disk.Add(this.disk[2]);
            disk.Add(this.disk[3]);
            my.Add("Disk", disk);
            all.Add(name, my);
            client.Headers.Add("Content-Type", "application/json");
            client.UploadString(url, "PUT", all.ToString());
            Console.WriteLine("업로드 완료");
        }
        private void see()
        {
            WebClient client = new WebClient();
            Console.WriteLine("상세 점수 표시?(y/n)");
            string allScore = Console.ReadLine();
            try
            {
                File.ReadAllLines("url.txt");
            }
            catch
            {
                Console.WriteLine("url.txt파일을 만들어 주세요");
                return;
            }
            string url = File.ReadAllLines("url.txt")[0];
            JObject all  = new JObject();
            try
            {
                all = JObject.Parse(client.DownloadString(url));
            }
            catch (Exception e)
            {
                Console.WriteLine("저런 올바른 url이 아니거나 담긴 정보가 json이 아니에요");
                return;
            }
            string[] sorted = sort(all.DeepClone() as JObject);

            if (allScore == "y" || allScore == "Y" || allScore == "")
            {
                string save = "";
                for (int i = 0; i < all.Count; i++)
                {
                    JObject one = all[sorted[i]] as JObject;
                    string send = "CPU\n" + 
                    $"\t싱글 코어 정수연산: {one["CPU"][0]}\n" +
                    $"\t멀티 코어 정수연산: {one["CPU"][1]}\n" +
                    $"\t싱글 코어 실수연산: {one["CPU"][2]}\n" +
                    $"\t멀티 코어 실수연산: {one["CPU"][3]}\n" + 
                    "저장장치\n" + 
                    $"\t대형 파일 쓰기: {one["Disk"][0]}\n" + 
                    $"\t소형 파일 쓰기: {one["Disk"][1]}\n" +
                    $"\t대형 파일 읽기: {one["Disk"][2]}\n" +
                    $"\t소형 파일 읽기: {one["Disk"][3]}";
                    save += $"{i + 1}: {sorted[i]} ({one["all"]})\n{send}\n";
                    File.WriteAllText("점수.txt", send);
                    Console.WriteLine("{0}에 파일이 저장되었습니다.", Environment.CurrentDirectory);
                }
            }
            else
            {
                for (int i = 0; i < all.Count; i++)
                {
                    int score = (int)all[sorted[i]]["all"];
                    Console.WriteLine($"{i + 1}: {sorted[i]} ({score})");
                }
            }
            
        }
        private string[] sort(JObject original)
        {
            string[] key = new string[original.Count];
            for (int i = 0; i <= original.Count; i++)
            {
                int temp = 0;
                foreach (var item in original)
                {
                    JObject user = item.Value as JObject;
                    if ((int)user["all"] > temp)
                    {
                        key[i] = item.Key;
                        temp = (int)user["all"];
                    }
                }
                original.Remove(key[i]);
            }
            return key;
        }
    }
}
