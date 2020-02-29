using System;
using Gtk;
using System.Threading.Tasks;
using UI = Gtk.Builder.ObjectAttribute;

namespace benchmark
{
    class MainWindow : Window
    {
        private Label _label1 = new Label();
        private Button _button1 = new Button();
        private ProgressBar _bar1 = new ProgressBar();
        Fixed _fixed = new Fixed();


        public MainWindow() : this(new Builder("MainWindow.glade")) { }

        private MainWindow(Builder builder) : base(builder.GetObject("MainWindow").Handle)
        {
            builder.Autoconnect(this);
            DeleteEvent += Window_DeleteEvent;
            setup();
        }

        private void Window_DeleteEvent(object sender, DeleteEventArgs a)
        {
            Application.Quit();
        }

        private async void Button1_Clicked(object sender, EventArgs a)
        {
            _bar1.Fraction = 0.5;
            for (double abcd = 0; abcd <= 1; abcd += 0.001)
            {
                _bar1.Fraction = abcd;
                await Task.Delay(3);
            }
            _bar1.Fraction = 1;
            
        }

        private void setup()
        {
            _fixed.Put(_bar1, 0, 100);
            _fixed.Put(_button1, 0, 200);
            _button1.Clicked += Button1_Clicked;
            _bar1.Fraction = 0; //GTK.ProgressBar.Fraction = Form.ProgressBar.Value 단 범위는 0~1 고정
            _bar1.Inverted = true;
        }
    }
}
