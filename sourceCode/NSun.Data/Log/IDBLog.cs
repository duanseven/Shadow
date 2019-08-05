using System;
using System.Collections.Generic;
using System.Text;

namespace NSun.Data.Log
{
    public interface IDbLog
    {
        void Write(Exception error);

        void Write(string mes);
    }
}
