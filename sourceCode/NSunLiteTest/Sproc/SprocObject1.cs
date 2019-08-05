using System; 
using NSun.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data;

namespace NSunLiteTest.Sproc
{
    [Procedure("addusers")]
    public class SprocObject1 : SprocEntity
    {
        //args : Return Output InputOutput 
        [InputParameter("name", System.Data.DbType.String)]
        public string Name { get; set; }

        [InputParameter("pass", System.Data.DbType.String)]
        public string Pass { get; set; }


        private int rETURN_VALUE;

        [NSun.Data.ReturnParameterAttribute("RETURN_VALUE", System.Data.DbType.Int32, 0)]
        public int RETURN_VALUE
        {
            get
            {
                return rETURN_VALUE;
            }
            set
            {
                rETURN_VALUE = value;
            }
        }

    }

    [Procedure("getusers")]
    public class SprocObject2 : SprocEntity
    {
        //args : Return Output InputOutput 
        [OutputParameter("name", System.Data.DbType.String, 50)]
        public string Name { get; set; }

        [InputParameter("id", System.Data.DbType.Int32)]
        public int Id { get; set; }

    }

    [Procedure("getusers2")]
    public class SprocObject3 : SprocEntity
    {
        //args : Return Output InputOutput 
        [InputOutputParameter("id", System.Data.DbType.Int32, 4)]
        public int Id { get; set; }

    }

    [Procedure("getusers3")]
    public class SprocObject4 : SprocEntity
    {
        //args : Return Output InputOutput 
        [InputParameter("id", System.Data.DbType.Int32)]
        public int Id { get; set; }

        [ReturnParameter("returnvalue", System.Data.DbType.Int32, 4)]
        public int Ret { get; set; }
    }


    [Procedure("scalarobj")]
    public class Scalarobj1 : SprocEntity
    {
        [InputParameter("id", System.Data.DbType.Int32)]
        public int Id { get; set; }

        [OutputParameter("name", System.Data.DbType.String, 50)]
        public string Name { get; set; }
    }

    [TestClass]
    public class SprocObjectTest
    {

        private DBQuery query;
        public SprocObjectTest()
        {
            query = DBFactoryNew.CreateDBQuery();//new DBQuery(new Database("db", SqlType.Sqlserver9));
        }

       [TestMethod]
      public void MyTestMethod()
       {
           AddusersSproc au = new AddusersSproc();
           au.Pass = "++fewfew";
           au.Name = "++wudi++";
           query.ToProcedureExecute(au);
       }

        [TestMethod]
        public void TestMethod1()
        {
            SprocObject1 sp = new SprocObject1();
            sp.Name = "adawudi123";
            sp.Pass = "hello world123";
            Console.WriteLine(sp.ToDbCommandText());
            query.ToProcedureExecute(sp);
            Console.WriteLine("____________1 d");
            //SprocObject2 s2 = new SprocObject2();
            //s2.Id = 2;
            //query.ToProcedureExecute(s2);
            //Console.WriteLine(s2.Name);
            //Console.WriteLine("____________2 d");
            //SprocObject3 p3 = new SprocObject3();
            //p3.Id = 2;
            //query.ToProcedureExecute(p3);
            //Console.WriteLine(p3.Id);
            //Console.WriteLine("____________3 d");
            //SprocObject4 s4 = new SprocObject4();
            //s4.Id = 2;
            //query.ToProcedureExecute(s4);
            //Console.WriteLine(s4.Ret);
            //Console.WriteLine("____________4 d");
            ////Scalarobj1 sj1 = new Scalarobj1();
            //Scalarobj1 sj1 = query.CreateSproc<Scalarobj1>();
            //sj1.Id = 2;
            //var w = query.ToProcedureScalar(sj1);
            //Console.WriteLine(sj1.Name);
            //Console.WriteLine(w);
            //Console.WriteLine("____________5 d");

            var select= query.CreateCustomSql(" select top 30 * from users order by id desc");
            var dt = query.ToDataSet(select).Tables[0];
            foreach (DataRow row in dt.Rows)
            {
                Console.WriteLine(row["name"]);
            }
        } 
    
    }
}
