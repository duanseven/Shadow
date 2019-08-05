using System;
using System.Collections.Generic;
using System.Linq;
using System.Text; 
namespace NSunLiteTest
{ 
    [Microsoft.VisualStudio.TestTools.UnitTesting.TestClass]
    public class Class1
    {
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
        public void A()
        {
            string s = "1,2,3,4,5"; 
            
            string s1 = "1，2，3，4，5";
            string s2 = "1,2，3,4，5";
            Console.WriteLine(s.Split(',').Length);
            Console.WriteLine(s1.Split('，').Length);
            Console.WriteLine(s2.Split(",，".ToCharArray()).Length);            
        }
    }
}
