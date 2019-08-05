using System;
namespace NSun.Data.DB2
{
    public static class DB2ExtensionMethods
    {

        public static ExpressionClip GetCurrentDate(this SelectSqlSection criteria)
        {
            return new ExpressionClip("CURRENT_TIMESTAMP", System.Data.DbType.DateTime);
        }

        public static ExpressionClip GetCurrentUtcDate(this SelectSqlSection criteria)
        {
            return new ExpressionClip("CURRENT_TIMESTAMP - CURRENT_TIMEZONE", System.Data.DbType.DateTime);
        }

        #region DateTime Expression

        public static ExpressionClip Day(this ExpressionClip expr)
        {
            return new ExpressionClip("DAY(" + expr.Sql + ")", System.Data.DbType.Int32, ((ExpressionClip)expr.Clone()).ChildExpressions);
        }

        public static ExpressionClip Month(this ExpressionClip expr)
        {
            return new ExpressionClip("MONTH(" + expr.Sql + ")", System.Data.DbType.Int32, ((ExpressionClip)expr.Clone()).ChildExpressions);
        }

        public static ExpressionClip Year(this ExpressionClip expr)
        {
            return new ExpressionClip("YEAR(" + expr.Sql + ")", System.Data.DbType.Int32, ((ExpressionClip)expr.Clone()).ChildExpressions);
        }


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
            var newExpr = new ExpressionClip("1+LOCATE(?, " + expr.Sql + ") - 1", System.Data.DbType.Int32, ((ExpressionClip)expr.Clone()).ChildExpressions);

            newExpr.ChildExpressions.Insert(0, new ParameterExpression(value, System.Data.DbType.String));

            return newExpr;
        }

        public static ExpressionClip Charindex(this ExpressionClip expr, ExpressionClip value)
        {
            var newExpr = new ExpressionClip("1+LOCATE(?, " + expr.Sql + ") - 1", System.Data.DbType.Int32, ((ExpressionClip)expr.Clone()).ChildExpressions);

            newExpr.ChildExpressions.Insert(0, value);

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

        public static ExpressionClip Length(this ExpressionClip expr)
        {
            return new ExpressionClip("LENGTH(" + expr.Sql + ")", System.Data.DbType.Int32,
                                      ((ExpressionClip)expr.Clone()).ChildExpressions);

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


        #endregion


        #endregion

    }
}
