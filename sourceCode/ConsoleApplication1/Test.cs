using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace ConsoleApplication1
{
    class Test
    {
        public static ManualResetEvent mre = new ManualResetEvent(false);

        public static void trmain()
        {
            Thread tr = Thread.CurrentThread;
            Console.WriteLine("thread: waiting for an event");
            mre.WaitOne();
            Console.WriteLine("thread: got an event");
            for (int x = 0; x < 10; x++)
            {
                Thread.Sleep(1000);
                Console.WriteLine(tr.Name + ": " + x);
            }
        }


        private static void GetValue()
        {
            //Insert(new { id = 1, name = "dc" });
            //Insert(o1: "1", o2: "dc");
            //Insert(new Dictionary<object, object>
            //           {
            //               {"id", 1}, 
            //               {"name", "dc"}
            //           }); 
            Thread thrd1 = new Thread(new ThreadStart(trmain));
            thrd1.Name = "thread1";
            thrd1.Start();
            for (int x = 0; x < 10; x++)
            {
                Thread.Sleep(900);
                Console.WriteLine("Main:" + x);
                if (5 == x) mre.Set();
            }
            while (thrd1.IsAlive)
            {
                Thread.Sleep(1000);
                Console.WriteLine("Main: waiting for thread to stop");
            }
        }


        private static void Insert(object o)
        {

        }

        private static void Insert(IDictionary<object, object> dic)
        {

        }

        private static void Insert(object o1, object o2)
        {

        }
    }
}
