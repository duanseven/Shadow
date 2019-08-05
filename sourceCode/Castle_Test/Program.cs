using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Castle.DynamicProxy;
using System.Reflection;

namespace Castle_Test
{
    public interface IInterface
    {
        int Show();
    }

    public class Target : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            if (invocation.Method.Name.StartsWith("get_"))
            {
                var c = invocation.Method.ReturnType.IsAssignableFrom(typeof(System.Collections.ICollection));
                Console.WriteLine(c);
                Console.WriteLine("得到的是Get");
            }
        }
    } 

    public class BaseModel : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }

    public class NotifyPropertyChangedInterceptor : StandardInterceptor
    {

        protected override void PreProceed(IInvocation invocation)
        {
            //操作前
            base.PreProceed(invocation);
        }

        protected override void PerformProceed(IInvocation invocation)
        {
            //执行前之后的操作==然后是赋值
            base.PerformProceed(invocation);
        }
        
        protected override void PostProceed(IInvocation invocation)
        {
            base.PostProceed(invocation);
            var methodName = invocation.Method.Name;
            // 这里可能不是很完善， 属性的 Setter 一般都是以 set_ 开头的， 
            // 应该有更好的判断方法。 
            if (methodName.StartsWith("get_"))
            {
                var propertyName = methodName.Substring(4);
                var target = invocation.Proxy as BaseModel;
                if (target != null)
                {
                    target.NotifyPropertyChanged(propertyName);
                }
            }
        }
    }

    public static class ModelHelper
    {
        private static readonly ProxyGenerator ProxyGenerator = new ProxyGenerator();

        private static readonly NotifyPropertyChangedInterceptor Interceptor = new NotifyPropertyChangedInterceptor();

        public static T CreateProxy<T>(T obj) where T : class, INotifyPropertyChanged
        {
            return ProxyGenerator.CreateClassProxyWithTarget(obj, Interceptor);
        }
    }

    public class UserModel : BaseModel
    {

        private string firstName;

        public virtual string FirstName
        {
            get
            {
                return firstName;
            }
            set { firstName = value; }
        }

        public string LastName
        {
            get;
            set;
        }
    }

    public class Mam
    {
        public virtual int Id { get; set; }
        public string Name { get; set; }
        public virtual List<string> list { get; set; }
    } 

    class Program
    {
        static void Main(string[] args)
        {
            var user = new UserModel()
                           {
                               FirstName = "dc",
                               LastName = "ada"
                           };
                 
             var dataContext = ModelHelper.CreateProxy(user);
             Console.WriteLine(
             dataContext.FirstName);
 
            //Console.WriteLine(
            //dataContext.LastName);


            //var generator = new ProxyGenerator();
            //Mam m = new Mam()
            //            {
            //                Id = 1,
            //                Name = "dacey"
            //            };
            //var m = generator.CreateClassProxy<Mam>();
            //m.Id = 1;
            //m.Name = "dacey";
            //var m1 = generator.CreateClassProxyWithTarget(m, new ProxyGenerationOptions() { }, new Target());

            //Console.WriteLine(m1.Id);
            // Console.WriteLine(m.list);
            // Console.ReadKey();

            //generator.CreateClassProxyWithTarget()
            //单个代理
            //var generator = new ProxyGenerator();
            //var interceptor = new CallingLogInterceptor();
            //var entity = generator.CreateClassProxy<SimpleSamepleEntity>(interceptor);
            //entity.Name = "Richie";
            //entity.Age = 50;
            //Console.WriteLine("The entity is: " + entity);
            //Console.WriteLine("Type of the entity: " + entity.GetType().FullName);
            //Console.ReadKey();

            //多个代理
            //ProxyGenerator generator = new ProxyGenerator();
            //SimpleSamepleEntity entity = generator.CreateClassProxy<SimpleSamepleEntity>(
            //    new SimpleLogInterceptor(),
            //    new CallingLogInterceptor());
            //entity.Name = "Richie";
            //entity.Age = 50;
            //Console.WriteLine("The entity is: " + entity);
            //Console.WriteLine("Type of the entity: " + entity.GetType().FullName);
            //Console.ReadKey();

            //分类型代理
            //ProxyGenerator generator = new ProxyGenerator();
            //var options = new ProxyGenerationOptions(new InterceptorFilter()) { Selector = new InterceptorSelector() };
            //SimpleSamepleEntity entity = generator.CreateClassProxy<SimpleSamepleEntity>(
            //    options,
            //    new SimpleLogInterceptor(), new CallingLogInterceptor());
            //entity.Name = "Richie";
            //entity.Age = 50;
            //Console.WriteLine("The entity is: " + entity);
            //Console.WriteLine("Type of the entity: " + entity.GetType().FullName);
            //Console.ReadKey();

            //空接口，只能拦截使用
            //ProxyGenerator generator = new ProxyGenerator();
            //IInterface w = generator.CreateInterfaceProxyWithoutTarget<IInterface>(new SimpleLogInterceptor());
            //w.Show();//不能用Proceed 只能拦截操作

            //代理对象替换
            //var generator = new ProxyGenerator();
            //var node = generator.CreateInterfaceProxyWithTargetInterface<IStorageNode>(
            //    new StorageNode("master")
            //    , new DualNodeInterceptor(new StorageNode("slave"))
            //    , new CallingLogInterceptor());
            //node.Save("my message"); //应该调用master对象
            //node.IsDead = true;
            //node.Save("my message"); //应该调用master对象
            //node.Save("my message"); //应该调用slave对象
            //Console.ReadKey();

            //类功能合并  多继承差不多
            //ProxyGenerator generator = new ProxyGenerator();
            //var options = new ProxyGenerationOptions();
            //options.AddMixinInstance(new ClassA());
            //ClassB objB = generator.CreateClassProxy<ClassB>(options, new CallingLogInterceptor());
            //objB.ActionB();
            //InterfaceA objA = objB as InterfaceA;
            //objA.ActionA();
            //Console.ReadKey();

            //var scope = new ModuleScope(true, true, ModuleScope.DEFAULT_ASSEMBLY_NAME, ModuleScope.DEFAULT_FILE_NAME,
            //                            "DynamicProxyTest.Proxies", "DynamicProxyTest.Proxies.dll");

            //var builder = new DefaultProxyBuilder(scope);
            //var generator = new ProxyGenerator(builder);
            //var options = new ProxyGenerationOptions(new InterceptorFilter())
            //{
            //    Selector = new InterceptorSelector()
            //};
            //SimpleSamepleEntity entity = generator.CreateClassProxy<SimpleSamepleEntity>(
            //    options,
            //    new SimpleLogInterceptor(), new CallingLogInterceptor());
            //IStorageNode node = generator.CreateInterfaceProxyWithTargetInterface<IStorageNode>(
            //    new StorageNode("master")
            //    , new DualNodeInterceptor(new StorageNode("slave"))
            //    , new CallingLogInterceptor());

            //options = new ProxyGenerationOptions();
            //options.AddMixinInstance(new ClassA());
            //ClassB objB = generator.CreateClassProxy<ClassB>(options, new CallingLogInterceptor());
            //scope.SaveAssembly(false);

            //var scope = new ModuleScope();
            //Assembly assembly = Assembly.Load("DynamicProxyTest.Proxies");
            //scope.LoadAssemblyIntoCache(assembly);

            //Castle.Proxies.ClassBProxy cp = new Castle.Proxies.ClassBProxy(new ClassA(), new IInterceptor[] { new CallingLogInterceptor() }); 
            //cp.ActionA();
            //cp.ActionB();
            //Console.ReadKey();

            //Castle.Proxies.IStorageNodeProxy s =
            //    new Castle.Proxies.IStorageNodeProxy(
            //        new IInterceptor[]
            //            {
            //                new DualNodeInterceptor(new StorageNode("slave"), new CallingLogInterceptor()) 
            //            },
            //        new StorageNode("master"));
            //s.Save("nihao");
            //s.IsDead = true;
            //s.Save("buhao");
            //s.Save("my message");
            //Console.ReadKey();

            //Castle.Proxies.SimpleSamepleEntityProxy s1 = new Castle.Proxies.SimpleSamepleEntityProxy(
            //    new IInterceptor[]
            //        {
            //             new SimpleLogInterceptor(), new CallingLogInterceptor()
            //        }
            //    , new InterceptorSelector());
            //s1.Name = "dsf";
            //Console.WriteLine(s1.Name);
            //Console.ReadKey();
        }
    }
}
