using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SQL_Management.Model
{
    public class ConnectionInfo : NotificationObject
    {
        private string connectionString;

        public string ConnectionString
        {
            get { return connectionString; }
            set
            {
                connectionString = value;
                RaisePropertyChanged("ConnectionString");
            }
        }

        private NSun.Data.SqlType sqlType;

        public NSun.Data.SqlType SqlType
        {
            get { return sqlType; }
            set { sqlType = value; RaisePropertyChanged("SqlType"); }
        }

    }
}
