using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NSun.Data;

namespace ConsoleApplication1
{
    public interface IBase
    {
        bool IsInitialize { get; set; }
    }

    public class TestClass : BaseEntity
    {
        public static int Age;
        private static string Name;
    }

    public class Base : IBase
    {

        private bool isInitialize;
        #region IBase 成员

        public bool IsInitialize
        {
            get { return isInitialize; }
            set
            {
                isInitialize = value;
            }
        }

        #endregion

        public bool S { get; set; }
    }
    public abstract class MyClassA
    {
        public abstract int m();
    }

    public abstract class MyClassB : MyClassA
    {
    } 



}
