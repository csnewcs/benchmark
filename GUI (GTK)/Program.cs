using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Gtk;
using System.Threading.Tasks;
using System.Net;
using Newtonsoft.Json.Linq;

namespace GUI_GTK
{
    partial class Program : Window
    {
        bool succeed = false;
        string down = "";
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
            nickname.Sensitive = false;
            wantUpload.Sensitive = false;
            lb.Text = "벤치마크 준비중...";
            Stopwatch sw = new Stopwatch();
            if (wantUpload.Active)
            {
                lb.Text = "사전 준비중...";
                WebClient client = new WebClient();
                client.Encoding = System.Text.Encoding.UTF8;
                string[] readFile = new string[0];
                JObject downloadJson = new JObject();
                if (nickname.Text == "")
                {
                    MessageDialog insertNickname = new MessageDialog(null, DialogFlags.DestroyWithParent, MessageType.Error, ButtonsType.Close, false, "닉네임을 입력하세요");
                    insertNickname.Run();
                    insertNickname.Dispose();
                    start.Sensitive = true;
                    wantUpload.Sensitive = true;
                    return;
                }
                try
                {
                    readFile = File.ReadAllLines("url.txt");
                }
                catch
                {
                    MessageDialog makeUrl = new MessageDialog(null, DialogFlags.DestroyWithParent, MessageType.Info, ButtonsType.Ok, false, "올바른 json서버 url을 적어주세요");
                    makeUrl.Run();
                    makeUrl.Dispose();
                    File.WriteAllText("url.txt", "");
                    Process.Start("url.txt");
                    start.Sensitive = true;
                    wantUpload.Sensitive = true;
                    return;
                }
                try
                {
                    client.DownloadStringCompleted += (sender, s) => {down = s.Result; };
                    client.DownloadStringAsync(new Uri(readFile[0]));
                    downloadJson = JObject.Parse(down);
                }
                catch
                {
                    MessageDialog makeUrl = new MessageDialog(null, DialogFlags.DestroyWithParent, MessageType.Info, ButtonsType.Ok, false, "올바른 json서버 url을 적어주세요");
                    makeUrl.Run();
                    makeUrl.Dispose();
                    Process.Start("url.txt");
                    start.Sensitive = true;
                    wantUpload.Sensitive = true;
                    return;
                }
                if (downloadJson.ContainsKey(nickname.Text))
                {
                    MessageDialog changeNickname = new MessageDialog(null, DialogFlags.DestroyWithParent, MessageType.Error, ButtonsType.Cancel, false, "이미 같은 닉네임이 있습니다. 바꿔주세요.");
                    changeNickname.Run();
                    changeNickname.Dispose();
                    Process.Start("url.txt");
                    start.Sensitive = true;
                    wantUpload.Sensitive = true;
                    nickname.Sensitive = true;
                    nickname.Text = "";
                }
                pb.Fraction += 0.1;
                Thread thread = new Thread(() => benchmark(9, downloadJson, readFile[0]));
                thread.Start();
            }
            else
            {
                Thread thread = new Thread(() => benchmark(8));
                thread.Start();
            }
        }
        private void benchmark(int all, JObject download = null, string url = "")
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
            pb.Fraction += unit;
            diskResult[2] = disk.bigRead();
            GC.Collect();
            Thread.Sleep(5000);

            lb.Text = $"소형 파일 읽기 (8 / 8)";
            pb.Fraction += unit;
            diskResult[3] += disk.smallRead();
            GC.Collect();
            Thread.Sleep(5000);

            lb.Text = "정리 중...";
            pb.Fraction += unit;
            Directory.Delete("small files", true);
            File.Delete("512MiB File");
            File.Delete("512MiB File2");

            int result = ((singleInt + multiInt + singleDouble + multiDouble) / 4 + ((int)(diskResult[0] + diskResult[1] * 5 + diskResult[2] + diskResult[3] * 5) * 8)) / 5;
            sw.Stop();
            DateTime dateTime = DateTime.Now;
            succeed = true;
            string save = $"벤치마크 결과 (일시: {dateTime}) (걸린 시간: {sw.Elapsed})\n" + 
            $"총 점수: {result}\n" + 
            $"(CPU) 싱글 코어 정수 연산: {singleInt}\n" +
            $"(CPU) 멀티 코어 정수 연산: {multiInt}\n" + 
            $"(CPU) 싱글 코어 실수 연산: {singleDouble}\n" + 
            $"(CPU) 멀티 코어 실수 연산: {multiDouble}\n" + 
            $"(저장장치) 대형 파일 쓰기: {diskResult[0]}\n" +
            $"(저장장치) 소형 파일 쓰기: {diskResult[1]}\n" + 
            $"(저장장치) 대형 파일 읽기: {diskResult[2]}\n" +
            $"(저장장치) 소형 파일 읽기: {diskResult[3]}";
            File.WriteAllText("벤치마크 결과.txt", save);
            if (!string.IsNullOrEmpty(url))
            {
                lb.Text = $"업로드 중...";
                pb.Fraction += unit;
                JObject my = new JObject();
                my.Add("all", result);
                JArray cpu = new JArray();
                cpu.Add(singleInt);
                cpu.Add(multiInt);
                cpu.Add(singleDouble);
                cpu.Add(multiDouble);
                my.Add("CPU",cpu);
                JArray ssd = new JArray();
                ssd.Add(diskResult[0]);
                ssd.Add(diskResult[1]);
                ssd.Add(diskResult[2]);
                ssd.Add(diskResult[3]);
                my.Add("Disk", ssd);
                download.Add(nickname.Text, my);
                client.Headers.Add("Content-Type", "application/json");
                client.UploadString(url, "PUT", download.ToString());
            }
            lb.Text = $"벤치마크 완료 (총 점수: {result})\n\'{Environment.CurrentDirectory}/벤치마크 결과.txt\' 에 벤치마크 결과가 저장되었습니다.";
            pb.Fraction = 1;
            start.Sensitive = true;
            wantUpload.Active = false;
            wantUpload.Sensitive = true;
            show.Sensitive = true;
        }
        async void showBench(object objcect, EventArgs e)
        {
            start.Sensitive = false;
            wantUpload.Sensitive = false;
            nickname.Sensitive = false;
            show.Sensitive = false;
            
            WebClient client = new WebClient();
            string[] readFile = new string[0];
            JObject downloadJson = new JObject();
            try
            {
                readFile = File.ReadAllLines("url.txt");
            }
            catch
            {
                MessageDialog makeUrl = new MessageDialog(null, DialogFlags.DestroyWithParent, MessageType.Info, ButtonsType.Ok, false, "올바른 json서버 url을 적어주세요");
                makeUrl.Run();
                makeUrl.Dispose();
                File.WriteAllText("url.txt", "");
                Process.Start("url.txt");
                start.Sensitive = true;
                wantUpload.Sensitive = true;
                return;
            }
            string url = File.ReadAllLines("url.txt")[0];
            bool fail = false;
            bool notFinish = true;
            Thread downloadThread = new Thread(() => 
            {
                Console.WriteLine("시작");
                try
                {
                    down = client.DownloadString(url);
                }
                catch
                {
                    fail = true;
                }
                Console.WriteLine("끝");
                notFinish = false;
            });
            downloadThread.Start();
            while (notFinish)
            {
                await Task.Delay(500);
            }
            Console.WriteLine(down);
            if (fail)
            {
                MessageDialog makeUrl = new MessageDialog(null, DialogFlags.DestroyWithParent, MessageType.Info, ButtonsType.Ok, false, "올바른 json서버 url을 적어주세요");
                makeUrl.Run();
                makeUrl.Dispose();
                Process.Start("url.txt");
                start.Sensitive = true;
                wantUpload.Sensitive = true;
                nickname.Sensitive = true;
                show.Sensitive = true;
                return;
            }
            downloadJson = JObject.Parse(down);
            string[] sorted = sort(downloadJson.DeepClone() as JObject);
            string save = "";
            MessageDialog dialog = new MessageDialog(null, DialogFlags.DestroyWithParent, MessageType.Question, ButtonsType.YesNo, false, "상세정보를 표시하시겠습니까?");
            int detail = dialog.Run();
            dialog.Dispose();
            Console.WriteLine(detail);
            if (detail == -8)
            {
                for (int i = 0; i < downloadJson.Count; ++i)
                {
                    Console.WriteLine(i);
                    JObject one = downloadJson[sorted[i]] as JObject;
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
                    save += $"{i + 1}: {sorted[i]} ({one["all"]})\n{send}\n\n";
                }
            }
            else
            {
                Console.WriteLine(downloadJson.Count);
                for (int i = 0; i < downloadJson.Count; ++i)
                {
                    int score = (int)downloadJson[sorted[i]]["all"];

                    save += $"{i + 1}: {sorted[i]} ({score})\n";
                }
            }
            File.WriteAllText("점수.txt", save);
            Console.WriteLine("{0}/점수.txt에 파일이 저장되었습니다.", Environment.CurrentDirectory);
        }
        private string[] sort(JObject original)
        {
            string[] key = new string[original.Count];
            Console.WriteLine(original.Count);
            int loop = original.Count;
            Console.WriteLine(loop);
            for (int i = 0; i < loop; i++) //이유는 모르겠는데 original.Count하면 얘가 1번 루프 덜함
            {
                Console.WriteLine("루프 " + i);
                int temp = 0;
                foreach (var item in original)
                {
                    JObject user = item.Value as JObject;
                    if ((int)user["all"] >= temp)
                    {
                        key[i] = item.Key;
                        temp = (int)user["all"];
                    }
                }
                original.Remove(key[i]);
            }
            foreach (string s in key)
            {
                Console.WriteLine(s);
            }
            return key;
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