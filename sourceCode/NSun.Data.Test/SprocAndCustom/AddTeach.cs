using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NSun.Data.Test.SprocAndCustom
{
    [Procedure("addteach")]
    public class AddTeach : NSun.Data.SprocEntity
    {
        [NSun.Data.InputParameter("name",System.Data.DbType.String)]
        public string Name { get; set; }

        [NSun.Data.InputParameter("pass", System.Data.DbType.String)]
        public string Pass { get; set; }
    }
}
