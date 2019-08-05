using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Text;

namespace RemotingSerivce
{
    class Program
    {
        static void Main(string[] args)
        {
            RemotingConfiguration.Configure("RemotingSerivce.exe.config");
            Console.WriteLine("begin service.................");
            Console.ReadKey();
        }
    }
}
