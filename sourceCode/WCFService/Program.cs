using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace WCFService
{
    class Program
    {
        static void Main(string[] args)
        {
            using (ServiceHost host = new ServiceHost(typeof(StudentService.StudentService)))
            {

                if (host.State != CommunicationState.Opening)
                    host.Open();
                //显示运行状态
                Console.WriteLine("Host is runing! and state is {0}", host.State);
                //等待输入即停止服务
                Console.Read();
            }
        }
    }
}
