using System;
using System.IO;
using System.Configuration;

namespace NSun.Data.Log
{
    /// <summary>
    /// .config Appstrings LogPath
    /// </summary>
    public class FileLog : IDbLog
    {
        private string filename = "Log" + DateTime.Today.ToString("yyyy-MM-dd") + ".log";

        protected string FilePath
        {
            get
            {
                string path = Path.Combine(Path.GetDirectoryName(this.GetType().Assembly.Location), filename);
                path = ConfigurationManager.AppSettings["LogPath"] ?? path;
                return path.Trim().Length > 0 ? path + filename : Path.Combine(Path.GetDirectoryName(this.GetType().Assembly.Location), filename);
            }
        }

        #region IDBLog 成员

        public void Write(Exception error)
        {
            if (!File.Exists(FilePath))
            {
                using (System.IO.StreamWriter sw = new StreamWriter(new FileStream(FilePath, FileMode.OpenOrCreate)))
                {
                    sw.WriteLine("Message:" + error.Message + "StackTrace:" + error.StackTrace + "   " + DateTime.Now);
                }
            }
            else
            {
                using (System.IO.StreamWriter sw = File.AppendText(FilePath))
                {
                    sw.WriteLine("Message:" + error.Message + "StackTrace:" + error.StackTrace + "   " + DateTime.Now);
                }
            }
        }

        public void Write(string mes)
        {
            Write(new Exception(mes));
        }

        #endregion
    }
}
