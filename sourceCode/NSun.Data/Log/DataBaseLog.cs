using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace NSun.Data.Log
{
    /// <summary>
    /// .config Appstrings LogCommand
    /// </summary>
    public class DataBaseLog : IDbLog
    {
        private DBQuery dbcon;

        public DataBaseLog(DBQuery dbcon)
        {
            Check.Require(dbcon != null);

            this.dbcon = dbcon;
        }

        protected string LogCommand
        {
            get
            {
                //@error @sqlselect
                return ConfigurationManager.AppSettings["LogCommand"];
            }
        }

        #region IDBLog 成员

        public void Write(Exception error)
        {
            IDbLog log = new FileLog();
            if (LogCommand == null)
            {
                log.Write(new Exception("没有配置相应的 LogCommand 节点"));
                return;
            }
            try
            {
                dbcon.ToExecute(dbcon.CreateCustomSql(LogCommand)
                    .AddInputParameter("@error", System.Data.DbType.String, "Message:" + error.Message + "StackTrace:" + error.StackTrace + "   " + DateTime.Now));
            }
            catch (Exception e)
            {
                log.Write(e);
            }
        }

        public void Write(string mes)
        {
            Write(new Exception(mes));
        }

        #endregion
    }
}
