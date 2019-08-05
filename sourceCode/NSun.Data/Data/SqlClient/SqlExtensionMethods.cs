using System;

namespace NSun.Data.SqlClient
{
    /// <summary>
    /// Extension query methods for SqlServer database
    /// </summary>
    public static class SqlExtensionMethods
    {
        #region QueryCriteria

        public static SelectSqlSection SortByRandom(this SelectSqlSection criteria)
        {
            criteria.SortBys.Add(new ExpressionClip("newid()", System.Data.DbType.Guid), false);
            return criteria;
        }

        public static SelectSqlSection ThenSortByRandom(this SelectSqlSection criteria)
        {
            return SortByRandom(criteria);
        }

        public static ExpressionClip GetCurrentDate()
        {
            return new ExpressionClip("getdate()", System.Data.DbType.DateTime);
        }

        public static ExpressionClip GetCurrentDate(this SelectSqlSection criteria)
        {
            return new ExpressionClip("getdate()", System.Data.DbType.DateTime);
        }

        public static ExpressionClip GetCurrentUtcDate(this SelectSqlSection criteria)
        {
            return new ExpressionClip("getutcdate()", System.Data.DbType.DateTime);
        }

        //CONVERT
        public static ExpressionClip Convert(this ExpressionClip expr, string datetype)
        {
            var newExpr = (ExpressionClip)expr.Clone();
            newExpr.Sql = string.Format("CONVERT({0} ," + expr.Sql + ")", datetype);
            //newExpr.ChildExpressions.Insert(0, new ParameterExpression(datetype, System.Data.DbType.String));
            return newExpr;
        }

        public static ExpressionClip Convert(this ExpressionClip expr, string datetype, string style)
        {
            var newExpr = (ExpressionClip)expr.Clone();
            newExpr.Sql = string.Format("CONVERT({0} ," + expr.Sql + ", {1})", datetype, style);
            //newExpr.ChildExpressions.Insert(0, new ParameterExpression(datetype, System.Data.DbType.String));
            //newExpr.ChildExpressions.Add(new ParameterExpression(style, System.Data.DbType.String));
            return newExpr;
        }

        //CAST ( expression AS data_type )
        public static ExpressionClip Cast(this ExpressionClip expr, string datetype)
        {
            var newExpr = (ExpressionClip)expr.Clone();
            newExpr.Sql = string.Format("CAST(" + expr.Sql + " AS {0})", datetype);
            //newExpr.ChildExpressions.Add(new ParameterExpression(datetype, System.Data.DbType.String));
            return newExpr;
        }

        #endregion

        #region Expression

        #region Int32 Expression

        public static ExpressionClip ToChar(this ExpressionClip expr)
        {
            return new ExpressionClip("CHAR(" + expr.Sql + ")", System.Data.DbType.Int32, ((ExpressionClip)expr.Clone()).ChildExpressions);
        }

        public static ExpressionClip ToNChar(this ExpressionClip expr)
        {
            return new ExpressionClip("NCHAR(" + expr.Sql + ")", System.Data.DbType.Int32, ((ExpressionClip)expr.Clone()).ChildExpressions);
        }

        #endregion

        #region DateTime Expression

        public static ExpressionClip AddDay(this ExpressionClip expr, int n)
        {
            var newExpr = (ExpressionClip)expr.Clone();
            newExpr.Sql = "DATEADD(day, ?, " + newExpr.Sql + ")";
            newExpr.ChildExpressions.Insert(0, new ParameterExpression(n, System.Data.DbType.Int32));
            return newExpr;
        }

        public static ExpressionClip AddDay(this ExpressionClip expr, ExpressionClip n)
        {
            if (n == null)
                throw new ArgumentNullException("n");

            var newExpr = (ExpressionClip)expr.Clone();

            newExpr.Sql = "DATEADD(day, ?, " + newExpr.Sql + ")";
            newExpr.ChildExpressions.Insert(0, n);

            return newExpr;
        }

        public static ExpressionClip AddMonth(this ExpressionClip expr, int n)
        {
            var newExpr = (ExpressionClip)expr.Clone();

            newExpr.Sql = "DATEADD(month, ?, " + newExpr.Sql + ")";
            newExpr.ChildExpressions.Insert(0, new ParameterExpression(n, System.Data.DbType.Int32));

            return newExpr;
        }

        public static ExpressionClip AddMonth(this ExpressionClip expr, ExpressionClip n)
        {
            if (n == null)
                throw new ArgumentNullException("n");

            var newExpr = (ExpressionClip)expr.Clone();

            newExpr.Sql = "DATEADD(month, ?, " + newExpr.Sql + ")";
            newExpr.ChildExpressions.Insert(0, n);

            return newExpr;
        }

        public static ExpressionClip AddYear(this ExpressionClip expr, int n)
        {
            var newExpr = (ExpressionClip)expr.Clone();

            newExpr.Sql = "DATEADD(year, ?, " + newExpr.Sql + ")";
            newExpr.ChildExpressions.Insert(0, new ParameterExpression(n, System.Data.DbType.Int32));

            return newExpr;
        }

        public static ExpressionClip AddYear(this ExpressionClip expr, ExpressionClip n)
        {
            if (n == null)
                throw new ArgumentNullException("n");

            var newExpr = (ExpressionClip)expr.Clone();

            newExpr.Sql = "DATEADD(year, ?, " + newExpr.Sql + ")";
            newExpr.ChildExpressions.Insert(0, n);

            return newExpr;
        }

        public static ExpressionClip AddHour(this ExpressionClip expr, int n)
        {
            var newExpr = (ExpressionClip)expr.Clone();

            newExpr.Sql = "DATEADD(hour, ?, " + newExpr.Sql + ")";
            newExpr.ChildExpressions.Insert(0, new ParameterExpression(n, System.Data.DbType.Int32));

            return newExpr;
        }

        public static ExpressionClip AddHour(this ExpressionClip expr, ExpressionClip n)
        {
            if (n == null)
                throw new ArgumentNullException("n");

            var newExpr = (ExpressionClip)expr.Clone();

            newExpr.Sql = "DATEADD(hour, ?, " + newExpr.Sql + ")";
            newExpr.ChildExpressions.Insert(0, n);

            return newExpr;
        }

        public static ExpressionClip AddMinute(this ExpressionClip expr, int n)
        {
            var newExpr = (ExpressionClip)expr.Clone();

            newExpr.Sql = "DATEADD(minute, ?, " + newExpr.Sql + ")";
            newExpr.ChildExpressions.Insert(0, new ParameterExpression(n, System.Data.DbType.Int32));

            return newExpr;
        }

        public static ExpressionClip AddMinute(this ExpressionClip expr, ExpressionClip n)
        {
            if (n == null)
                throw new ArgumentNullException("n");

            var newExpr = (ExpressionClip)expr.Clone();

            newExpr.Sql = "DATEADD(minute, ?, " + newExpr.Sql + ")";
            newExpr.ChildExpressions.Insert(0, n);

            return newExpr;
        }

        public static ExpressionClip AddSecond(this ExpressionClip expr, int n)
        {
            var newExpr = (ExpressionClip)expr.Clone();

            newExpr.Sql = "DATEADD(second, ?, " + newExpr.Sql + ")";
            newExpr.ChildExpressions.Insert(0, new ParameterExpression(n, System.Data.DbType.Int32));

            return newExpr;
        }

        public static ExpressionClip AddSecond(this ExpressionClip expr, ExpressionClip n)
        {
            if (n == null)
                throw new ArgumentNullException("n");

            var newExpr = (ExpressionClip)expr.Clone();

            newExpr.Sql = "DATEADD(second, ?, " + newExpr.Sql + ")";
            newExpr.ChildExpressions.Insert(0, n);

            return newExpr;
        }

        public static ExpressionClip GetDay(this ExpressionClip expr)
        {
            return new ExpressionClip("DATEPART(day, " + expr.Sql + ")", System.Data.DbType.Int32, ((ExpressionClip)expr.Clone()).ChildExpressions);
        }

        public static ExpressionClip Day(this ExpressionClip expr)
        {
            return new ExpressionClip("DAY(" + expr.Sql + ")", System.Data.DbType.Int32, ((ExpressionClip)expr.Clone()).ChildExpressions);
        }

        public static ExpressionClip GetMonth(this ExpressionClip expr)
        {
            return new ExpressionClip("DATEPART(month, " + expr.Sql + ")", System.Data.DbType.Int32, ((ExpressionClip)expr.Clone()).ChildExpressions);
        }

        public static ExpressionClip Month(this ExpressionClip expr)
        {
            return new ExpressionClip("MONTH(" + expr.Sql + ")", System.Data.DbType.Int32, ((ExpressionClip)expr.Clone()).ChildExpressions);
        }

        public static ExpressionClip GetYear(this ExpressionClip expr)
        {
            return new ExpressionClip("DATEPART(year, " + expr.Sql + ")", System.Data.DbType.Int32, ((ExpressionClip)expr.Clone()).ChildExpressions);
        }

        public static ExpressionClip Year(this ExpressionClip expr)
        {
            return new ExpressionClip("YEAR(" + expr.Sql + ")", System.Data.DbType.Int32, ((ExpressionClip)expr.Clone()).ChildExpressions);
        }

        public static ExpressionClip GetHour(this ExpressionClip expr)
        {
            return new ExpressionClip("DATEPART(hour, " + expr.Sql + ")", System.Data.DbType.Int32, ((ExpressionClip)expr.Clone()).ChildExpressions);
        }

        public static ExpressionClip GetMinute(this ExpressionClip expr)
        {
            return new ExpressionClip("DATEPART(minute, " + expr.Sql + ")", System.Data.DbType.Int32, ((ExpressionClip)expr.Clone()).ChildExpressions);
        }

        public static ExpressionClip GetSecond(this ExpressionClip expr)
        {
            return new ExpressionClip("DATEPART(second, " + expr.Sql + ")", System.Data.DbType.Int32, ((ExpressionClip)expr.Clone()).ChildExpressions);
        }

        public static ExpressionClip DateDiff(this ExpressionClip expr, string datepart, DateTime enddate)
        {
            var newexpr = new ExpressionClip("DATEDIFF(" + datepart + ", " + expr.Sql + ",?)", System.Data.DbType.Int32,
                               ((ExpressionClip)expr.Clone()).ChildExpressions);
            newexpr.ChildExpressions.Add(new ParameterExpression(enddate, System.Data.DbType.DateTime));
            return newexpr;
        }

        #endregion

        #region String Expression

        public static Condition Contains(this ExpressionClip expr, string value)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentNullException("value");

            return Contains(expr, new ParameterExpression(value, System.Data.DbType.String));
        }

        public static Condition Contains(this ExpressionClip expr, ExpressionClip value)
        {
            if (ReferenceEquals(value, null))
                throw new ArgumentNullException("value");

            var escapedLikeValue = (ExpressionClip)value.Clone();
            escapedLikeValue.Sql = "'%' + " + escapedLikeValue.Sql + " + '%'";

            return new Condition(expr, ExpressionOperator.Like, escapedLikeValue);
        }

        public static Condition EndsWith(this ExpressionClip expr, string value)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentNullException("value");

            return EndsWith(expr, new ParameterExpression(value, System.Data.DbType.String));
        }

        public static Condition EndsWith(this ExpressionClip expr, ExpressionClip value)
        {
            if (ReferenceEquals(value, null))
                throw new ArgumentNullException("value");

            var escapedLikeValue = (ExpressionClip)value.Clone();
            escapedLikeValue.Sql = "'%' + " + escapedLikeValue.Sql;

            return new Condition(expr, ExpressionOperator.Like, escapedLikeValue);
        }

        public static Condition StartsWith(this ExpressionClip expr, string value)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentNullException("value");

            return StartsWith(expr, new ParameterExpression(value, System.Data.DbType.String));
        }

        public static Condition StartsWith(this ExpressionClip expr, ExpressionClip value)
        {
            if (ReferenceEquals(value, null))
                throw new ArgumentNullException("value");

            var escapedLikeValue = (ExpressionClip)value.Clone();
            escapedLikeValue.Sql = escapedLikeValue.Sql + " + '%'";

            return new Condition(expr, ExpressionOperator.Like, escapedLikeValue);
        }

        public static ExpressionClip Charindex(this ExpressionClip expr, string value)
        {
            var newExpr = new ExpressionClip("CHARINDEX(?, " + expr.Sql + ") - 1", System.Data.DbType.Int32, ((ExpressionClip)expr.Clone()).ChildExpressions);

            newExpr.ChildExpressions.Insert(0, new ParameterExpression(value, System.Data.DbType.String));

            return newExpr;
        }

        public static ExpressionClip Charindex(this ExpressionClip expr, ExpressionClip value)
        {
            var newExpr = new ExpressionClip("CHARINDEX(?, " + expr.Sql + ") - 1", System.Data.DbType.Int32, ((ExpressionClip)expr.Clone()).ChildExpressions);

            newExpr.ChildExpressions.Insert(0, value);

            return newExpr;
        }

        public static ExpressionClip Replace(this ExpressionClip expr, string find, string replace)
        {
            if (string.IsNullOrEmpty(find))
                throw new ArgumentNullException("find");
            if (string.IsNullOrEmpty(replace))
                throw new ArgumentNullException("replace");

            var newExpr = (ExpressionClip)expr.Clone();

            newExpr.Sql = "REPLACE(" + newExpr.Sql + ", ?, ?)";
            newExpr.ChildExpressions.Add(new ParameterExpression(find, System.Data.DbType.String));
            newExpr.ChildExpressions.Add(new ParameterExpression(replace, System.Data.DbType.String));

            return newExpr;
        }

        public static ExpressionClip Replace(this ExpressionClip expr, ExpressionClip find, string replace)
        {
            if (ReferenceEquals(find, null))
                throw new ArgumentNullException("find");
            if (string.IsNullOrEmpty(replace))
                throw new ArgumentNullException("replace");

            var newExpr = (ExpressionClip)expr.Clone();

            newExpr.Sql = "REPLACE(" + newExpr.Sql + ", ?, ?)";
            newExpr.ChildExpressions.Add(find);
            newExpr.ChildExpressions.Add(new ParameterExpression(replace, System.Data.DbType.String));

            return newExpr;
        }

        public static ExpressionClip Replace(this ExpressionClip expr, string find, ExpressionClip replace)
        {
            if (string.IsNullOrEmpty(find))
                throw new ArgumentNullException("find");
            if (ReferenceEquals(replace, null))
                throw new ArgumentNullException("replace");

            var newExpr = (ExpressionClip)expr.Clone();

            newExpr.Sql = "REPLACE(" + newExpr.Sql + ", ?, ?)";
            newExpr.ChildExpressions.Add(new ParameterExpression(find, System.Data.DbType.String));
            newExpr.ChildExpressions.Add(replace);

            return newExpr;
        }

        public static ExpressionClip Replace(this ExpressionClip expr, ExpressionClip find, ExpressionClip replace)
        {
            if (ReferenceEquals(find, null))
                throw new ArgumentNullException("find");
            if (ReferenceEquals(replace, null))
                throw new ArgumentNullException("replace");

            var newExpr = (ExpressionClip)expr.Clone();

            newExpr.Sql = "REPLACE(" + newExpr.Sql + ", ?, ?)";
            newExpr.ChildExpressions.Add(find);
            newExpr.ChildExpressions.Add(replace);

            return newExpr;
        }

        public static ExpressionClip Substring(this ExpressionClip expr, int begin, int length)
        {
            var newExpr = (ExpressionClip)expr.Clone();

            newExpr.Sql = "SUBSTRING(" + newExpr.Sql + ", ? + 1, ?)";
            newExpr.ChildExpressions.Add(new ParameterExpression(begin, System.Data.DbType.Int32));
            newExpr.ChildExpressions.Add(new ParameterExpression(length, System.Data.DbType.Int32));

            return newExpr;
        }

        public static ExpressionClip Substring(this ExpressionClip expr, ExpressionClip begin, int length)
        {
            if (ReferenceEquals(begin, null))
                throw new ArgumentNullException("begin");

            var newExpr = (ExpressionClip)expr.Clone();

            newExpr.Sql = "SUBSTRING(" + newExpr.Sql + ", ? + 1, ?)";
            newExpr.ChildExpressions.Add(begin);
            newExpr.ChildExpressions.Add(new ParameterExpression(length, System.Data.DbType.Int32));

            return newExpr;
        }

        public static ExpressionClip Substring(this ExpressionClip expr, int begin, ExpressionClip length)
        {
            if (ReferenceEquals(length, null))
                throw new ArgumentNullException("length");

            var newExpr = (ExpressionClip)expr.Clone();

            newExpr.Sql = "SUBSTRING(" + newExpr.Sql + ", ? + 1, ?)";
            newExpr.ChildExpressions.Add(new ParameterExpression(begin, System.Data.DbType.Int32));
            newExpr.ChildExpressions.Add(length);

            return newExpr;
        }

        public static ExpressionClip Substring(this ExpressionClip expr, ExpressionClip begin, ExpressionClip length)
        {
            if (ReferenceEquals(begin, null))
                throw new ArgumentNullException("begin");
            if (ReferenceEquals(length, null))
                throw new ArgumentNullException("length");

            var newExpr = (ExpressionClip)expr.Clone();

            newExpr.Sql = "SUBSTRING(" + newExpr.Sql + ", ? + 1, ?)";
            newExpr.ChildExpressions.Add(begin);
            newExpr.ChildExpressions.Add(length);

            return newExpr;
        }

        public static ExpressionClip Left(this ExpressionClip expr, int length)
        {
            var newExpr = (ExpressionClip)expr.Clone();

            newExpr.Sql = "LEFT(" + newExpr.Sql + ", ?)";
            newExpr.ChildExpressions.Add(new ParameterExpression(length, System.Data.DbType.Int32));

            return newExpr;
        }

        public static ExpressionClip Left(this ExpressionClip expr, ExpressionClip length)
        {
            if (ReferenceEquals(length, null))
                throw new ArgumentNullException("length");

            var newExpr = (ExpressionClip)expr.Clone();

            newExpr.Sql = "LEFT(" + newExpr.Sql + ", ?)";
            newExpr.ChildExpressions.Add(length);

            return newExpr;
        }

        public static ExpressionClip Right(this ExpressionClip expr, int length)
        {
            var newExpr = (ExpressionClip)expr.Clone();

            newExpr.Sql = "RIGHT(" + newExpr.Sql + ", ?)";
            newExpr.ChildExpressions.Add(new ParameterExpression(length, System.Data.DbType.Int32));

            return newExpr;
        }

        public static ExpressionClip Right(this ExpressionClip expr, ExpressionClip length)
        {
            if (ReferenceEquals(length, null))
                throw new ArgumentNullException("length");

            var newExpr = (ExpressionClip)expr.Clone();

            newExpr.Sql = "RIGHT(" + newExpr.Sql + ", ?)";
            newExpr.ChildExpressions.Add(length);

            return newExpr;
        }

        public static ExpressionClip LTrim(this ExpressionClip expr)
        {
            var newExpr = (ExpressionClip)expr.Clone();

            newExpr.Sql = "LTRIM(" + expr.Sql + ")";

            return newExpr;
        }
         
        public static ExpressionClip RTrim(this ExpressionClip expr)
        {
            var newExpr = (ExpressionClip)expr.Clone();

            newExpr.Sql = "RTRIM(" + expr.Sql + ")";

            return newExpr;
        }

        public static ExpressionClip Ascii(this ExpressionClip expr)
        {
            return new ExpressionClip("ASCII(" + expr.Sql + ")", System.Data.DbType.Int32, ((ExpressionClip)expr.Clone()).ChildExpressions);
        }

        public static ExpressionClip Unicode(this ExpressionClip expr)
        {
            return new ExpressionClip("UNICODE(" + expr.Sql + ")", System.Data.DbType.Int32, ((ExpressionClip)expr.Clone()).ChildExpressions);
        }

        public static ExpressionClip Length(this ExpressionClip expr)
        {
            return new ExpressionClip("LEN(" + expr.Sql + ")", System.Data.DbType.Int32,
                                      ((ExpressionClip)expr.Clone()).ChildExpressions);

        }

        //PATINDEX 返回指定表达式中某模式第一次出现的起始位置；如果在全部有效的文本和字符数据类型中没有找到该模式，则返回零。
        public static ExpressionClip Patindex(this ExpressionClip expr, string txt)
        {
            var newExpr = (ExpressionClip)expr.Clone();
            newExpr.Sql = "PATINDEX(?," + newExpr.Sql + ")";
            newExpr.ChildExpressions.Insert(0, new ParameterExpression(txt, System.Data.DbType.String));
            return newExpr;
        }

        //LOWER 将大写字符数据转换为小写字符数据后返回字符表达式。
        public static ExpressionClip Lower(this ExpressionClip expr)
        {
            var newExpr = (ExpressionClip)expr.Clone();
            newExpr.Sql = "LOWER(" + expr.Sql + ")";
            return newExpr;
        }

        //UPPER 返回将小写字符数据转换为大写的字符表达式
        public static ExpressionClip Upper(this ExpressionClip expr)
        {
            var newExpr = (ExpressionClip)expr.Clone();
            newExpr.Sql = "UPPER(" + expr.Sql + ")";
            return newExpr;
        }


        #endregion

        #region Group

        //COUNT_BIG
        public static ExpressionClip CountBig(this ExpressionClip expr)
        {
            return CountBig(expr, false);
        }

        public static ExpressionClip CountBig(this ExpressionClip expr, bool distinct)
        {
            if (distinct)
            {
                return new ExpressionClip("COUNT_BIG(DISTINCT " + expr.Sql + ")",
              System.Data.DbType.Int64, ((ExpressionClip)expr.Clone()).ChildExpressions);
            }
            else
            {
                return new ExpressionClip("COUNT_BIG(" + expr.Sql + ")",
               System.Data.DbType.Int64, ((ExpressionClip)expr.Clone()).ChildExpressions);
            }
        }

        //StDev   估算样本的标准差（忽略样本中的逻辑值和文本）。 
        public static ExpressionClip StDev(this ExpressionClip expr)
        {
            return new ExpressionClip("STDEV(" + expr.Sql + ")", System.Data.DbType.Single,
                                    ((ExpressionClip)expr.Clone()).ChildExpressions);
        }

        //StDevP   计算以参数形式（忽略逻辑值和文本）给出的整个样本总体的标准偏差。 
        public static ExpressionClip StDevP(this ExpressionClip expr)
        {
            return new ExpressionClip("STDEVP(" + expr.Sql + ")", System.Data.DbType.Single,
                                    ((ExpressionClip)expr.Clone()).ChildExpressions);
        }

        //Var   估算样本方差（忽略样本中的逻辑值和文本）。 
        public static ExpressionClip Var(this ExpressionClip expr)
        {
            return new ExpressionClip("VAR(" + expr.Sql + ")", System.Data.DbType.Single,
                                    ((ExpressionClip)expr.Clone()).ChildExpressions);
        }

        //VarP   计算整个样本总体的方差（忽略样本总体中的逻辑值和文本）。
        public static ExpressionClip VarP(this ExpressionClip expr)
        {
            return new ExpressionClip("VARP(" + expr.Sql + ")", System.Data.DbType.Single,
                                    ((ExpressionClip)expr.Clone()).ChildExpressions);
        }

        #endregion

        #region Math Method

        //Abs    绝对值 
        public static ExpressionClip Abs(this ExpressionClip expr)
        {
            var newexpr = (ExpressionClip)expr.Clone();
            newexpr.Sql = "ABS(" + expr.Sql + ")";
            return newexpr;
        }

        //Atn    返正切值。 
        public static ExpressionClip Atan(this ExpressionClip expr)
        {
            return new ExpressionClip("ATN(" + expr.Sql + ")", System.Data.DbType.Single,
                                    ((ExpressionClip)expr.Clone()).ChildExpressions);
        }

        public static ExpressionClip Atan2(this ExpressionClip expr, float al2)
        {
            var newexpr = new ExpressionClip("ATN2(" + expr.Sql + ",?)", System.Data.DbType.Single,
                                    ((ExpressionClip)expr.Clone()).ChildExpressions);
            newexpr.ChildExpressions.Add(new ParameterExpression(al2, System.Data.DbType.Single));
            return newexpr;
        }

        public static ExpressionClip Atan2(this ExpressionClip expr, ExpressionClip al2)
        {
            var newexpr = new ExpressionClip("ATN2(" + expr.Sql + ",?)", System.Data.DbType.Single,
                                    ((ExpressionClip)expr.Clone()).ChildExpressions);
            newexpr.ChildExpressions.Add(al2);
            return newexpr;
        }

        //CEILING 返回大于或等于所给数字表达式的最小整数。
        public static ExpressionClip Ceiling(this ExpressionClip expr)
        {
            var newexpr = (ExpressionClip)expr.Clone();
            newexpr.Sql = "CEILING(" + expr.Sql + ")";
            return newexpr;
        }

        //FLOOR 返回小于或等于所给数字表达式的最大整数。
        public static ExpressionClip Floor(this ExpressionClip expr)
        {
            var newexpr = (ExpressionClip)expr.Clone();
            newexpr.Sql = "FLOOR(" + expr.Sql + ")";
            return newexpr;
        }

        //Acos
        public static ExpressionClip ACos(this ExpressionClip expr)
        {
            return new ExpressionClip("ACos(" + expr.Sql + ")", System.Data.DbType.Single,
                                    ((ExpressionClip)expr.Clone()).ChildExpressions);
        }

        //Cos    余弦值 
        public static ExpressionClip Cos(this ExpressionClip expr)
        {
            return new ExpressionClip("COS(" + expr.Sql + ")", System.Data.DbType.Single,
                                    ((ExpressionClip)expr.Clone()).ChildExpressions);
        }

        //Exp    返回 e 的给定次幂。 
        public static ExpressionClip Exp(this ExpressionClip expr)
        {
            return new ExpressionClip("EXP(" + expr.Sql + ")", System.Data.DbType.Single,
                                    ((ExpressionClip)expr.Clone()).ChildExpressions);
        }

        //Log   返回以E为底的对数值 
        public static ExpressionClip Log(this ExpressionClip expr)
        {
            return new ExpressionClip("LOG(" + expr.Sql + ")", System.Data.DbType.Single,
                                    ((ExpressionClip)expr.Clone()).ChildExpressions);
        }

        //LOG10 返回给定 float 表达式的以 10 为底的对数。
        public static ExpressionClip Log10(this ExpressionClip expr)
        {
            return new ExpressionClip("LOG10(" + expr.Sql + ")", System.Data.DbType.Single,
                                    ((ExpressionClip)expr.Clone()).ChildExpressions);
        }

        //SIGN  返回给定表达式的正 (+1)、零 (0) 或负 (-1) 号。
        public static ExpressionClip Sign(this ExpressionClip expr)
        {
            var newexpr = (ExpressionClip)expr.Clone();
            newexpr.Sql = "SIGN(" + expr.Sql + ")";
            return newexpr;
        }

        //ASIN 反正弦值 
        public static ExpressionClip Asin(this ExpressionClip expr)
        {
            return new ExpressionClip("ASIN(" + expr.Sql + ")", System.Data.DbType.Single,
                                    ((ExpressionClip)expr.Clone()).ChildExpressions);
        }

        //Sin   正弦值 
        public static ExpressionClip Sin(this ExpressionClip expr)
        {
            return new ExpressionClip("SIN(" + expr.Sql + ")", System.Data.DbType.Single,
                                    ((ExpressionClip)expr.Clone()).ChildExpressions);
        }

        //Sqr   返回平方根值 
        public static ExpressionClip Sqrt(this ExpressionClip expr)
        {
            return new ExpressionClip("SQRT(" + expr.Sql + ")", System.Data.DbType.Single,
                                    ((ExpressionClip)expr.Clone()).ChildExpressions);
        }

        //Tan    正切值
        public static ExpressionClip Tan(this ExpressionClip expr)
        {
            return new ExpressionClip("TAN(" + expr.Sql + ")", System.Data.DbType.Single,
                                    ((ExpressionClip)expr.Clone()).ChildExpressions);
        }

        //SQUARE 返回给定表达式的平方。
        public static ExpressionClip Square(this ExpressionClip expr)
        {
            return new ExpressionClip("SQUARE(" + expr.Sql + ")", System.Data.DbType.Single,
                                    ((ExpressionClip)expr.Clone()).ChildExpressions);
        }

        //ROUND 返回数字表达式并四舍五入为指定的长度或精度
        public static ExpressionClip Round(this ExpressionClip expr)
        {
            var newexpr = (ExpressionClip)expr.Clone();
            newexpr.Sql = "ROUND(" + expr.Sql + ")";
            return newexpr;
        }

        public static ExpressionClip Round(this ExpressionClip expr, int n)
        {
            var newexpr = (ExpressionClip)expr.Clone();
            newexpr.Sql = "ROUND(" + expr.Sql + ",?)";
            newexpr.ChildExpressions.Add(new ParameterExpression(n, System.Data.DbType.Int32));
            return newexpr;
        }

        //COT 三角余切值
        public static ExpressionClip Cot(this ExpressionClip expr)
        {
            return new ExpressionClip("COT(" + expr.Sql + ")", System.Data.DbType.Single,
                                   ((ExpressionClip)expr.Clone()).ChildExpressions);
        }

        //DEGREES 当给出以弧度为单位的角度时，返回相应的以度数为单位的角度。
        public static ExpressionClip Degrees(this ExpressionClip expr)
        {
            var newexpr = (ExpressionClip)expr.Clone();
            newexpr.Sql = "DEGREES(" + expr.Sql + ")";
            return newexpr;
        }

        //PI 返回 PI 的常量值
        public static ExpressionClip PI(this SelectSqlSection expr)
        {
            return new ExpressionClip("PI()", System.Data.DbType.Single);
        }

        //POWER 返回给定表达式乘指定次方的值。
        public static ExpressionClip Pow(this ExpressionClip expr, int n)
        {
            var newexpr = (ExpressionClip)expr.Clone();
            newexpr.Sql = "POWER(" + expr.Sql + ",?)";
            newexpr.ChildExpressions.Add(new ParameterExpression(n, System.Data.DbType.Int32));
            return newexpr;
        }

        //RADIANS 对于在数字表达式中输入的度数值返回弧度值。
        public static ExpressionClip Radians(this ExpressionClip expr)
        {
            var newexpr = (ExpressionClip)expr.Clone();
            newexpr.Sql = "RADIANS(" + expr.Sql + ")";
            return newexpr;
        }

        //RAND 返回 0 到1 之间的随机float 值
        public static ExpressionClip Rand(this SelectSqlSection criteria)
        {
            return new ExpressionClip("RAND()", System.Data.DbType.Single);
        }

        public static ExpressionClip Rand(this SelectSqlSection criteria, decimal n)
        {
            var newexpr = new ExpressionClip("RND(?)", System.Data.DbType.Single);
            newexpr.ChildExpressions.Add(new ParameterExpression(n, System.Data.DbType.Single));
            return newexpr;
        }

        public static ExpressionClip Rand(this ExpressionClip expr)
        {
            var newexpr = (ExpressionClip)expr.Clone();
            newexpr.Sql = "RAND(" + expr.Sql + ")";
            return newexpr;
        }

        #endregion 

        #endregion
    }
}
