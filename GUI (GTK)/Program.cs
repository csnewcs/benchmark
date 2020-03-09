using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Gtk;
using System.Threading.Tasks;

namespace GUI_GTK
{
    class Program : Window
    {
        int singleInt;
        int singleDouble;
        int multiInt;
        int multiDouble;
        double[] disk = new double[4];
        Button start = new Button("벤치마크 시작하기"); //이때 전달되는 인자는 버튼의 텍스트, 버튼 생성
        ProgressBar pb = new ProgressBar();
        Label lb = new Label("\'벤치마크 시작하기\' 버튼을 눌러주세요");
        public Program() : base("Benchmark")
        {
            SetDefaultSize(450, 120);
            base.Resizable = false;
            SetPosition(WindowPosition.Center);

            VBox vbox = new VBox(false, 2);
            
            Table table = new Table(3, 1, true);
            start.Clicked += startBench;
            pb.ShowText = true;
            table.Attach(start, 0, 1, 0, 1);
            table.Attach(lb, 0, 1, 1, 2);
            table.Attach(pb, 0, 1, 2, 3);
            start.SetSizeRequest(250, 40); //크기 지정하기
            vbox.PackEnd(table, true, true, 0);
            
            Add(vbox); //컨테이너 추가
            DeleteEvent += delegate { Application.Quit(); };

            ShowAll(); //모든것을 보여주기

        }
        static void Main(string[] args)
        {
            Application.Init();
            new Program();
            Application.Run();
        }
        private void startBench(object sender, EventArgs e)
        {
            start.Sensitive = false;
            Thread thread= new Thread(new ThreadStart(loop));
            thread.Start();
        }
        private void loop()
        {
            lb.Text = "벤치마크 준비중...";
            Stopwatch sw = benchmark();
            int result = ((singleInt + multiInt + singleDouble + multiDouble) + ((int)(disk[0] + disk[1] + disk[2] + disk[3]) / 2)) / 5;
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
            lb.Text = $"벤치마크 완료 (총 점수: {result})\n\'{Environment.CurrentDirectory}/벤치마크 결과.txt\' 에 벤치마크 결과가 저장되었습니다.";
        }
        private Stopwatch benchmark()
        {
            SingleCore singleCore = new SingleCore();
            MultiCore multiCore = new MultiCore();
            Disk disk = new Disk();
            Stopwatch stopwatch = new Stopwatch();

            stopwatch.Start();
            lb.Text = "단일 코어 정수 연산 (1 / 8)";
            singleInt = singleCore.@int();
            Thread.Sleep(5000);

            lb.Text = "다중 코어 정수 연산 (2 / 8)";
            pb.Fraction = 0.11f;
            multiInt = multiCore.@int();
            Thread.Sleep(5000);

            lb.Text = "단일 코어 실수 연산 (3 / 8)";
            pb.Fraction = 0.22f;
            singleDouble = singleCore.@double();
            Thread.Sleep(5000);

            lb.Text = "다중 코어 실수 연산 (4 / 8)";
            pb.Fraction = 0.33f;
            multiDouble = multiCore.@double();
            Thread.Sleep(5000);

            lb.Text = "대형 파일 쓰기 (5 / 8)";
            pb.Fraction = 0.44f;
            this.disk[0] = disk.bigWrite();
            

            lb.Text = "소형 파일 쓰기 (6 / 8)";
            Thread.Sleep(5000);
            pb.Fraction = 0.55f;
            this.disk[1] = disk.smallWrite();
            Thread.Sleep(5000);

            lb.Text = "대형 파일 읽기 (7 / 8)";
            pb.Fraction = 0.66f;
            this.disk[2] = disk.bigRead();
            Thread.Sleep(5000);

            lb.Text = "소형 파일 읽기 (8 / 8)";
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
    }
}
