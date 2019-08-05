using System;

namespace NSun.Data.Npgsql
{
    public static class NpgsqlExtensionMethods
    {
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

        #endregion

        public static ExpressionClip GetCurrentDate(this SelectSqlSection criteria)
        {
            return new ExpressionClip("current_timestamp", System.Data.DbType.DateTime);
        }

        public static ExpressionClip GetCurrentUTCDate(this SelectSqlSection criteria)
        {
            return new ExpressionClip("LOCALTIMESTAMP", System.Data.DbType.DateTime);
        }

        #region DateTime Expression

        public static ExpressionClip Day(this ExpressionClip expr)
        {
            return new ExpressionClip("date_part('day'" + expr.Sql + ")", System.Data.DbType.Int32, ((ExpressionClip)expr.Clone()).ChildExpressions);
        }

        public static ExpressionClip Month(this ExpressionClip expr)
        {
            return new ExpressionClip("date_part('month'," + expr.Sql + ")", System.Data.DbType.Int32, ((ExpressionClip)expr.Clone()).ChildExpressions);
        }

        public static ExpressionClip Year(this ExpressionClip expr)
        {
            return new ExpressionClip("date_part('year'," + expr.Sql + ")", System.Data.DbType.Int32, ((ExpressionClip)expr.Clone()).ChildExpressions);
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

        public static ExpressionClip Substring(this ExpressionClip expr, int begin, int length)
        {
            var newExpr = (ExpressionClip)expr.Clone();

            newExpr.Sql = "substr(" + newExpr.Sql + ", ? + 1, ?)";
            newExpr.ChildExpressions.Add(new ParameterExpression(begin, System.Data.DbType.Int32));
            newExpr.ChildExpressions.Add(new ParameterExpression(length, System.Data.DbType.Int32));

            return newExpr;
        }

        public static ExpressionClip Substring(this ExpressionClip expr, ExpressionClip begin, int length)
        {
            if (ReferenceEquals(begin, null))
                throw new ArgumentNullException("begin");

            var newExpr = (ExpressionClip)expr.Clone();

            newExpr.Sql = "substr(" + newExpr.Sql + ", ? + 1, ?)";
            newExpr.ChildExpressions.Add(begin);
            newExpr.ChildExpressions.Add(new ParameterExpression(length, System.Data.DbType.Int32));

            return newExpr;
        }

        public static ExpressionClip Substring(this ExpressionClip expr, int begin, ExpressionClip length)
        {
            if (ReferenceEquals(length, null))
                throw new ArgumentNullException("length");

            var newExpr = (ExpressionClip)expr.Clone();

            newExpr.Sql = "substr(" + newExpr.Sql + ", ? + 1, ?)";
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

            newExpr.Sql = "substr(" + newExpr.Sql + ", ? + 1, ?)";
            newExpr.ChildExpressions.Add(begin);
            newExpr.ChildExpressions.Add(length);

            return newExpr;
        }

        public static ExpressionClip Length(this ExpressionClip expr)
        {
            return new ExpressionClip("length(" + expr.Sql + ")", System.Data.DbType.Int32,
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
