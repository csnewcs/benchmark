using System;
using System.IO;
using System.Threading.Tasks;

namespace Core
{
    class Program
    {
        static void Main(string[] args)
        {
            new Program().mainAsync().GetAwaiter().GetResult();
        }
        private async Task mainAsync()
        {
            Console.WriteLine("시작? (y/n)");
            string a = Console.ReadLine();
            if (a == "Y"|| a =="y")
            {
                Console.WriteLine("벤치마크를 시작합니다...");
                int score = benchmark();
            }
            else
            {
                Console.WriteLine("취소되었습니다");
                await Task.Delay(1500);
                return;
            }
        }
        private int benchmark()
        {
            SingleCore singleCore = new SingleCore();
            MultiCore multiCore = new MultiCore();
            Disk disk = new Disk();
            Console.WriteLine("정수 연산(싱글 코어) 시작");
            int singleInt = singleCore.@int();
            Console.WriteLine("정수 연산(싱글 코어) 완료");
            Console.WriteLine("정수 연산(멀티 코어) 시작");
            int multiInt = multiCore.@int();
            Console.WriteLine("정수 연산(멀티 코어) 완료");
            Console.WriteLine("실수 연산(싱글 코어) 시작");
            int singleDouble = singleCore.@double();
            Console.WriteLine("실수 연산(싱글 코어) 완료");
            Console.WriteLine("실수 연산(멀티 코어) 시작");
            int multiDouble = multiCore.@double();
            Console.WriteLine("실수 연산(멀티 코어) 완료");
            return 0;
        }
    }
}
