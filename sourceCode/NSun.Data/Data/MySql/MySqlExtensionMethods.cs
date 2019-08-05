using System;

namespace NSun.Data.MySql
{
    public static class MySqlExtensionMethods
    {

        //DATE_FORMAT(
        public static ExpressionClip DateFormat(this ExpressionClip expr, string datetype)
        {
            var newExpr = (ExpressionClip)expr.Clone();
            newExpr.Sql = "DATE_FORMAT(" + expr.Sql + ", ?)";
            newExpr.ChildExpressions.Add(new ParameterExpression(datetype, System.Data.DbType.String));
            return newExpr;
        }

        public static ExpressionClip Cast(this ExpressionClip expr, string datetype)
        {
            var newExpr = (ExpressionClip)expr.Clone();
            newExpr.Sql = string.Format("CAST(" + expr.Sql + "AS {0})", datetype);          
            return newExpr;
        }

        public static ExpressionClip ASCII(this ExpressionClip expr)
        {
            var newexpr = new ExpressionClip("ASCII(" + expr.Sql + ")", System.Data.DbType.Int32,
                                             ((ExpressionClip)expr.Clone()).ChildExpressions);
            return newexpr;
        }

        public static ExpressionClip Bin(this ExpressionClip expr)
        {
            var newexpr = new ExpressionClip("BIN(" + expr.Sql + ")", System.Data.DbType.String,
                                             ((ExpressionClip)expr.Clone()).ChildExpressions);
            return newexpr;
        }

        public static ExpressionClip Bit_Length(this ExpressionClip expr)
        {
            var newexpr = new ExpressionClip("BIT_LENGTH(" + expr.Sql + ")", System.Data.DbType.Int32,
                                             ((ExpressionClip)expr.Clone()).ChildExpressions);
            return newexpr;
        }

        public static ExpressionClip Char(this ExpressionClip expr)
        {
            var newexpr = new ExpressionClip("CHAR(" + expr.Sql + ")", System.Data.DbType.String,
                                             ((ExpressionClip)expr.Clone()).ChildExpressions);
            return newexpr;
        }

        public static ExpressionClip Lower(this ExpressionClip expr)
        {
            var newExpr = (ExpressionClip)expr.Clone();
            newExpr.Sql = "LOWER(" + expr.Sql + ")";
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

        #region Date/Time

        public static ExpressionClip GetCurrentDate(this SelectSqlSection criteria)
        {
            return new ExpressionClip("CURDATE()", System.Data.DbType.DateTime);
        }

        public static ExpressionClip GetCurrentTime(this SelectSqlSection criteria)
        {
            return new ExpressionClip("CURTIME()", System.Data.DbType.DateTime);
        }

        public static ExpressionClip GetCurrentUtcDate(this SelectSqlSection criteria)
        {
            return new ExpressionClip("UTC_DATE()", System.Data.DbType.DateTime);
        }

        public static ExpressionClip GetCurrentUtcTime(this SelectSqlSection criteria)
        {
            return new ExpressionClip("UTC_TIME()", System.Data.DbType.DateTime);
        }

        #endregion
    }
}
