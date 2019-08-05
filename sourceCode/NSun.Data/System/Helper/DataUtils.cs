using System; 
using System.Data;

namespace NSun.Data.Helper
{
    public sealed class DataUtils
    {
        public static string ToString(DbType dbType, object val)
        {
            if (val == null || val == DBNull.Value)
            {
                return "NULL";
            }
            Type type = val.GetType();
            if (dbType == DbType.AnsiString || dbType == DbType.AnsiStringFixedLength)
            {
                return string.Format("'{0}'", val.ToString().Replace("'", "''"));
            }
            if (dbType == DbType.String || dbType == DbType.StringFixedLength)
            {
                return string.Format("N'{0}'", val.ToString().Replace("'", "''"));
            }
            if (type == typeof(DateTime) || type == typeof(Guid))
            {
                return string.Format("'{0}'", val);
            }
            if (type == typeof(TimeSpan))
            {
                DateTime baseTime = new DateTime(2006, 11, 23);//±Í÷µ-≤Ó÷µ
                return string.Format("(CAST('{0}' AS datetime) - CAST('{1}' AS datetime))", baseTime + ((TimeSpan)val), baseTime);
            }
            if (type == typeof(bool))
            {
                return ((bool)val) ? "1" : "0";
            }
            if (type == typeof(byte[]))
            {
                return "0x" + BitConverter.ToString((byte[])val).Replace("-", string.Empty);
            }
            if (val is ExpressionClip)
            {
                return ToString((ExpressionClip)val);
            }
            if (type.IsEnum)
            {
                return Convert.ToInt32(val).ToString();
            }
            if (type.IsValueType)
            {
                return val.ToString();
            }
            return string.Format("'{0}'", val.ToString().Replace("'", "''"));         
        }

        public static string ToString(IExpression expr)
        {
            if (expr == null)
            {
                return null;
            }

            string sql = expr.ToString();

            if (!string.IsNullOrEmpty(sql))
            {
                foreach (var childExpression in expr.ChildExpressions)
                {
                    if(childExpression is ParameterExpression)
                    {
                        var child = childExpression as ParameterExpression;
                        sql = sql.Replace('@' + child.Sql.TrimStart("@:?".ToCharArray()),
                                          ToString(child.DataType, child.Value));
                    }
                } 
            }
            return sql.Replace("= NULL", "IS NULL");
        }

        public static string ToString(System.Data.Common.DbCommand cmd)
        {
            if (cmd == null)
            {
                return null;
            } 
            string sql = cmd.CommandText; 
            if (!string.IsNullOrEmpty(sql))
            {
                System.Collections.IEnumerator en = cmd.Parameters.GetEnumerator(); 
                while (en.MoveNext())
                {
                    var p = (System.Data.Common.DbParameter)en.Current;
                    sql = sql.Replace(p.ParameterName, ToString(p.DbType, p.Value));
                }  
                return sql.Replace("= NULL", "IS NULL");
            }
            return null;
        }

        public static Guid ToGuid(object obj)
        {
            if (obj != null && obj != DBNull.Value)
            {
                if (obj is Guid)
                    return (Guid)obj;
                if (obj is string)
                    return new Guid(obj as string);
                if (obj is byte[])
                    return new Guid(obj as byte[]);
            }

            return default(Guid);
        }

        public static DbType GetDbType(object value)
        {
            if (value == null)
            {
                return DbType.String;
            }
            if (value is int)
            {
                return DbType.Int32;
            }
            return DbType.String;
        }
    }
}
