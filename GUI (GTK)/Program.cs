using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Gtk;
using System.Threading.Tasks;

namespace GUI_GTK
{
    partial class Program : Window
    {
        int singleInt;
        int singleDouble;
        int multiInt;
        int multiDouble;
        double[] disk = new double[4];
        bool isUpload = false;
        
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
            Thread thread= new Thread(new ThreadStart(loop));
            thread.Start();
        }
        private void loop()
        {
            lb.Text = "벤치마크 준비중...";
            Stopwatch sw = new Stopwatch();
            if (isUpload)
            {
                sw = benchmark(9);
            }
            else
            {
                sw = benchmark(8);
            }
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
            start.Sensitive = true;
            show.Sensitive = !isUpload;
            lb.Text = $"벤치마크 완료 (총 점수: {result})\n\'{Environment.CurrentDirectory}/벤치마크 결과.txt\' 에 벤치마크 결과가 저장되었습니다.";
        }
        private Stopwatch benchmark(int all)
        {
            SingleCore singleCore = new SingleCore();
            MultiCore multiCore = new MultiCore();
            Disk disk = new Disk();
            Stopwatch stopwatch = new Stopwatch();

            stopwatch.Start();
            lb.Text = $"단일 코어 정수 연산 (1 / {all})";
            singleInt = singleCore.@int();
            Thread.Sleep(5000);

            lb.Text = $"다중 코어 정수 연산 (2 / {all})";
            pb.Fraction = 0.11f;
            multiInt = multiCore.@int();
            Thread.Sleep(5000);

            lb.Text = $"단일 코어 실수 연산 (3 / {all})";
            pb.Fraction = 0.22f;
            singleDouble = singleCore.@double();
            Thread.Sleep(5000);

            lb.Text = $"다중 코어 실수 연산 (4 / {all})";
            pb.Fraction = 0.33f;
            multiDouble = multiCore.@double();
            Thread.Sleep(5000);

            lb.Text = $"대형 파일 쓰기 (5 / {all})";
            pb.Fraction = 0.44f;
            this.disk[0] = disk.bigWrite();
            GC.Collect();

            lb.Text = $"소형 파일 쓰기 (6 / {all})";
            Thread.Sleep(5000);
            pb.Fraction = 0.55f;
            this.disk[1] = disk.smallWrite();
            Thread.Sleep(5000);

            lb.Text = $"대형 파일 읽기 (7 / {all})";
            pb.Fraction = 0.66f;
            this.disk[2] = disk.bigRead();
            Thread.Sleep(5000);

            lb.Text = $"소형 파일 읽기 (8 / {all})";
            pb.Fraction = 0.77f;
            this.disk[3] = disk.smallRead();

            lb.Text = "정리 중...";
            pb.Fraction = 0.88f;
            Directory.Delete("small files", true);
            File.Delete("512MiB File");
            File.Delete("512MiB File2");

            pb.Fraction = 1;
            stopwatch.Stop();
            return stopwatch;
        }
        void showBench(object objcect, EventArgs e)
        {

        }
        void checkUpload(object objcect, EventArgs e)
        {
            show.Sensitive = isUpload;
            isUpload = !isUpload;
            nickname.Sensitive = isUpload;
        }
    }
}
