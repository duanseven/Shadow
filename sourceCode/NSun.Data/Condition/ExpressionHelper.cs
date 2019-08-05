using System;
using System.IO;
using System.Reflection; 
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using NSun.Data.Helper;

namespace NSun.Data
{
    public static class ExpressionHelper
    {
        #region Public Methods

        internal static string ToSelectColumnName(this ExpressionClip column)
        {
            return column is IColumn ? ToSelectColumnName((column as IColumn)) : column.Sql;
        }
 
        internal static string ToSelectColumnName(this IColumn column)
        {
            var sql = column.ToExpressionCacheableSql();
            var columnName = column.ColumnName.ToDatabaseObjectName();

            if (sql == columnName)
                return sql;
            if (columnName.Contains("].["))
            {
                columnName = columnName.Split('.')[1];
            }
            return sql + " AS " + columnName;
        }

        internal static string ToDatabaseObjectName(this ExpressionClip column)
        {
            return column is IColumn
                       ? (column as IColumn).ColumnName.ToDatabaseObjectName()
                       : column.Sql.ToDatabaseObjectName();
        }

        internal static string ToDatabaseObjectName(this string name)
        {
            if (name.Contains(")"))
                return name;
            return "[" + name.TrimStart('[').TrimEnd(']').Replace(".", "].[") + "]";
        }

        internal static IExpression CreateParameterExpression(object value)
        {
            if (value == null || value == DBNull.Value)
                return NullExpression.Value;
            
            switch (Type.GetTypeCode(value.GetType()))
            {
                case TypeCode.Boolean:
                    return new ParameterExpression(value,System.Data.DbType.Boolean);
                case TypeCode.Byte:
                case TypeCode.SByte:
                    return new ParameterExpression(value, System.Data.DbType.Byte);
                case TypeCode.Char:
                case TypeCode.String:
                    return new ParameterExpression(value, System.Data.DbType.String);
                case TypeCode.DateTime:
                    return new ParameterExpression(value, System.Data.DbType.DateTime);
                case TypeCode.Decimal:
                    return new ParameterExpression(value, System.Data.DbType.Decimal);
                case TypeCode.Single:
                case TypeCode.Double:
                    return new ParameterExpression(value, System.Data.DbType.Double);
                case TypeCode.UInt16:
                case TypeCode.Int16:
                    return new ParameterExpression(value, System.Data.DbType.Int16);
                case TypeCode.UInt32:
                case TypeCode.Int32:
                    return new ParameterExpression(value, System.Data.DbType.Int32);
                case TypeCode.UInt64:
                case TypeCode.Int64:
                    return new ParameterExpression(value, System.Data.DbType.Int64);
            }
            return new ParameterExpression(value, System.Data.DbType.String);
        }

        internal static string Serialize<T>(T value)
        {
            if (ReferenceEquals(value, default(T)))
                throw new ArgumentNullException("value");
            var ms = new MemoryStream();
            string xml;
            try
            { 
                var dcs = new XmlSerializer(typeof(T));
                using (var xmlTextWriter = new XmlTextWriter(ms, Encoding.Default))
                {
                    xmlTextWriter.Formatting = Formatting.None;
                    dcs.Serialize(xmlTextWriter, value);
                    xmlTextWriter.Flush();
                    ms = (MemoryStream)xmlTextWriter.BaseStream;
                    ms.Flush();
                    xml = Encoding.UTF8.GetString(ms.ToArray());
                }
            }
            finally
            {
                if (ms != null)
                {
                    ms.Close();
                }
            }
            return xml;
        }

        internal static T Deserialize<T>(string xml)
        {
            if (string.IsNullOrEmpty(xml))
                throw new ArgumentNullException("xml");
            var xs = new XmlSerializer(typeof(T));
            var sr = new StringReader(xml);
            object obj = xs.Deserialize(sr);
            return (T)obj;
        }

        internal static string ToString(ExpressionOperator op)
        {
            switch (op)
            {
                case ExpressionOperator.Add:
                    return "+";
                case ExpressionOperator.Subtract:
                    return "-";
                case ExpressionOperator.Multiply:
                    return "*";
                case ExpressionOperator.Divide:
                    return "/";
                case ExpressionOperator.Mod:
                    return "%";
                case ExpressionOperator.BitwiseAnd:
                    return "&";
                case ExpressionOperator.BitwiseOr:
                    return "|";
                case ExpressionOperator.BitwiseXor:
                    return "^";
                case ExpressionOperator.BitwiseNot:
                    return "~";
                case ExpressionOperator.Link:
                    return "||";
                    //logic operators please call ToConditionCacheableSql()
            }

            return string.Empty;
        }

        internal static void GetLeftRightOperatorsForBetween(bool includeLeft, bool includeRight
            , out ExpressionOperator leftOp, out ExpressionOperator rightOp)
        {
            leftOp = includeLeft ? ExpressionOperator.GreaterThanOrEquals : ExpressionOperator.GreaterThan;
            rightOp = includeRight ? ExpressionOperator.LessThanOrEquals : ExpressionOperator.LessThan;
        }

        internal static T DefaultValue<T>()
        {
            return default(T);
        }

        internal static object DefaultValue(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");
            return typeof(ExpressionHelper).GetMethod("DefaultValue", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public, null, Type.EmptyTypes, null).MakeGenericMethod(type).Invoke(null, null);
        }
        
        #endregion

        #region Non-Public Methods

        private static string ToCacheableSql(IExpression leftExpression, ExpressionOperator op, IExpression rightExpression)
        {
            if (ReferenceEquals(leftExpression, null) && op != ExpressionOperator.Exists)
                throw new ArgumentNullException("leftExpression");
            if (op != ExpressionOperator.None)
            {
                if (ReferenceEquals(rightExpression, null))
                    throw new ArgumentNullException("rightExpression");
            }

            switch (op)
            {
                case ExpressionOperator.Like:
                    return leftExpression.ToExpressionCacheableSql() + " LIKE " + rightExpression.ToExpressionCacheableSql();                
                case ExpressionOperator.Equals:
                    return leftExpression.ToExpressionCacheableSql() + " = " + rightExpression.ToExpressionCacheableSql();
                case ExpressionOperator.NotEquals:
                    return leftExpression.ToExpressionCacheableSql() + " <> " + rightExpression.ToExpressionCacheableSql();
                case ExpressionOperator.GreaterThan:
                    return leftExpression.ToExpressionCacheableSql() + " > " + rightExpression.ToExpressionCacheableSql();
                case ExpressionOperator.GreaterThanOrEquals:
                    return leftExpression.ToExpressionCacheableSql() + " >= " + rightExpression.ToExpressionCacheableSql();
                case ExpressionOperator.In:
                    return leftExpression.ToExpressionCacheableSql() + " IN " + rightExpression.ToExpressionCacheableSql();
                case ExpressionOperator.LessThan:
                    return leftExpression.ToExpressionCacheableSql() + " < " + rightExpression.ToExpressionCacheableSql();
                case ExpressionOperator.LessThanOrEquals:
                    return leftExpression.ToExpressionCacheableSql() + " <= " + rightExpression.ToExpressionCacheableSql();
                case ExpressionOperator.Is:
                    return leftExpression.ToExpressionCacheableSql() + " IS " + rightExpression.ToExpressionCacheableSql();
                case ExpressionOperator.IsNot:
                    return leftExpression.ToExpressionCacheableSql() + " IS NOT " + rightExpression.ToExpressionCacheableSql();
                case ExpressionOperator.Exists:
                    return " EXISTS " + rightExpression.ToExpressionCommonSql();
                case ExpressionOperator.None:
                    return leftExpression.ToExpressionCacheableSql();
                case ExpressionOperator.Escape:
                    return " ESCAPE " + rightExpression.ToExpressionCommonSql();                
                    //arthmetric & bitwise operators please call ToString()
            }

            return string.Empty;
        }


        internal static string ToExpressionCacheableSql(this IExpression expr)
        {
            if (ReferenceEquals(expr, null))
                throw new ArgumentNullException("expr");

            if (expr.ChildExpressions == null || expr.ChildExpressions.Count == 0)
                return expr.Sql;

            var splittedSql = expr.Sql.Split('?');
            var sb = new StringBuilder();
            sb.Append(splittedSql[0]);

            var en = expr.ChildExpressions.GetEnumerator();
            for (int i = 1; i < splittedSql.Length; ++i)
            {
                en.MoveNext(); //en.Current === expr.ChildExpressions[i - 1]
                sb.Append(en.Current.ToExpressionCacheableSql());
                sb.Append(splittedSql[i]);
            }

            return sb.ToString();
        }

        internal static string ToConditionCacheableSql(this Condition condition)
        {
            if (ReferenceEquals(condition, null))
                throw new ArgumentNullException("condition");
            var sb = new StringBuilder();
            if (condition.LinkedConditions.Count > 0 || condition.IsNot)
            {
                if (condition.IsNot)
                {
                    sb.Append("NOT ");
                }
                sb.Append("(");
            }
            sb.Append(ToCacheableSql(condition.LeftExpression, condition.Operator, condition.RightExpression));

            var en = condition.LinkedConditions.GetEnumerator();
            var enAndOr = condition.LinkedConditionAndOrs.GetEnumerator();
            while (en.MoveNext() && enAndOr.MoveNext())
            {
                //string link = enAndOr.Current == ((int)ConditionAndOr.And) ? " AND " : " OR ";
                switch (enAndOr.Current)
                {
                    case ConditionAndOr.And:
                        sb.Append(" AND ");
                        break;
                    case ConditionAndOr.Or:
                        sb.Append(" OR ");
                        break;
                    case ConditionAndOr.Space:
                        sb.Append(" ");
                        break; 
                }
                //sb.Append(enAndOr.Current == ((int)ConditionAndOr.And) ? " AND " : " OR ");
                sb.Append(en.Current.ToConditionCacheableSql());
            }

            if (condition.LinkedConditions.Count > 0 || condition.IsNot)
            {
                sb.Append(")");
            }

            return sb.ToString();
        }

        #endregion

        #region ToCommonSql

        internal static string ToExpressionCommonSql(this IExpression expr)
        {
            if (ReferenceEquals(expr, null))
                throw new ArgumentNullException("expr");

            if (expr.ChildExpressions == null || expr.ChildExpressions.Count == 0)
            {
                if (expr is ParameterExpression)
                    return DataUtils.ToString(((ParameterExpression)expr).DataType,
                                                         ((ParameterExpression)expr).Value);
                return expr.Sql;
            }
            if (expr is ExpressionCollection)
            {
                var c = expr as ExpressionCollection;
                if (c.ChildExpressions.Count > 0)
                {
                    var sblist = new StringBuilder("(");
                    foreach (var variable in c)
                    {
                        if (variable is ParameterExpression)
                            sblist.AppendFormat("{0},", DataUtils.ToString(((ParameterExpression)variable).DataType,
                                                                 ((ParameterExpression)variable).Value));
                        else
                            sblist.AppendFormat("{0},", variable.Sql);
                    }
                    sblist = new StringBuilder(sblist.ToString().TrimEnd(','));
                    sblist.Append(")");
                    return sblist.ToString();
                }
            }
            if (expr is ExpressionClip)
            {

            }

            var splittedSql = expr.Sql.Split('?');
            var sb = new StringBuilder();
            sb.Append(splittedSql[0]);

            var en = expr.ChildExpressions.GetEnumerator();
            for (int i = 1; i < splittedSql.Length; ++i)
            {
                en.MoveNext(); //en.Current === expr.ChildExpressions[i - 1]
                sb.Append(en.Current.ToExpressionCacheableSql());
                sb.Append(splittedSql[i]);
            }
            return sb.ToString();
        }

        internal static string ToConditionCommonSql(this Condition condition)
        {
            if (ReferenceEquals(condition, null))
                throw new ArgumentNullException("condition");
            var sb = new StringBuilder();
            if (condition.LinkedConditions.Count > 0 || condition.IsNot)
            {
                if (condition.IsNot)
                {
                    sb.Append("NOT ");
                }
                sb.Append("(");
            }
            sb.Append(ToCommonSql(condition.LeftExpression, condition.Operator, condition.RightExpression));

            var en = condition.LinkedConditions.GetEnumerator();
            var enAndOr = condition.LinkedConditionAndOrs.GetEnumerator();
            while (en.MoveNext() && enAndOr.MoveNext())
            {
                //sb.Append(enAndOr.Current == ((int)ConditionAndOr.And) ? " AND " : " OR ");
                switch (enAndOr.Current)
                {
                    case ConditionAndOr.And:
                        sb.Append(" AND ");
                        break;
                    case ConditionAndOr.Or:
                        sb.Append(" OR ");
                        break;
                    case ConditionAndOr.Space:
                        sb.Append("  ");
                        break;
                }
                sb.Append(en.Current.ToConditionCommonSql());
            }

            if (condition.LinkedConditions.Count > 0 || condition.IsNot)
            {
                sb.Append(")");
            }

            return sb.ToString();
        }

        private static string ToCommonSql(IExpression leftExpression, ExpressionOperator op, IExpression rightExpression)
        {
            if (ReferenceEquals(leftExpression, null) && op != ExpressionOperator.Exists)
                throw new ArgumentNullException("leftExpression");
            if (op != ExpressionOperator.None)
            {
                if (ReferenceEquals(rightExpression, null))
                    throw new ArgumentNullException("rightExpression");
            }

            switch (op)
            {
                case ExpressionOperator.Like:
                    return leftExpression.ToExpressionCommonSql() + " LIKE " + rightExpression.ToExpressionCommonSql();
                case ExpressionOperator.Equals:
                    return leftExpression.ToExpressionCommonSql() + " = " + rightExpression.ToExpressionCommonSql();
                case ExpressionOperator.NotEquals:
                    return leftExpression.ToExpressionCommonSql() + " <> " + rightExpression.ToExpressionCommonSql();
                case ExpressionOperator.GreaterThan:
                    return leftExpression.ToExpressionCommonSql() + " > " + rightExpression.ToExpressionCommonSql();
                case ExpressionOperator.GreaterThanOrEquals:
                    return leftExpression.ToExpressionCommonSql() + " >= " + rightExpression.ToExpressionCommonSql();
                case ExpressionOperator.In:
                    return leftExpression.ToExpressionCommonSql() + " IN " + rightExpression.ToExpressionCommonSql();
                case ExpressionOperator.LessThan:
                    return leftExpression.ToExpressionCommonSql() + " < " + rightExpression.ToExpressionCommonSql();
                case ExpressionOperator.LessThanOrEquals:
                    return leftExpression.ToExpressionCommonSql() + " <= " + rightExpression.ToExpressionCommonSql();
                case ExpressionOperator.Is:
                    return leftExpression.ToExpressionCommonSql() + " IS " + rightExpression.ToExpressionCommonSql();
                case ExpressionOperator.IsNot:
                    return leftExpression.ToExpressionCommonSql() + " IS NOT " + rightExpression.ToExpressionCommonSql();
                case ExpressionOperator.Exists:
                    return " EXISTS " + rightExpression.ToExpressionCommonSql();//leftExpression.ToExpressionCommonSql() + " IS NOT " + rightExpression.ToExpressionCommonSql();
                case ExpressionOperator.None:
                    return leftExpression.ToExpressionCommonSql();

                //arthmetric & bitwise operators please call ToString()
            }

            return string.Empty;
        }

        #endregion
    }
}
