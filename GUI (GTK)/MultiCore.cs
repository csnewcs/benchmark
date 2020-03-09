using System;
using System.Threading;
using System.Diagnostics;
using System.Collections.Generic;

namespace GUI_GTK
{
    public class MultiCore
    {
        object obj = new object();
        ulong allInt = 0;
        double allDouble = 0;
        bool keep = true;
        public int @int()
        {
            int cpus = Environment.ProcessorCount;
            List<Thread> dict = new List<Thread>();
            for (int i = 0; i < cpus; i++)
            {
                dict.Add(new Thread((new ThreadStart(intCore))));
            }
            foreach(var a in dict)
            {
                a.Start();
            }
            Thread.Sleep(10000);
            keep = false;
            Thread.Sleep(10);
            ulong score = allInt / 500000;
            return (int)score;
        }
        public int @double()
        {
            keep = true;
            int cpus = Environment.ProcessorCount;
            List<Thread> dict = new List<Thread>();
            for (int i = 0; i < cpus; i++)
            {
                dict.Add(new Thread((new ThreadStart(doubleCore))));
            }
            foreach(var a in dict)
            {
                a.Start();
            }
            Thread.Sleep(10000);
            keep = false;
            Thread.Sleep(10);
            double score = allDouble / 50000;
            return (int)score;
        }
        public void intCore()
        {
            ulong single = 0;
            while (keep)
            {
                single++;
            }
            lock(obj)
            {
                allInt +=single;
            }
        }
        public void doubleCore()
        {
            double single = 0;
            while (keep)
            {
                single += 0.1;
            }
            lock(obj)
            {
                allDouble += single;
            }
        }
    }
}