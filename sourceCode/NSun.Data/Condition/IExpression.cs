using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace NSun.Data
{
    public interface IExpression : ICloneable
    {
        string Sql { get; }

        ICollection<IExpression> ChildExpressions { get; }
    }

    public interface IParameterExpression : IExpression
    {
        string ID { get; }

        object Value { get; set; }

        bool? IsUnicode { get; set; }

        System.Data.DbType DataType { get; set; }

        int? Size { get; set; }

        SprocParameterDirection? Direction { get; }
    }
}
