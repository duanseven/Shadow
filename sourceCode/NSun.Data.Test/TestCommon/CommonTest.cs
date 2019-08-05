using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data;
using NSun.Data.Test.Mapping;

namespace NSun.Data.Test.TestCommon
{
    public partial class Users
    {
        private int id;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public virtual int Id
        {
            get { return id; }
            set { id = value; }
        }

        private string name;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public virtual string Name
        {
            get { return name; }
            set { name = value; }
        }

        private string pass;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public virtual string Pass
        {
            get { return pass; }
            set { pass = value; }
        }

        private System.Nullable<System.DateTime> logintime;
        [System.Runtime.Serialization.DataMemberAttribute()]
        public virtual System.Nullable<System.DateTime> Logintime
        {
            get { return logintime; }
            set { logintime = value; }
        } 
    }
    /// <summary>
    /// CommonTest 的摘要说明
    /// </summary>
    [TestClass]
    public class CommonTest
    {
        [TestMethod]
        public void TestDesignByContract()
        {            
            string s = null;
            //s = "dc;";            
            Check.Require(s != null);
            Check.Ensure("" != s);
            Check.Assert(s != string.Empty);
            Console.WriteLine(s);
        }

        [TestMethod]
        public void Mapper()
        {
            NSun.Data.Mapper.MapperFactory mf = new Mapper.MapperFactory();
            NSun.Data.Mapper.MapperBuilder<Student,User> mb= mf.ConfigureMapper<Student, User>(Data.Mapper.MappingType.Property);
            mb.From(p => p).To((u, s) =>
                                   {
                                       u.Id = s.Id;
                                       u.Name = s.Name;
                                       u.Pass = "123456";
                                       return u;
                                   });

            var mapping= mf.GetMapper<Student, User>();

            int count = 1000;
            List<Student> stus = new List<Student>();
            for (int i = 0; i < count
; i++)
            {
                stus.Add(new Student()
                             {
                                 Id = i + 1,
                                 Name = "DACEY" + i,
                                 Age = i + 1
                             });
            }

            System.Diagnostics.Stopwatch stop = new System.Diagnostics.Stopwatch();
            stop.Start();
            foreach (var student in stus)
            {
                User user = mapping(student);
                Console.WriteLine(user.Id);
            }
            stop.Stop();
            Console.WriteLine(stop.Elapsed);

            //Console.WriteLine(string.Format("id={0},name={1},pass={2}", user.Id, user.Name, user.Pass));
        }

        [TestMethod]
        public void Mapper2()
        {            
            //var db = DBFactory.CreateDBQuery<UsersEntity>();
            //IDataReader dr = db.ToDataReader();
            //while (dr.Read())
            //{
            //    Console.WriteLine(dr.GetValue(0));
            //}
            //dr.Close();
            //NSun.Data.Mapper.MapperBuilder<IDataReader, UsersEntity> mb =
            //    mf.ConfigureMapper<IDataReader, UsersEntity>(Data.Mapper.MappingType.All);
            //mb.From(p => p).To(
            //    (us, dr1) =>
            //    {
            //        dr1.Read();
            //        us.Id = dr1.GetInt32(dr1.GetOrdinal("id"));
            //        us.Name = dr1.GetString(dr1.GetOrdinal("name"));
            //        us.Pass = dr1.GetString(dr1.GetOrdinal("pass"));
            //        us.Logintime = dr1.GetDateTime(dr1.GetOrdinal("logintime"));
            //        return us;
            //    }
            //    );   
        }

        [TestMethod]
        public void MyTestMethod()
        {
            var ps= ReflectionUtils.DeepGetProperties(typeof (User));
            foreach (var propertyInfo in ps)
            {
                Console.WriteLine(propertyInfo.Name);
            }
        }
    }
}

