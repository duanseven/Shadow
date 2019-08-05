using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text; 
using Castle.DynamicProxy;

namespace Castle_Test
{

    public interface IStorageNode
    {
        bool IsDead { get; set; }
        void Save(string message);
    }

    [Serializable]
    public class StorageNode : IStorageNode
    {
        private string _name;
        public StorageNode(string name)
        {
            this._name = name;
        }
        public bool IsDead { get; set; }
        public void Save(string message)
        {
            Console.WriteLine(string.Format("\"{0}\" was saved to {1}", message, this._name));
        }
    }

    [Serializable]
    public class DualNodeInterceptor : IInterceptor
    {
        private IStorageNode _slave;
        public DualNodeInterceptor(IStorageNode slave, CallingLogInterceptor callingLogInterceptor)
        {
            this._slave = slave;
        }
        public void Intercept(IInvocation invocation)
        {
            IStorageNode master = invocation.InvocationTarget as IStorageNode;
            if (master.IsDead)
            {
                IChangeProxyTarget cpt = invocation as IChangeProxyTarget;
                //将被代理对象master更换为slave
                cpt.ChangeInvocationTarget(this._slave);
                //测试中恢复master的状态，以便看到随后的调用仍然使用master这一效果
                master.IsDead = false;
            }
            invocation.Proceed();
        }
    }

    [Serializable]
    public class SimpleSamepleEntity
    {
        private string name;
        public virtual string Name { get
        {
            return name;
        } set
        {
            name = value;
        } }
        public virtual int Age { get; set; }
        public override string ToString()
        {
            return string.Format("{{ Name: \"{0}\", Age: {1} }}", this.Name, this.Age);
        }
    }
    
    [Serializable]
    public class SimpleLogInterceptor : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            Console.WriteLine(">>" + invocation.Method.Name);
            invocation.Proceed();
        }
    }
    [Serializable]
    public class CallingLogInterceptor : IInterceptor
    {
        private int _indent = 0;
        private void PreProceed(IInvocation invocation)
        {
            if (this._indent > 0)
                Console.Write(" ".PadRight(this._indent * 4, ' '));
            this._indent++;
            Console.Write("Intercepting: " + invocation.Method.Name + "(");
            if (invocation.Arguments != null && invocation.Arguments.Length > 0)
                for (int i = 0; i < invocation.Arguments.Length; i++)
                {
                    if (i != 0) Console.Write(", ");
                    Console.Write(invocation.Arguments[i] == null
                        ? "null"
                        : invocation.Arguments[i].GetType() == typeof(string)
                           ? "\"" + invocation.Arguments[i].ToString() + "\""
                           : invocation.Arguments[i].ToString());
                }
            Console.WriteLine(")");
        }
        private void PostProceed(IInvocation invocation)
        {
            this._indent--;
        }
        public void Intercept(IInvocation invocation)
        {
            this.PreProceed(invocation);
            invocation.Proceed();
            this.PostProceed(invocation);
        }
    }


    [Serializable]
    public class InterceptorSelector : IInterceptorSelector
    {
        public IInterceptor[] SelectInterceptors(Type type, MethodInfo method, IInterceptor[] interceptors)
         {
            if (method.Name.StartsWith("set_")) 
                return interceptors;
            else 
                return interceptors.Where(i => i is CallingLogInterceptor).ToArray<IInterceptor>();
        }
    }


    [Serializable]
    public class InterceptorFilter : IProxyGenerationHook
    {
        public bool ShouldInterceptMethod(Type type, MethodInfo memberInfo)
        {
            return memberInfo.IsSpecialName &&
                (memberInfo.Name.StartsWith("set_") || memberInfo.Name.StartsWith("get_"));
        }

        public void MethodsInspected()
        {
        }

        #region IProxyGenerationHook 成员


        public void NonProxyableMemberNotification(Type type, MemberInfo memberInfo)
        {

        }

        #endregion
    }

    public interface InterfaceA
    {
        void ActionA();
    }

    [Serializable]
    public class ClassA : InterfaceA
    {
        public void ActionA()
        {
            Console.WriteLine("I'm from ClassA");
        }
    }

    [Serializable]
    public class ClassB
    {
        public virtual void ActionB()
        {
            Console.WriteLine("I'm from ClassB");
        }
    }
     

}
