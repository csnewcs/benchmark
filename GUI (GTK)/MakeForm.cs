using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Gtk;

namespace GUI_GTK
{
    partial class Program : Window //창 설정 부분
    {
        Button start = new Button("벤치마크 시작하기"); //이때 전달되는 인자는 버튼의 텍스트, 버튼 생성
        ProgressBar pb = new ProgressBar();
        Label lb = new Label("버튼을 눌러 작업을 시작해주세요");
        CheckButton wantUpload = new CheckButton("벤치마크 결과 업로드");
        Entry nickname = new Entry("닉네임을 입력하세요");
        Button show = new Button("벤치마크 결과 보기");    

        
        public Program() : base("Benchmark")
        {
            SetDefaultSize(450, 120);
            base.Resizable = false;
            SetPosition(WindowPosition.Center);

            VBox vbox = new VBox(false, 2);
            
            Table table = new Table(4, 2, true);
            table.ColumnSpacing = 5;
            table.RowSpacing = 5;

            start.Clicked += startBench;
            show.Clicked += showBench;
            wantUpload.Clicked += checkUpload;

            pb.ShowText = true;
            table.Attach(start, 0, 1, 0, 1);
            table.Attach(show, 1, 2, 0, 1);

            table.Attach(wantUpload, 0, 1, 1, 2);
            table.Attach(nickname, 1, 2, 1, 2);

            table.Attach(lb, 0, 2, 2, 3);

            table.Attach(pb, 0, 2, 3, 4);

            start.SetSizeRequest(250, 40); //크기 지정하기
            vbox.PackEnd(table, true, true, 0);
            
            Add(vbox); //컨테이너 추가
            DeleteEvent += delegate { Application.Quit(); };

            ShowAll(); //모든것을 보여주기
            nickname.Sensitive = false;
        }
    }
}