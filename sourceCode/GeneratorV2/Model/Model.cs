using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeneratorV2.Model
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
        public NSun.Data.SqlType SQLType { get; set; }
    }

    public class FileInfo : NotificationObject
    {
        private string outPutPath;

        public string OutPutPath
        {
            get { return outPutPath; }
            set
            {
                outPutPath = value;
                RaisePropertyChanged("OutPutPath");
            }
        }
        public string FileNameSuffix { get; set; }
        public string FileNameSpace { get; set; }
    }

    public class SearchInfo
    {
        public SearchInfo()
        {
            TableName = string.Empty;
            ViewName = string.Empty;
        }
        public string TableName { get; set; }
        public string ViewName { get; set; }
    }

    public class SQLTypeInfo
    {
        public string Type { get; set; }
        public NSun.Data.SqlType EnumType { get; set; }
    }

    public class TVPSelect : NotificationObject
    {
        private string tabname;
        public string Tabname
        {
            get { return tabname; }

            set
            {
                tabname = value;
                RaisePropertyChanged("Tabname");
            }
        }

        private bool ischeck;
        public bool Ischeck
        {
            get { return ischeck; }

            set { ischeck = value; RaisePropertyChanged("Ischeck"); }
        }
    }
}
