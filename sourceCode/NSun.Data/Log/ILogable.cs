using System;
using System.Collections.Generic;
using System.Text;

namespace NSun.Data.Log
{ 
    public delegate void LogHandler(string logMsg);
 
    public interface ILogable
    {
        event LogHandler OnLog;
    }
}
