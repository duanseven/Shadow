using System;
using System.Text;

namespace NSun.Data.Sqlite
{
    public static class SqliteExtensionMethods
    {
        /*
         * YYYY-MM-DD
         * YYYY-MM-DD HH:MM
         * YYYY-MM-DD HH:MM:SS
         * YYYY-MM-DD HH:MM:SS.SSS
         * HH:MM
         * HH:MM:SS
         * HH:MM:SS.SSS
         * now
         * select date('2006-10-17','+1 day','+1 year'); res:2007-10-18
         * select strftime('%Y.%m.%d %H:%M:%S','now','localtime'); res:2006.10.17 21:41:09
         */

        /*
         * abs()
         * random(*)
         * round(x[,y])
         * length()
         * lower()
         * upper()
         * substr(x,y,z) 
         * like 
         * select "a"+"b"  res:0
         * select "a"+1  res:1
         * select "a"+"1"  res:1
         * select 2+1  res:3
         */

        public static ExpressionClip DateTime(this SelectSqlSection criteria)
        {
            var newexpr = new ExpressionClip("datetime()", System.Data.DbType.DateTime);          
            return newexpr;
        }

        public static ExpressionClip DateTimeNow(this SelectSqlSection criteria)
        {
            var newexpr = new ExpressionClip("datetime('now')", System.Data.DbType.DateTime);
            return newexpr;
        }

        public static ExpressionClip DateTime(this ExpressionClip pars,string[] format)
        {
            var newexpr = (ExpressionClip) pars.Clone();
            StringBuilder sb = new StringBuilder("datetime(" + pars.Sql);
            foreach (var s in format)
            {
                sb.Append(",?");
                newexpr.ChildExpressions.Add(new ParameterExpression(s, System.Data.DbType.String));
            }
            sb.Append(",'localtime')");
            newexpr.Sql = sb.ToString();
            return newexpr;
        }

        public static ExpressionClip Date(this SelectSqlSection criteria)
        {
            return new ExpressionClip("date()", System.Data.DbType.DateTime);
        }

        public static ExpressionClip DateNow(this SelectSqlSection criteria)
        {
            return new ExpressionClip("date('now')", System.Data.DbType.DateTime);
        }

        public static ExpressionClip Date(this ExpressionClip pars, string[] format)
        {
            var newexpr = (ExpressionClip)pars.Clone();
            StringBuilder sb = new StringBuilder("date(" + pars.Sql);
            foreach (var s in format)
            {
                sb.Append(",?");
                newexpr.ChildExpressions.Add(new ParameterExpression(s, System.Data.DbType.String));
            }
            sb.Append(")");
            newexpr.Sql = sb.ToString();
            return newexpr;
        }

        public static ExpressionClip Time(this SelectSqlSection criteria)
        {
            return new ExpressionClip("time()", System.Data.DbType.DateTime);
        }

        public static ExpressionClip TimeNow(this SelectSqlSection criteria)
        {
            return new ExpressionClip("time('now')", System.Data.DbType.DateTime);
        }

        public static ExpressionClip Time(this ExpressionClip pars, string[] format)
        {
            var newexpr = (ExpressionClip)pars.Clone();
            StringBuilder sb = new StringBuilder("time(" + pars.Sql);
            foreach (var s in format)
            {
                sb.Append(",?");
                newexpr.ChildExpressions.Add(new ParameterExpression(s, System.Data.DbType.String));
            }
            sb.Append(")");
            newexpr.Sql = sb.ToString();
            return newexpr;
        }

        public static ExpressionClip StrfTime(this ExpressionClip pars, string format, string[] parms)
        {
            var newexpr = (ExpressionClip)pars.Clone();
            StringBuilder sb = new StringBuilder("strftime(?," + pars.Sql);
            foreach (var s in format)
            {
                sb.Append(",?");
                newexpr.ChildExpressions.Add(new ParameterExpression(s, System.Data.DbType.String));
            }
            sb.Append(")");
            newexpr.Sql = sb.ToString();
            return newexpr; 
        }

        public static ExpressionClip Random(this SelectSqlSection criteria)
        {
            return new ExpressionClip("random(*)", System.Data.DbType.Single);
        }

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

        public static ExpressionClip Length(this ExpressionClip expr)
        {
            return new ExpressionClip("length(" + expr.Sql + ")", System.Data.DbType.Int32,
                                      ((ExpressionClip)expr.Clone()).ChildExpressions);
        }

        public static ExpressionClip Lower(this ExpressionClip expr)
        {
            return new ExpressionClip("lower(" + expr.Sql + ")", System.Data.DbType.String,
                                      ((ExpressionClip)expr.Clone()).ChildExpressions);
        }

        public static ExpressionClip Upper(this ExpressionClip expr)
        {
            return new ExpressionClip("upper(" + expr.Sql + ")", System.Data.DbType.String,
                                      ((ExpressionClip)expr.Clone()).ChildExpressions);
        }

        public static ExpressionClip Substring(this ExpressionClip expr, int begin, int length)
        {

            var newExpr = (ExpressionClip)expr.Clone();

            newExpr.Sql = "substr(" + newExpr.Sql + ", ?+1, ?)";
            newExpr.ChildExpressions.Add(new ParameterExpression(begin, System.Data.DbType.Int32));
            newExpr.ChildExpressions.Add(new ParameterExpression(length, System.Data.DbType.Int32));

            return newExpr;
        }

        public static ExpressionClip Substring(this ExpressionClip expr, ExpressionClip begin, int length)
        {

            var newExpr = (ExpressionClip)expr.Clone();

            newExpr.Sql = "substr(" + newExpr.Sql + ", ?+1, ?)";
            newExpr.ChildExpressions.Add(begin);
            newExpr.ChildExpressions.Add(new ParameterExpression(length, System.Data.DbType.Int32));

            return newExpr;
        }

        public static ExpressionClip Substring(this ExpressionClip expr, int begin, ExpressionClip length)
        {

            var newExpr = (ExpressionClip)expr.Clone();

            newExpr.Sql = "substr(" + newExpr.Sql + ", ?+1, ?)";
            newExpr.ChildExpressions.Add(new ParameterExpression(begin, System.Data.DbType.Int32));
            newExpr.ChildExpressions.Add(length);

            return newExpr;
        }

        public static ExpressionClip Substring(this ExpressionClip expr, ExpressionClip begin, ExpressionClip length)
        {

            var newExpr = (ExpressionClip)expr.Clone();

            newExpr.Sql = "substr(" + newExpr.Sql + ", ?+1, ?)";
            newExpr.ChildExpressions.Add(begin);
            newExpr.ChildExpressions.Add(length);             
            return newExpr;
        }
    }
}
