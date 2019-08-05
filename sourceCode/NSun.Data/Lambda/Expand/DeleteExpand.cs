using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;

namespace NSun.Data
{
    public static class DeleteExpand
    {
        public static int Execute(this DeleteSqlSection section)
        {
            return Execute(section, null);
        }

        public static int Execute(this DeleteSqlSection section, DbTransaction tran)
        {
            DBQuery db = new DBQuery(section.Db);
            if (tran != null)
            {
                section.SetTransaction(tran);
            }            
            return db.ToExecute(section);
        }

        public static int Execute<T>(this DeleteSqlSection<T> section) where T :class, IBaseEntity
        {
            return Execute<T>(section, null);
        }

        public static int Execute<T>(this DeleteSqlSection<T> section, DbTransaction tran) where T :class, IBaseEntity
        {            
            DBQuery<T> db = new DBQuery<T>(section.Db);            
            return db.DeleteKey(section, tran);
        }
    }
}
