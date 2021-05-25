using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Reflection;

namespace ABSoftware
{
    public class AsyncWork
    {
        public delegate void AsyncWorker(object[] args);

        public static List<Worker> workers = new List<Worker>();

        static Thread thread;
        public static void Init()
        {
            thread = new Thread(AsWorkLoop);
            thread.Start();
        }

        public static void Abort()
        {
            thread.Join(100);
        }

        //AsyncWork.StartNewAW(Print, "printer", new object[] { new string[] { "text" } }); -- will transfer 1 string

        /*
         *  static void Print(object[] args) 
         *  {
         *      Console.WriteLine(args[0].ToString());
         *      AsyncWork.StopWorker("printer");
         *  }
        */
        public static void StartNewAW(AsyncWorker worker, string name, params object[] args)
        {
            StartNew(worker, name, args);
        }

        static void StartNew(Delegate method, string name, params object[] args)
        {
            if (NewWorker(name))
            {
                Worker w = new Worker(args, method, name);
                workers.Add(w);
            }
            else
            {
                StopWorker(name);
                Worker w = new Worker(args, method, name);
                workers.Add(w);
            }
        }

        public static void StopWorker(string name)
        {
            workers.Remove(GetWorker(name));
        }

        private static bool NewWorker(string name)
        {
            foreach (Worker w in workers)
                if (w.name == name)
                    return false;

            return true;
        }

        public static Worker GetWorker(string name)
        {
            return workers.FirstOrDefault(s => Equals(s.name, name));
        }


        private static void AsWorkLoop()
        {
            while(true)
            {
                if(workers.Count > 0 && workers != null)
                {
                    foreach (Worker w in workers.ToList())
                    {
                        w.method.Method.Invoke(null, w.args);
                    }
                }
            }
        }


        public class Worker
        {
            public object[] args;
            public Delegate method;
            public string name;

            public Worker(object[] args, Delegate method, string name)
            {
                this.args = args;
                this.method = method;
                this.name = name;
            }
        }
    }
}
