using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NSun.Data.MsAccess
{
    public static class MsAccessExtensionMethods
    {
        #region QueryCriteria

        public static ExpressionClip GetCurrentDate(this SelectSqlSection criteria)
        {
            return new ExpressionClip("Date()", System.Data.DbType.DateTime);
        }

        //Now   返回当前时间(完整时间，包括年月日 小时分秒) 
        public static ExpressionClip GetCurrentTime(this SelectSqlSection criteria)
        {
            return new ExpressionClip("NOW()", System.Data.DbType.DateTime);
        }

        //Rnd   返回一个0到1之间的随机数值 
        public static ExpressionClip Rnd(this SelectSqlSection criteria)
        {
            return new ExpressionClip("RND()", System.Data.DbType.Single);
        }

        public static ExpressionClip Rnd(this SelectSqlSection criteria, decimal n)
        {
            var newexpr = new ExpressionClip("RND(?)", System.Data.DbType.Single);
            newexpr.ChildExpressions.Add(new ParameterExpression(n, System.Data.DbType.Single));
            return newexpr;
        }

        public static ExpressionClip Rnd(this SelectSqlSection criteria, ExpressionClip n)
        {
            var newexpr = new ExpressionClip("RND(?)", System.Data.DbType.Single);
            newexpr.ChildExpressions.Add(n);
            return newexpr;
        }

        //Space   产生空格 select Space(4)返回4个空格
        public static ExpressionClip Space(this SelectSqlSection criteria, int n)
        {
            var newexpr = new ExpressionClip("Space(?)", System.Data.DbType.String);
            newexpr.ChildExpressions.Add(new ParameterExpression(n, System.Data.DbType.Int32));
            return newexpr;
        }

        #endregion

        #region Date/Time

        //CDate   将字符串转化成为日期 
        public static ExpressionClip CDate(this ExpressionClip expr)
        {
            return new ExpressionClip("CDate(" + expr.Sql + ")", System.Data.DbType.DateTime,
                                      ((ExpressionClip)expr.Clone()).ChildExpressions);
        }

        //DateAdd 将指定日期加上某个日期select dateAdd("d",30,Date())将当前日期加上30天,其中d可以换为yyyy或H等 
        public static ExpressionClip DateAdd(this ExpressionClip expr, string addtype, int n)
        {
            var newExpr = (ExpressionClip)expr.Clone();
            newExpr.Sql = "DATEADD(\"" + addtype + "\", ?, " + newExpr.Sql + ")";
            newExpr.ChildExpressions.Insert(0, new ParameterExpression(n, System.Data.DbType.Int32));
            return newExpr;
        }

        //DateDiff 判断两个日期之间的间隔 select DateDiff("d","2006-5-1","2006-6-1")返回31,其中d可以换为yyyy,m,H等 
        public static ExpressionClip DateDiff(this ExpressionClip expr, string addtype, DateTime time)
        {
            var newExpr = (ExpressionClip)expr.Clone();
            newExpr.Sql = "DATEDIFF(\"" + addtype + "\", " + newExpr.Sql + ",?)";
            newExpr.ChildExpressions.Add(new ParameterExpression(time, System.Data.DbType.DateTime));
            return newExpr;
        }

        public static ExpressionClip DateDiff(this ExpressionClip expr, string addtype, ExpressionClip n)
        {
            if (n == null)
                throw new ArgumentNullException("n");
            var newExpr = (ExpressionClip)expr.Clone();
            newExpr.Sql = "DATEDIFF(\"" + addtype + "\", " + newExpr.Sql + ",?)";
            newExpr.ChildExpressions.Add(n);
            return newExpr;
        }

        //DatePart 返回日期的某个部分 select DatePart("d","2006-5-1")返回1,即1号，d也可以换为yyyy或m
        public static ExpressionClip DatePart(this ExpressionClip expr, string addtype)
        {
            return new ExpressionClip("DATEPART(\"" + addtype + "\", " + expr.Sql + ")", System.Data.DbType.Int32,
                                      ((ExpressionClip)expr.Clone()).ChildExpressions);
        }

        //Day   返回日期的d部分,等同于datepart的d部分 
        public static ExpressionClip Day(this ExpressionClip expr)
        {
            return new ExpressionClip("CDate(" + expr.Sql + ")", System.Data.DbType.Int32,
                                      ((ExpressionClip)expr.Clone()).ChildExpressions);
        }

        //Hour   返回日期的小时
        public static ExpressionClip Hour(this ExpressionClip expr)
        {
            return new ExpressionClip("HOUR(" + expr.Sql + ")", System.Data.DbType.Int32,
                                      ((ExpressionClip)expr.Clone()).ChildExpressions);
        }

        //IsDate   判断是否是日期,是日期返回-1,不是日期返回0 
        public static ExpressionClip IsDate(this ExpressionClip expr)
        {
            return new ExpressionClip("ISDATE(" + expr.Sql + ")", System.Data.DbType.Int32,
                                      ((ExpressionClip)expr.Clone()).ChildExpressions);
        }

        //Minute   返回日期的分钟部分 
        public static ExpressionClip Minute(this ExpressionClip expr)
        {
            return new ExpressionClip("MINUTE(" + expr.Sql + ")", System.Data.DbType.Int32,
                                      ((ExpressionClip)expr.Clone()).ChildExpressions);
        }

        //Month   返回日期的月份部分 
        public static ExpressionClip Month(this ExpressionClip expr)
        {
            return new ExpressionClip("MONTH(" + expr.Sql + ")", System.Data.DbType.Int32,
                                      ((ExpressionClip)expr.Clone()).ChildExpressions);
        }

        //Second   返回日期的秒部分 
        public static ExpressionClip Second(this ExpressionClip expr)
        {
            return new ExpressionClip("SECOND(" + expr.Sql + ")", System.Data.DbType.Int32,
                                      ((ExpressionClip)expr.Clone()).ChildExpressions);
        }

        //Time   返回当前的时间部分(即除去年/月/日的部分) 
        public static ExpressionClip Time(this ExpressionClip expr)
        {
            return new ExpressionClip("TIME(" + expr.Sql + ")", System.Data.DbType.DateTime,
                                      ((ExpressionClip)expr.Clone()).ChildExpressions);
        }

        //Weekday   返回某个日期的当前星期(星期天为1,星期一为2,星期二为3...)，例如select weekday(now()); 
        public static ExpressionClip Weekday(this ExpressionClip expr)
        {
            return new ExpressionClip("WEEKDAY(" + expr.Sql + ")", System.Data.DbType.Int32,
                                      ((ExpressionClip)expr.Clone()).ChildExpressions);
        }
        //Year   返回某个日期的年份
        public static ExpressionClip Year(this ExpressionClip expr)
        {
            return new ExpressionClip("YEAR(" + expr.Sql + ")", System.Data.DbType.Int32,
                                      ((ExpressionClip)expr.Clone()).ChildExpressions);
        }

        #endregion

        #region Check Method

        /// <summary>
        /// IsNull   检测是否为Null值，null值返回0，非null值返回-1
        /// </summary>
        /// <param name="expr"></param>
        /// <returns></returns>
        public static ExpressionClip IsNull(this ExpressionClip expr)
        {
            return new ExpressionClip("ISNULL(" + expr.Sql + ")", System.Data.DbType.Int32,
                                      ((ExpressionClip)expr.Clone()).ChildExpressions);
        }

        /// <summary>
        /// IsNumeric 检测是否为数字,是数字返回-1，否则返回0
        /// </summary>
        /// <param name="expr"></param>
        /// <returns></returns>
        public static ExpressionClip IsNumeric(this ExpressionClip expr)
        {
            return new ExpressionClip("ISNUMERIC(" + expr.Sql + ")", System.Data.DbType.Int32,
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
        public static ExpressionClip Atn(this ExpressionClip expr)
        {
            return new ExpressionClip("ATN(" + expr.Sql + ")", System.Data.DbType.Double,
                                    ((ExpressionClip)expr.Clone()).ChildExpressions);
        }

        //Cos    余弦值 
        public static ExpressionClip Cos(this ExpressionClip expr)
        {
            return new ExpressionClip("COS(" + expr.Sql + ")", System.Data.DbType.Double,
                                    ((ExpressionClip)expr.Clone()).ChildExpressions);
        }

        //Exp    返回 e 的给定次幂。 
        public static ExpressionClip Exp(this ExpressionClip expr)
        {
            return new ExpressionClip("EXP(" + expr.Sql + ")", System.Data.DbType.Double,
                                    ((ExpressionClip)expr.Clone()).ChildExpressions);
        }

        //Fix    返回数字的整数部分(即小数部分完全截掉) 
        public static ExpressionClip Fix(this ExpressionClip expr)
        {
            return new ExpressionClip("FIX(" + expr.Sql + ")", System.Data.DbType.Int32,
                                    ((ExpressionClip)expr.Clone()).ChildExpressions);
        }

        //Int   将数字向下取整到最接近的整数。(其实等同于Fix) 
        public static ExpressionClip Int(this ExpressionClip expr)
        {
            return new ExpressionClip("INT(" + expr.Sql + ")", System.Data.DbType.Int32,
                                    ((ExpressionClip)expr.Clone()).ChildExpressions);
        }

        //Log   返回以E为底的对数值 
        public static ExpressionClip Log(this ExpressionClip expr)
        {
            return new ExpressionClip("LOG(" + expr.Sql + ")", System.Data.DbType.Double,
                                    ((ExpressionClip)expr.Clone()).ChildExpressions);
        }

        //Sgn   返回数字的正负符号(正数返回1,负数返回-1,0值返回0) 
        public static ExpressionClip Sgn(this ExpressionClip expr)
        {
            return new ExpressionClip("SGN(" + expr.Sql + ")", System.Data.DbType.Int32,
                                    ((ExpressionClip)expr.Clone()).ChildExpressions);
        }

        //Sin   正弦值 
        public static ExpressionClip Sin(this ExpressionClip expr)
        {
            return new ExpressionClip("SIN(" + expr.Sql + ")", System.Data.DbType.Double,
                                    ((ExpressionClip)expr.Clone()).ChildExpressions);
        }

        //Sqr   返回平方根值 
        public static ExpressionClip Sqr(this ExpressionClip expr)
        {
            return new ExpressionClip("SQR(" + expr.Sql + ")", System.Data.DbType.Double,
                                    ((ExpressionClip)expr.Clone()).ChildExpressions);
        }

        //Tan    正切值
        public static ExpressionClip Tan(this ExpressionClip expr)
        {
            return new ExpressionClip("TAN(" + expr.Sql + ")", System.Data.DbType.Double,
                                    ((ExpressionClip)expr.Clone()).ChildExpressions);
        }

        #endregion

        #region Goupr Method

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

        #region String

        //Asc    返回字母的Acsii值，select Asc("A")返回65 
        public static ExpressionClip Asc(this ExpressionClip expr)
        {
            return new ExpressionClip("ASC(" + expr.Sql + ")", System.Data.DbType.Int32,
                                    ((ExpressionClip)expr.Clone()).ChildExpressions);
        }

        //Chr    将ascii值转换到字符 select chr(65)返回"A" 
        public static ExpressionClip Chr(this ExpressionClip expr)
        {
            return new ExpressionClip("CHR(" + expr.Sql + ")", System.Data.DbType.String,
                                    ((ExpressionClip)expr.Clone()).ChildExpressions);
        }

        //Format    格式化字符串，Select Format(now(),'yyyy-mm-dd')返回类似于"2005-04-03" ,Select Format(3/9,"0.00")返回0.33 
        public static ExpressionClip Format(this ExpressionClip expr, string format)
        {
            return new ExpressionClip("FORMAT(" + expr.Sql + ",\"" + format + "\")", System.Data.DbType.String,
                                      ((ExpressionClip)expr.Clone()).ChildExpressions);
        }

        //InStr    查询子串在字符串中的位置 select Instr("abc","a")返回1,select Instr("abc","f")返回0 
        public static ExpressionClip InStr(this ExpressionClip expr, string findstr)
        {
            var newExpr = new ExpressionClip("INSTR(" + expr.Sql + ",?)", System.Data.DbType.Int32,
                                      ((ExpressionClip)expr.Clone()).ChildExpressions);
            newExpr.ChildExpressions.Add(new ParameterExpression(findstr, System.Data.DbType.String));
            return newExpr;
        }

        //LCase   返回字符串的小写形式
        public static ExpressionClip LCase(this ExpressionClip expr)
        {
            var newexpr = (ExpressionClip)expr.Clone();
            newexpr.Sql = "LCASE(" + expr.Sql + ")";
            return newexpr;
        }

        //UCase   将字符串转大写
        public static ExpressionClip UCase(this ExpressionClip expr)
        {
            var newexpr = (ExpressionClip)expr.Clone();
            newexpr.Sql = "UCASE(" + expr.Sql + ")";
            return newexpr;
        }

        //Left   左截取字符串 
        public static ExpressionClip Left(this ExpressionClip expr, int length)
        {
            var newexpr = (ExpressionClip)expr.Clone();
            newexpr.Sql = "LEFT(" + expr.Sql + ",?)";
            newexpr.ChildExpressions.Add(new ParameterExpression(length, System.Data.DbType.Int32));
            return newexpr;
        }

        //Right   右截取字符串 
        public static ExpressionClip Right(this ExpressionClip expr, int length)
        {
            var newexpr = (ExpressionClip)expr.Clone();
            newexpr.Sql = "RIGHT(" + expr.Sql + ",?)";
            newexpr.ChildExpressions.Add(new ParameterExpression(length, System.Data.DbType.Int32));
            return newexpr;
        }

        //Len   返回字符串长度 
        public static ExpressionClip Len(this ExpressionClip expr)
        {
            return new ExpressionClip("LEN(" + expr.Sql + ")", System.Data.DbType.Int32,
                                      ((ExpressionClip)expr.Clone()).ChildExpressions);
        }

        //LTrim   左截取空格 
        public static ExpressionClip LTrim(this ExpressionClip expr)
        {
            var newexpr = (ExpressionClip)expr.Clone();
            newexpr.Sql = "LTRIM(" + expr.Sql + ")";
            return newexpr;
        }

        //RTrim   右截取空格 
        public static ExpressionClip RTrim(this ExpressionClip expr)
        {
            var newexpr = (ExpressionClip)expr.Clone();
            newexpr.Sql = "RTRIM(" + expr.Sql + ")";
            return newexpr;
        }

        //Trim   截取字符串两头的空格
        public static ExpressionClip Trim(this ExpressionClip expr)
        {
            var newexpr = (ExpressionClip)expr.Clone();
            newexpr.Sql = "TRIM(" + expr.Sql + ")";
            return newexpr;
        }

        //StrComp   比较两个字符串是否内容一致(不区分大小写)select StrComp("abc","ABC")返回0,select StrComp("abc","123")返回-1 
        public static ExpressionClip StrComp(this ExpressionClip expr, string str)
        {
            var newexpr = new ExpressionClip("STRCOMP(" + expr.Sql + ",?)", System.Data.DbType.Int32,
                                    ((ExpressionClip)expr.Clone()).ChildExpressions);
            newexpr.ChildExpressions.Add(new ParameterExpression(str, System.Data.DbType.String));
            return newexpr;
        }

        public static ExpressionClip StrComp(this ExpressionClip expr, ExpressionClip str)
        {
            var newexpr = new ExpressionClip("STRCOMP(" + expr.Sql + ",?)", System.Data.DbType.Int32,
                                    ((ExpressionClip)expr.Clone()).ChildExpressions);
            newexpr.ChildExpressions.Add(str);
            return newexpr;
        }

        //Mid     取得子字符串 select mid("123",1,2) as midDemo 返回12         
        public static ExpressionClip Mid(this ExpressionClip expr, int begin, int length)
        {
            var newExpr = (ExpressionClip)expr.Clone();

            newExpr.Sql = "Mid(" + newExpr.Sql + ", ?, ?)";
            newExpr.ChildExpressions.Add(new ParameterExpression(begin, System.Data.DbType.Int32));
            newExpr.ChildExpressions.Add(new ParameterExpression(length, System.Data.DbType.Int32));

            return newExpr;
        }

        public static ExpressionClip Mid(this ExpressionClip expr, ExpressionClip begin, int length)
        {
            if (ReferenceEquals(begin, null))
                throw new ArgumentNullException("begin");

            var newExpr = (ExpressionClip)expr.Clone();

            newExpr.Sql = "Mid(" + newExpr.Sql + ", ?, ?)";
            newExpr.ChildExpressions.Add(begin);
            newExpr.ChildExpressions.Add(new ParameterExpression(length, System.Data.DbType.Int32));

            return newExpr;
        }

        public static ExpressionClip Mid(this ExpressionClip expr, int begin, ExpressionClip length)
        {
            if (ReferenceEquals(length, null))
                throw new ArgumentNullException("length");

            var newExpr = (ExpressionClip)expr.Clone();

            newExpr.Sql = "Mid(" + newExpr.Sql + ", ?, ?)";
            newExpr.ChildExpressions.Add(new ParameterExpression(begin, System.Data.DbType.Int32));
            newExpr.ChildExpressions.Add(length);

            return newExpr;
        }

        public static ExpressionClip Mid(this ExpressionClip expr, ExpressionClip begin, ExpressionClip length)
        {
            if (ReferenceEquals(begin, null))
                throw new ArgumentNullException("begin");
            if (ReferenceEquals(length, null))
                throw new ArgumentNullException("length");

            var newExpr = (ExpressionClip)expr.Clone();

            newExpr.Sql = "Mid(" + newExpr.Sql + ", ?, ?)";
            newExpr.ChildExpressions.Add(begin);
            newExpr.ChildExpressions.Add(length);

            return newExpr;
        }

        #endregion
    }
}
