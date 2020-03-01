using System.Diagnostics;
using System;
using System.Threading;

namespace Core
{
    public class SingleCore
    {
        ulong single = 0;
        double singleDouble = 0;
        bool keep = true;
        public int @int()
        {
            Thread thread = new Thread(new ThreadStart(intCore));
            thread.Start();
            Thread.Sleep(10000);
            keep = false;
            Thread.Sleep(10);
            ulong asdf = single / 500000;
            Console.WriteLine(asdf);
            return (int)asdf;
        }
        public int @double()
        {
            keep = true;
            Thread thread = new Thread(new ThreadStart(doubleCore));
            thread.Start();
            Thread.Sleep(10000);
            keep = false;
            Thread.Sleep(10);
            double asdf = singleDouble / 50000;
            Console.WriteLine((int)asdf);
            return (int)asdf;
        }
        private void intCore()
        {
            while (keep)
            {
                single++;
            }
        }
        private void doubleCore()
        {
            while (keep)
            {
                singleDouble += 0.1;
            }
        }
    }
}