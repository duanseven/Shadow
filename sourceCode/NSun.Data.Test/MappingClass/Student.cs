using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NSun.Data.Test.Mapping
{
    public class Student
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int Age { get; set; }
    }

    public class User
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Pass { get; set; }
    }
}
