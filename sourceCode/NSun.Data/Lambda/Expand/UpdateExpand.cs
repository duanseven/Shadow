using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;

namespace NSun.Data
{
    public static class UpdateExpand
    {
        public static int Execute(this UpdateSqlSection section)
        {
            return Execute(section, null);
        }

        public static int Execute(this UpdateSqlSection section, DbTransaction tran)
        {
            DBQuery db = new DBQuery(section.Db);
            return db.Update(section, tran);
        }

        public static int Execute<T>(this UpdateSqlSection<T> section) where T : class, IBaseEntity
        {
            return Execute<T>(section, null);
        }

        public static int Execute<T>(this UpdateSqlSection<T> section, DbTransaction tran) where T :class, IBaseEntity
        {
            DBQuery<T> db = new DBQuery<T>(section.Db);
            return db.Update(section, tran);
        }
    }
}
