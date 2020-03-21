using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Gtk;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace GUI_GTK
{
    partial class Program : Window
    {
        bool succeed = false;

        Stopwatch sw;
        static void Main(string[] args)
        {
            Application.Init();
            new Program();
            Application.Run(); //실제 창 만드는건 MakeForm.cs
        }
        private void startBench(object sender, EventArgs e)
        {
            start.Sensitive = false;
            show.Sensitive = false;
            wantUpload.Sensitive = false;
            lb.Text = "벤치마크 준비중...";
            Stopwatch sw = new Stopwatch();
            if (wantUpload.Active)
            {
                lb.Text = "사전 준비중...";
                WebClient client = new WebClient();
                client.Encoding = System.Text.Encoding.UTF8;
                string[] readFile = new string[0];
                string downloadJson = "";
                Console.WriteLine("0");
                if (nickname.Text == "")
                {
                    MessageDialog insertNickname = new MessageDialog(null, DialogFlags.DestroyWithParent, MessageType.Error, ButtonsType.Close, false, "닉네임을 입력하세요");
                    insertNickname.Run();
                    insertNickname.Dispose();
                }
                try
                {
                    readFile = File.ReadAllLines("url.txt");
                }
                catch
                {
                    MessageDialog makeUrl = new MessageDialog(null, DialogFlags.DestroyWithParent, MessageType.Question, ButtonsType.YesNo, false, "url.txt파일을 만드세요");
                    makeUrl.Run();
                    makeUrl.Dispose();
                }
                downloadJson = client.DownloadString(readFile[0]);
                float unit = 0.1f;
                pb.Fraction += unit;
                Thread thread = new Thread(() => benchmark(10, downloadJson, readFile[0]));
                thread.Start();
            }
            else
            {
                Thread thread = new Thread(() => benchmark(8));
                thread.Start();
            }
        }
        private void benchmark(int all, string download = "", string url = "")
        {
            sw = new Stopwatch();

            // 벤치마크 메서드가 있는 클래스 선언
            SingleCore singleCore = new SingleCore();
            MultiCore multiCore = new MultiCore();
            Disk disk = new Disk();

            // 값을 저장할 변수
            int singleInt;
            int singleDouble;
            int multiInt;
            int multiDouble;
            double[] diskResult = new double[4];

            // 한 번에 더할 것 정하기
            double unit = 1.0d / (all + 1);
            unit = Math.Truncate(unit * 100) / 100;
            
            // 업로드 & 다운로드용
            WebClient client = new WebClient();

            sw.Start();
            lb.Text = $"단일 코어 정수 연산 (1 / 8)";
            singleInt = singleCore.@int();
            GC.Collect();
            Thread.Sleep(5000);

            lb.Text = $"다중 코어 정수 연산 (2 / 8)";
            pb.Fraction += unit;
            multiInt = multiCore.@int();
            GC.Collect();
            Thread.Sleep(5000);

            lb.Text = $"단일 코어 실수 연산 (3 / 8)";
            pb.Fraction += unit;
            singleDouble = singleCore.@double();
            GC.Collect();
            Thread.Sleep(5000);

            lb.Text = $"다중 코어 실수 연산 (4 / 8)";
            pb.Fraction += unit;
            multiDouble = multiCore.@double();
            GC.Collect();
            Thread.Sleep(5000);

            lb.Text = $"대형 파일 쓰기 (5 / 8)";
            pb.Fraction += unit;
            diskResult[0] = disk.bigWrite();
            GC.Collect();
            Thread.Sleep(5000);

            lb.Text = $"소형 파일 쓰기 (6 / 8)";
            pb.Fraction += unit;
            diskResult[1] = disk.smallWrite();
            GC.Collect();
            Thread.Sleep(5000);

            lb.Text = $"대형 파일 읽기 (7 / 8)";
            pb.Fraction = unit;
            diskResult[2] = disk.bigRead();
            GC.Collect();
            Thread.Sleep(5000);

            lb.Text = $"소형 파일 읽기 (8 / 8)";
            pb.Fraction = unit;
            diskResult[3] = disk.smallRead();
            GC.Collect();
            Thread.Sleep(5000);

            lb.Text = "정리 중...";
            pb.Fraction = unit;
            Directory.Delete("small files", true);
            File.Delete("512MiB File");
            File.Delete("512MiB File2");

            all = ((singleInt + multiInt + singleDouble + multiDouble) / 4 + ((int)(diskResult[0] + diskResult[1] * 5 + diskResult[2] + diskResult[3] * 5) * 8)) / 5;
            sw.Stop();
            DateTime dateTime = DateTime.Now;
            succeed = true;
            pb.Fraction = 1;
            string save = $"벤치마크 결과 (일시: {dateTime}) (걸린 시간: {sw.Elapsed})\n" + 
            $"총 점수: {all}\n" + 
            $"(CPU) 싱글 코어 정수 연산: {singleInt}\n" +
            $"(CPU) 멀티 코어 정수 연산: {multiInt}\n" + 
            $"(CPU) 싱글 코어 실수 연산: {singleDouble}\n" + 
            $"(CPU) 멀티 코어 실수 연산: {multiDouble}\n" + 
            $"(저장장치) 대형 파일 쓰기: {diskResult[0]}\n" +
            $"(저장장치) 소형 파일 쓰기: {diskResult[1]}\n" + 
            $"(저장장치) 대형 파일 읽기: {diskResult[2]}\n" +
            $"(저장장치) 소형 파일 읽기: {diskResult[3]}";
            File.WriteAllText("벤치마크 결과.txt", save);
            lb.Text = $"벤치마크 완료 (총 점수: {all})\n\'{Environment.CurrentDirectory}/벤치마크 결과.txt\' 에 벤치마크 결과가 저장되었습니다.";
            start.Sensitive = true;
            wantUpload.Active = false;
            wantUpload.Sensitive = true;
            show.Sensitive = true;
        }
        private void up(string name, int singleInt, int multiInt, int singleDouble, int multiDouble, double[] diskResult)
        {
            WebClient client = new WebClient();
            string url = File.ReadAllLines("url.txt")[0];
            JObject all  = new JObject();
            while (true)
            {
                try
                {
                    all = JObject.Parse(client.DownloadString(url));
                    break;
                }
                catch (TimeoutException)
                {
                    Console.WriteLine("타임아웃 재시도");
                }
                catch
                {
                    Console.WriteLine("저런 올바른 url이 아니거나 담긴 정보가 json이 아니에요");
                    return;
                }
            }
            while (all.ContainsKey(name))
            {
                
            }
            JObject my = new JObject();
            my.Add("all", all);
            JArray cpu = new JArray();
            cpu.Add(singleInt);
            cpu.Add(multiInt);
            cpu.Add(singleDouble);
            cpu.Add(multiDouble);
            my.Add("CPU",cpu);
            JArray disk = new JArray();
            disk.Add(diskResult[0]);
            disk.Add(diskResult[1]);
            disk.Add(diskResult[2]);
            disk.Add(diskResult[3]);
            my.Add("Disk", disk);
            all.Add(name, my);
            client.Headers.Add("Content-Type", "application/json");
            client.UploadString(url, "PUT", all.ToString());
            Console.WriteLine("업로드 완료");
        }
        void showBench(object objcect, EventArgs e)
        {

        }
        string text = "";
        void checkUpload(object objcect, EventArgs e)
        {
            bool check = wantUpload.Active;
            show.Sensitive = !check;
            nickname.Sensitive = check;
            if (check)
            {
                nickname.Text = text;
            }
            else
            {
                text = nickname.Text;
                nickname.Text = "닉네임을 입력하세요";
            }
        }
    }
}