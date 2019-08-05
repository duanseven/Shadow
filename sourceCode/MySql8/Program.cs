using MySql8.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MySql8
{
    class Program
    {
        static void Main(string[] args)
        {
            var db = DBFactory.CreateDBQuery<Kkpass>();
            var select = db.CreateQuery().SetMaxResults(10);
            var list=db.ToList<Kkpass>(select);
            Console.WriteLine(select.ToDbCommandText());
            //Console.WriteLine(list.Count());
            //foreach (var item in list)
            //{
            //    Console.WriteLine(item.PASSID);
            //}

        }
    }
}
