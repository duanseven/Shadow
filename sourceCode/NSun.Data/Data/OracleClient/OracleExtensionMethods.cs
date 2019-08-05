using System;

namespace NSun.Data.OracleClient
{
    /// <summary>
    /// Query extension methods for Oracle database
    /// </summary>
    public static class OracleExtensionMethods
    {
        #region QueryCriteria
 
        public static ExpressionClip GetCurrentDate(this QueryCriteria criteria)
        {
            return new ExpressionClip("CURRENT_TIMESTAMP", System.Data.DbType.DateTime);
        }

        public static ExpressionClip GetCurrentSysDate(this QueryCriteria criteria)
        {
            return new ExpressionClip("SYSDATE", System.Data.DbType.DateTime);
        }

        public static ExpressionClip GetCurrentTimeStamp(this QueryCriteria criteria)
        {
            return new ExpressionClip("CURRENT_TIMESTAMP", System.Data.DbType.DateTime);
        }
        

        #endregion

        #region Expression

        //5.格式控制符的类型：
        //YYYY 四位的年
        //YEAR 年的拼写
        //MM 2 位数字的月
        //MONTH 月的全名
        //MON 月名的前三个字符
        //DY 星期名的前三个字符
        //DAY 星期名的全称
        //DD 2 位的天
        //6.时间格式控制符：
        //HH24:MI:SS AM
        //HH12:MI:SS PM
        #region Int32 Expression

        public static ExpressionClip Char(this ExpressionClip expr)
        {
            return new ExpressionClip("CHR(" + expr.Sql + ")", System.Data.DbType.Int32,
                                      ((ExpressionClip)expr.Clone()).ChildExpressions);
        }

        public static ExpressionClip Nchr(this ExpressionClip expr)
        {
            return new ExpressionClip("NCHR(" + expr.Sql + ")", System.Data.DbType.Int32,
                                      ((ExpressionClip)expr.Clone()).ChildExpressions);
        }

        public static ExpressionClip ToChar(this ExpressionClip expr)
        {
            return new ExpressionClip("TO_CHAR(" + expr.Sql + ")", System.Data.DbType.String,
                                      ((ExpressionClip)expr.Clone()).ChildExpressions);
        }

        public static ExpressionClip ToChar(this ExpressionClip expr,string format)
        {
            var newExpr = (ExpressionClip)expr.Clone();
            newExpr.DataType = System.Data.DbType.String;
            newExpr.Sql = "TO_CHAR(" + expr.Sql + ")";
            newExpr.ChildExpressions.Add(
                new ParameterExpression(format, System.Data.DbType.String));
            return newExpr; 
        }
       
        public static ExpressionClip Convert(this ExpressionClip expr, string datetype)
        {
            var newExpr = (ExpressionClip)expr.Clone();
            newExpr.Sql = "CONVERT(" + expr.Sql + ",?)";
            newExpr.ChildExpressions.Add(new ParameterExpression(datetype, System.Data.DbType.String));
            return newExpr;
        }

        public static ExpressionClip Unicode(this ExpressionClip expr)
        {
            var newExpr = (ExpressionClip)expr.Clone();
            newExpr.Sql = "UNICODE(" + expr.Sql + ")";
            return newExpr;
        }

        #endregion

        #region DateTime Expression

        public static ExpressionClip AddMonth(this ExpressionClip expr, int n)
        {
            var newExpr = (ExpressionClip)expr.Clone();

            newExpr.Sql = "ADD_MONTHS(month, ?, " + newExpr.Sql + ")";
            newExpr.ChildExpressions.Insert(0, new ParameterExpression(n, System.Data.DbType.Int32));

            return newExpr;
        }

        public static ExpressionClip AddMonth(this ExpressionClip expr, ExpressionClip n)
        {
            if (n == null)
                throw new ArgumentNullException("n");

            var newExpr = (ExpressionClip)expr.Clone();

            newExpr.Sql = "ADD_MONTHS(month, ?, " + newExpr.Sql + ")";
            newExpr.ChildExpressions.Insert(0, n);

            return newExpr;
        }

        #endregion

        #region String Expression

        public static ExpressionClip Link(this ExpressionClip left, object right)
        {
            var expr = (ExpressionClip)left.Clone();
            expr.Sql += " " + ExpressionHelper.ToString(ExpressionOperator.Link) + " ?";
            expr.ChildExpressions.Add(new ParameterExpression(right, left.DataType));

            return expr;
        }

        public static ExpressionClip Link(this ExpressionClip left, ExpressionClip right)
        {
            var expr = (ExpressionClip)left.Clone();

            expr.Sql += " " + ExpressionHelper.ToString(ExpressionOperator.Link) + " ?";
            expr.ChildExpressions.Add(right);

            return expr;
        }

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

        public static ExpressionClip IndexOf(this ExpressionClip expr, string value)
        {
            var newExpr = new ExpressionClip("INSTR(" + expr.Sql + ", ?) - 1", System.Data.DbType.Int32, ((ExpressionClip)expr.Clone()).ChildExpressions);

            newExpr.ChildExpressions.Insert(0, new ParameterExpression(value, System.Data.DbType.String));

            return newExpr;
        }

        public static ExpressionClip IndexOf(this ExpressionClip expr, ExpressionClip value)
        {
            var newExpr = new ExpressionClip("INSTR(" + expr.Sql + ", ?) - 1", System.Data.DbType.Int32, ((ExpressionClip)expr.Clone()).ChildExpressions);

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

            newExpr.Sql = "SUBSTR(" + newExpr.Sql + ", ? + 1, ?)";
            newExpr.ChildExpressions.Add(new ParameterExpression(begin, System.Data.DbType.Int32));
            newExpr.ChildExpressions.Add(new ParameterExpression(length, System.Data.DbType.Int32));

            return newExpr;
        }

        public static ExpressionClip Substring(this ExpressionClip expr, ExpressionClip begin, int length)
        {
            if (ReferenceEquals(begin, null))
                throw new ArgumentNullException("begin");

            var newExpr = (ExpressionClip)expr.Clone();

            newExpr.Sql = "SUBSTR(" + newExpr.Sql + ", ? + 1, ?)";
            newExpr.ChildExpressions.Add(begin);
            newExpr.ChildExpressions.Add(new ParameterExpression(length, System.Data.DbType.Int32));

            return newExpr;
        }

        public static ExpressionClip Substring(this ExpressionClip expr, int begin, ExpressionClip length)
        {
            if (ReferenceEquals(length, null))
                throw new ArgumentNullException("length");

            var newExpr = (ExpressionClip)expr.Clone();

            newExpr.Sql = "SUBSTR(" + newExpr.Sql + ", ? + 1, ?)";
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

            newExpr.Sql = "SUBSTR(" + newExpr.Sql + ", ? + 1, ?)";
            newExpr.ChildExpressions.Add(begin);
            newExpr.ChildExpressions.Add(length);

            return newExpr;
        }


        
        public static ExpressionClip ToNumber(this ExpressionClip expr)
        {
            return new ExpressionClip("TO_NUMBER(" + expr.Sql + ")", System.Data.DbType.Decimal, ((ExpressionClip)expr.Clone()).ChildExpressions);
        }

        public static ExpressionClip ToDate(this ExpressionClip expr)
        {
            return new ExpressionClip("TO_DATE(" + expr.Sql + ")", System.Data.DbType.DateTime, ((ExpressionClip)expr.Clone()).ChildExpressions);
        }
        //TRIM
        public static ExpressionClip Trim(this ExpressionClip expr)
        {
            var newExpr = (ExpressionClip)expr.Clone();

            newExpr.Sql = "TRIM(" + expr.Sql + ")";

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

        public static ExpressionClip Lpad(this ExpressionClip expr, int index, string str)
        {
            var newExpr = (ExpressionClip)expr.Clone();

            newExpr.Sql = "LPAD(" + expr.Sql + ",?,?)";
            newExpr.ChildExpressions.Add(new ParameterExpression(index, System.Data.DbType.Int32));
            newExpr.ChildExpressions.Add(new ParameterExpression(str, System.Data.DbType.String));
            return newExpr;
        }

        public static ExpressionClip Rpad(this ExpressionClip expr, int index, string str)
        {
            var newExpr = (ExpressionClip)expr.Clone();

            newExpr.Sql = "RPAD(" + expr.Sql + ",?,?)";
            newExpr.ChildExpressions.Add(new ParameterExpression(index, System.Data.DbType.Int32));
            newExpr.ChildExpressions.Add(new ParameterExpression(str, System.Data.DbType.String));
            return newExpr;
        }

        public static ExpressionClip Ascii(this ExpressionClip expr)
        {
            return new ExpressionClip("ASCII(" + expr.Sql + ")", System.Data.DbType.Int32, ((ExpressionClip)expr.Clone()).ChildExpressions);
        }

        public static ExpressionClip Length(this ExpressionClip expr)
        {
            return new ExpressionClip("LENGTH(" + expr.Sql + ")", System.Data.DbType.Int32, ((ExpressionClip)expr.Clone()).ChildExpressions);
        }

        public static ExpressionClip Lower(this ExpressionClip expr)
        {
            var newExpr = (ExpressionClip)expr.Clone();
            newExpr.Sql = "LOWER(" + expr.Sql + ")";
            return newExpr;
        }

        public static ExpressionClip Upper(this ExpressionClip expr)
        {
            var newExpr = (ExpressionClip)expr.Clone();
            newExpr.Sql = "UPPER(" + expr.Sql + ")";
            return newExpr;
        }

        public static ExpressionClip Initcap(this ExpressionClip expr)
        {
            var newExpr = (ExpressionClip)expr.Clone();
            newExpr.Sql = "INITCAP(" + expr.Sql + ")";
            return newExpr;
        }

        public static ExpressionClip Concat(this ExpressionClip expr, string str)
        {
            var newExpr = (ExpressionClip)expr.Clone();
            newExpr.Sql = "CONCAT(" + expr.Sql + ",?)";
            newExpr.ChildExpressions.Add(new ParameterExpression(str, System.Data.DbType.String));
            return newExpr;
        }

        public static ExpressionClip Concat(this ExpressionClip expr, ExpressionClip str)
        {
            var newExpr = (ExpressionClip)expr.Clone();
            newExpr.Sql = "CONCAT(" + expr.Sql + ",?)";
            newExpr.ChildExpressions.Add(str);
            return newExpr;
        }


        #endregion

        #region Math Expression


        #region Math Method

        //  ABS(x) 函数，此函数用来返回一个数的绝对值。
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

        //CEIL 返回大于或等于所给数字表达式的最小整数。
        public static ExpressionClip Ceil(this ExpressionClip expr)
        {
            var newexpr = (ExpressionClip)expr.Clone();
            newexpr.Sql = "CEIL(" + expr.Sql + ")";
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

        //COSH(x)
        public static ExpressionClip Cosh(this ExpressionClip expr)
        {
            return new ExpressionClip("COSH(" + expr.Sql + ")", System.Data.DbType.Single,
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

        //Sqrt   返回平方根值 
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

        //􀁺 LN(x)函数，返回x 的自然对数。x 必须大于0。
        public static ExpressionClip Ln(this ExpressionClip expr)
        {
            return new ExpressionClip("LN(" + expr.Sql + ")", System.Data.DbType.Single,
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

        //POWER 返回给定表达式乘指定次方的值。
        public static ExpressionClip Pow(this ExpressionClip expr, int n)
        {
            var newexpr = (ExpressionClip)expr.Clone();
            newexpr.Sql = "POWER(" + expr.Sql + ",?)";
            newexpr.ChildExpressions.Add(new ParameterExpression(n, System.Data.DbType.Int32));
            return newexpr;
        }

        public static ExpressionClip Mod(this ExpressionClip expr)
        {
            var newexpr = (ExpressionClip)expr.Clone();
            newexpr.Sql = "MOD(" + expr.Sql + ")";
            return newexpr;
        }

        public static ExpressionClip Sinh(this ExpressionClip expr)
        {
            var newexpr = (ExpressionClip)expr.Clone();
            newexpr.Sql = "SINH(" + expr.Sql + ")";
            return newexpr;
        }

        public static ExpressionClip Tanh(this ExpressionClip expr)
        {
            var newexpr = (ExpressionClip)expr.Clone();
            newexpr.Sql = "TANH(" + expr.Sql + ")";
            return newexpr;
        }

        #endregion

        #endregion

        #endregion
    }
}
