using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Runtime.Remoting.Proxies;

namespace ELTest
{
    class Program
    {
        static void Main(string[] args)
        {
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();            
            sw.Start();
            for (int i = 0; i < 1000000; i++)
            {
               var users= PolicyInjectionFactory.Create<Users>(new Users()
                                                         {
                                                             Id = i,
                                                             Name = "dc" + i,
                                                             Pass = "ada" + i
                                                         });
            } 
            sw.Stop();
            Console.WriteLine(sw.Elapsed);

            sw.Start();
            for (int i = 0; i < 1000000; i++)
            {
                var user = PolicyInjectionFactory.CreateCastle<User>(new User()
                {
                    Id = i,
                    Name = "dc" + i,
                    Pass = "ada" + i
                }); 
            }
            sw.Stop();
            Console.WriteLine(sw.Elapsed); 
        }
    }


    public class InterceptorUser : Castle.DynamicProxy.IInterceptor
    {
        #region IInterceptor 成员

        public void Intercept(Castle.DynamicProxy.IInvocation invocation)
        {
            invocation.Proceed();
        }

        #endregion
    }


    public class Users : MarshalByRefObject
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Pass { get; set; }
    }

    public class User
    {
        public virtual int Id { get; set; }
        public virtual string Name { get; set; }
        public virtual string Pass { get; set; }
    }

    public static class PolicyInjectionFactory
    {
        static Castle.DynamicProxy.ProxyGenerator pg = new Castle.DynamicProxy.ProxyGenerator();

        public static T CreateCastle<T>(T instance) where T : class
        {
            return pg.CreateClassProxyWithTarget(instance, new InterceptorUser());
        }

        public static T Create<T>(T instance)
        {
            var realProxy = new RealProxyEx<T>(instance);
            T transparentProxy = (T)realProxy.GetTransparentProxy();
            return transparentProxy;
        }
    }

    public class RealProxyEx<T> : RealProxy
    {
        private T _target;

        public RealProxyEx(T target)
            : base(typeof(T))
        {

            this._target = target;

        }

        public override System.Runtime.Remoting.Messaging.IMessage Invoke(System.Runtime.Remoting.Messaging.IMessage msg)
        {
            //Invoke injected pre-operation.

            //Console.WriteLine("The injected pre-operation is invoked");

            //Invoke the real target instance.

            IMethodCallMessage callMessage = (IMethodCallMessage)msg;

            object returnValue = callMessage.MethodBase.Invoke(this._target, callMessage.Args);

            //Invoke the injected post-operation.

            //Console.WriteLine("The injected post-peration is executed");

            //Return

            return new ReturnMessage(returnValue, new object[0], 0, null, callMessage);
            //return null;
        }
    }

}
