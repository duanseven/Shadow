using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Runtime.Serialization;
using System.Text;
using NSun.Data.Configuration;
using NSun.Data.Lambda;

namespace NSun.Data
{
    [Serializable]
    [DataContract(Namespace = "http://nsun-shadow.com")]
    [KnownType("KnownTypes")]
    public class ExpressionClip : Expression
    {  
        #region Construction

        internal ExpressionClip()
        {            
            Sql = string.Empty;
        }
 
        public ExpressionClip(string sql, DbType dbtype, IEnumerable<IExpression> childExpressions)
            : base(sql, dbtype, childExpressions) { }

        public ExpressionClip(string sql, DbType dbtype)
            : base(sql, dbtype, null) { }

        #endregion

        #region Equals & Operator

        public Condition EqualsObject(object value)
        {
            return new Condition(this, ExpressionOperator.Equals, new ParameterExpression(value, DataType));
        }

        public Condition Equals(ExpressionClip value)
        {
            return new Condition(this, ExpressionOperator.Equals, (IExpression)value ?? NullExpression.Value);
        }

        public Condition NotEquals(object value)
        {
            return new Condition(this, ExpressionOperator.NotEquals, new ParameterExpression(value, DataType));
        }

        public Condition NotEquals(ExpressionClip value)
        {
            return new Condition(this, ExpressionOperator.NotEquals, (IExpression)value ?? NullExpression.Value);
        }

        public static Condition operator ==(ExpressionClip left, ExpressionClip right)
        {
            if (ReferenceEquals(right, null))
            {
                return new Condition(left, ExpressionOperator.Is, NullExpression.Value);
            }
            if (ReferenceEquals(left, null))
            {
                return new Condition(right, ExpressionOperator.Is, NullExpression.Value);
            }
            return new Condition(left, ExpressionOperator.Equals, right);
        }

        public static Condition operator !=(ExpressionClip left, ExpressionClip right)
        {
            if (ReferenceEquals(right, null))
            {
                return new Condition(left, ExpressionOperator.IsNot, NullExpression.Value);
            }
            if (ReferenceEquals(left, null))
            {
                return new Condition(right, ExpressionOperator.IsNot, NullExpression.Value);
            }
            return new Condition(left, ExpressionOperator.NotEquals, right);
        }

        public static Condition operator ==(ExpressionClip left, object right)
        {
            return new Condition(left, ExpressionOperator.Equals, new ParameterExpression(right, left.DataType));
        }

        public static Condition operator !=(ExpressionClip left, object right)
        {
            return new Condition(left, ExpressionOperator.NotEquals, new ParameterExpression(right, left.DataType));
        }

        public static Condition operator ==(object left, ExpressionClip right)
        {
            return new Condition(new ParameterExpression(left, right.DataType), ExpressionOperator.Equals, right);
        }

        public static Condition operator !=(object left, ExpressionClip right)
        {
            return new Condition(new ParameterExpression(left, right.DataType), ExpressionOperator.NotEquals, right);
        }

        #endregion

        #region Bitwise

        public ExpressionClip BitwiseAnd(object value)
        {
            var expr = (ExpressionClip)Clone();

            expr.Sql += " " + ExpressionHelper.ToString(ExpressionOperator.BitwiseAnd) + " ?";
            expr.ChildExpressions.Add(new ParameterExpression(value, DataType));

            return expr;
        }

        public ExpressionClip BitwiseAnd(ExpressionClip value)
        {
            if (ReferenceEquals(value, null))
                throw new ArgumentNullException("value");

            var expr = (ExpressionClip)Clone();

            expr.Sql += " " + ExpressionHelper.ToString(ExpressionOperator.BitwiseAnd) + " ?";
            expr.ChildExpressions.Add(value);

            return expr;
        }

        public ExpressionClip BitwiseOr(object value)
        {
            var expr = (ExpressionClip)Clone();

            expr.Sql += " " + ExpressionHelper.ToString(ExpressionOperator.BitwiseOr) + " ?";
            expr.ChildExpressions.Add(new ParameterExpression(value, DataType));

            return expr;
        }

        public ExpressionClip BitwiseOr(ExpressionClip value)
        {
            if (ReferenceEquals(value, null))
                throw new ArgumentNullException("value");

            var expr = (ExpressionClip)Clone();

            expr.Sql += " " + ExpressionHelper.ToString(ExpressionOperator.BitwiseOr) + " ?";
            expr.ChildExpressions.Add(value);

            return expr;
        }

        public ExpressionClip BitwiseXor(object value)
        {
            var expr = (ExpressionClip)Clone();

            expr.Sql += " " + ExpressionHelper.ToString(ExpressionOperator.BitwiseXor) + " ?";
            expr.ChildExpressions.Add(new ParameterExpression(value, DataType));

            return expr;
        }

        public ExpressionClip BitwiseXor(ExpressionClip value)
        {
            if (ReferenceEquals(value, null))
                throw new ArgumentNullException("value");

            var expr = (ExpressionClip)Clone();

            expr.Sql += " " + ExpressionHelper.ToString(ExpressionOperator.BitwiseXor) + " ?";
            expr.ChildExpressions.Add(value);

            return expr;
        }

        public ExpressionClip BitwiseNot()
        {
            var expr = (ExpressionClip)Clone();

            expr.Sql = ExpressionHelper.ToString(ExpressionOperator.BitwiseNot) + "(" + expr.Sql + ")";

            return expr;
        }

        #endregion

        #region Between

        public Condition Between(object left, object right, bool includeLeft, bool includeRight)
        {
            ExpressionOperator leftOp;
            ExpressionOperator rightOp;
            ExpressionHelper.GetLeftRightOperatorsForBetween(includeLeft, includeRight, out leftOp, out rightOp);

            var leftCondition = new Condition(this, leftOp, new ParameterExpression(left, DataType));
            var rightCondition = new Condition(this, rightOp, new ParameterExpression(right, DataType));
            return leftCondition.And(rightCondition);
        }

        public Condition Between(ExpressionClip left, object right, bool includeLeft, bool includeRight)
        {
            if (left == null)
                throw new ArgumentNullException("left");

            ExpressionOperator leftOp;
            ExpressionOperator rightOp;
            ExpressionHelper.GetLeftRightOperatorsForBetween(includeLeft, includeRight, out leftOp, out rightOp);

            var leftCondition = new Condition(this, leftOp, left);
            var rightCondition = new Condition(this, rightOp, new ParameterExpression(right, DataType));
            return leftCondition.And(rightCondition);
        }

        public Condition Between(object left, ExpressionClip right, bool includeLeft, bool includeRight)
        {
            if (right == null)
                throw new ArgumentNullException("right");

            ExpressionOperator leftOp;
            ExpressionOperator rightOp;
            ExpressionHelper.GetLeftRightOperatorsForBetween(includeLeft, includeRight, out leftOp, out rightOp);

            var leftCondition = new Condition(this, leftOp, new ParameterExpression(left, DataType));
            var rightCondition = new Condition(this, rightOp, right);
            return leftCondition.And(rightCondition);
        }

        public Condition Between(ExpressionClip left, ExpressionClip right, bool includeLeft, bool includeRight)
        {
            if (left == null)
                throw new ArgumentNullException("left");
            if (right == null)
                throw new ArgumentNullException("right");

            ExpressionOperator leftOp;
            ExpressionOperator rightOp;
            ExpressionHelper.GetLeftRightOperatorsForBetween(includeLeft, includeRight, out leftOp, out rightOp);

            var leftCondition = new Condition(this, leftOp, left);
            var rightCondition = new Condition(this, rightOp, right);
            return leftCondition.And(rightCondition);
        }

        #endregion

        #region GreaterThan & LessThan

        public Condition GreaterThan(byte value)
        {
            return new Condition(this, ExpressionOperator.GreaterThan, new ParameterExpression(value, DataType));
        }

        public Condition GreaterThan(ExpressionClip value)
        {
            return new Condition(this, ExpressionOperator.GreaterThan, value);
        }

        public Condition LessThan(byte value)
        {
            return new Condition(this, ExpressionOperator.LessThan, new ParameterExpression(value, DataType));
        }

        public Condition LessThan(ExpressionClip value)
        {
            return new Condition(this, ExpressionOperator.LessThan, value);
        }

        public static Condition operator >(ExpressionClip left, ExpressionClip right)
        {
            if (ReferenceEquals(right, null))
            {
                return new Condition(left, ExpressionOperator.GreaterThan, NullExpression.Value);
            }
            if (ReferenceEquals(left, null))
            {
                return new Condition(NullExpression.Value, ExpressionOperator.GreaterThan, right);
            }
            return new Condition(left, ExpressionOperator.GreaterThan, right);
        }

        public static Condition operator <(ExpressionClip left, ExpressionClip right)
        {
            if (ReferenceEquals(right, null))
            {
                return new Condition(left, ExpressionOperator.LessThan, NullExpression.Value);
            }
            if (ReferenceEquals(left, null))
            {
                return new Condition(NullExpression.Value, ExpressionOperator.LessThan, right);
            }
            return new Condition(left, ExpressionOperator.LessThan, right);
        }

        public static Condition operator >(ExpressionClip left, object right)
        {
            return new Condition(left, ExpressionOperator.GreaterThan, new ParameterExpression(right, left.DataType));
        }

        public static Condition operator <(ExpressionClip left, object right)
        {
            return new Condition(left, ExpressionOperator.LessThan, new ParameterExpression(right, left.DataType));
        }

        public static Condition operator >(object left, ExpressionClip right)
        {
            return new Condition(new ParameterExpression(left, right.DataType), ExpressionOperator.GreaterThan, right);
        }

        public static Condition operator <(object left, ExpressionClip right)
        {
            return new Condition(new ParameterExpression(left, right.DataType), ExpressionOperator.LessThan, right);
        }

        #endregion

        #region GreaterThanOrEquals & LessThanOrEquals

        public Condition GreaterThanOrEquals(object value)
        {
            return new Condition(this, ExpressionOperator.GreaterThanOrEquals, new ParameterExpression(value, DataType));
        }

        public Condition GreaterThanOrEquals(ExpressionClip value)
        {
            return new Condition(this, ExpressionOperator.GreaterThanOrEquals, value);
        }

        public Condition LessThanOrEquals(object value)
        {
            return new Condition(this, ExpressionOperator.LessThanOrEquals, new ParameterExpression(value, DataType));
        }

        public Condition LessThanOrEquals(ExpressionClip value)
        {
            return new Condition(this, ExpressionOperator.LessThanOrEquals, value);
        }

        public static Condition operator >=(ExpressionClip left, ExpressionClip right)
        {
            if (ReferenceEquals(right, null))
            {
                return new Condition(left, ExpressionOperator.GreaterThanOrEquals, NullExpression.Value);
            }
            if (ReferenceEquals(left, null))
            {
                return new Condition(NullExpression.Value, ExpressionOperator.GreaterThanOrEquals, right);
            }
            return new Condition(left, ExpressionOperator.GreaterThanOrEquals, right);
        }

        public static Condition operator <=(ExpressionClip left, ExpressionClip right)
        {
            if (ReferenceEquals(right, null))
            {
                return new Condition(left, ExpressionOperator.LessThanOrEquals, NullExpression.Value);
            }
            if (ReferenceEquals(left, null))
            {
                return new Condition(NullExpression.Value, ExpressionOperator.LessThanOrEquals, right);
            }
            return new Condition(left, ExpressionOperator.LessThanOrEquals, right);
        }

        public static Condition operator >=(ExpressionClip left, object right)
        {
            return new Condition(left, ExpressionOperator.GreaterThanOrEquals, new ParameterExpression(right, left.DataType));
        }

        public static Condition operator <=(ExpressionClip left, object right)
        {
            return new Condition(left, ExpressionOperator.LessThanOrEquals, new ParameterExpression(right, left.DataType));
        }

        public static Condition operator >=(object left, ExpressionClip right)
        {
            return new Condition(new ParameterExpression(left, right.DataType), ExpressionOperator.GreaterThanOrEquals, right);
        }

        public static Condition operator <=(object left, ExpressionClip right)
        {
            return new Condition(new ParameterExpression(left, right.DataType), ExpressionOperator.LessThanOrEquals, right);
        }

        #endregion

        #region + - * / %

        public ExpressionClip Add(object value)
        {
            var expr = (ExpressionClip)Clone();

            expr.Sql += " " + ExpressionHelper.ToString(ExpressionOperator.Add) + " ?";
            expr.ChildExpressions.Add(new ParameterExpression(value, DataType));

            return expr;
        }

        public ExpressionClip Add(ExpressionClip value)
        {
            var expr = (ExpressionClip)Clone();

            expr.Sql += " " + ExpressionHelper.ToString(ExpressionOperator.Add) + " ?";
            expr.ChildExpressions.Add(ReferenceEquals(value, null) ? NullExpression.Value : (IExpression)value);

            return expr;
        }

        public ExpressionClip Subtract(object value)
        {
            var expr = (ExpressionClip)Clone();

            expr.Sql += " " + ExpressionHelper.ToString(ExpressionOperator.Subtract) + " ?";
            expr.ChildExpressions.Add(new ParameterExpression(value, DataType));

            return expr;
        }

        public ExpressionClip Subtract(ExpressionClip value)
        {
            var expr = (ExpressionClip)Clone();

            expr.Sql += " " + ExpressionHelper.ToString(ExpressionOperator.Subtract) + " ?";
            expr.ChildExpressions.Add(ReferenceEquals(value, null) ? NullExpression.Value : (IExpression)value);

            return expr;
        }

        public ExpressionClip Multiply(object value)
        {
            var expr = (ExpressionClip)Clone();

            expr.Sql += " " + ExpressionHelper.ToString(ExpressionOperator.Multiply) + " ?";
            expr.ChildExpressions.Add(new ParameterExpression(value, DataType));

            return expr;
        }

        public ExpressionClip Multiply(ExpressionClip value)
        {
            var expr = (ExpressionClip)Clone();

            expr.Sql += " " + ExpressionHelper.ToString(ExpressionOperator.Multiply) + " ?";
            expr.ChildExpressions.Add(ReferenceEquals(value, null) ? NullExpression.Value : (IExpression)value);

            return expr;
        }

        public ExpressionClip Divide(object value)
        {
            var expr = (ExpressionClip)Clone();

            expr.Sql += " " + ExpressionHelper.ToString(ExpressionOperator.Divide) + " ?";
            expr.ChildExpressions.Add(new ParameterExpression(value, DataType));

            return expr;
        }

        public ExpressionClip Divide(ExpressionClip value)
        {
            var expr = (ExpressionClip)Clone();

            expr.Sql += " " + ExpressionHelper.ToString(ExpressionOperator.Divide) + " ?";
            expr.ChildExpressions.Add(ReferenceEquals(value, null) ? NullExpression.Value : (IExpression)value);

            return expr;
        }

        public ExpressionClip Mod(object value)
        {
            var expr = (ExpressionClip)Clone();

            expr.Sql += " " + ExpressionHelper.ToString(ExpressionOperator.Mod) + " ?";
            expr.ChildExpressions.Add(new ParameterExpression(value, DataType));

            return expr;
        }

        public ExpressionClip Mod(ExpressionClip value)
        {
            var expr = (ExpressionClip)Clone();

            expr.Sql += " " + ExpressionHelper.ToString(ExpressionOperator.Mod) + " ?";
            expr.ChildExpressions.Add(ReferenceEquals(value, null) ? NullExpression.Value : (IExpression)value);

            return expr;
        }

        public static ExpressionClip operator +(ExpressionClip left, ExpressionClip right)
        {
            var expr = (ExpressionClip)left.Clone();

            expr.Sql += " " + ExpressionHelper.ToString(ExpressionOperator.Add) + " ?";
            expr.ChildExpressions.Add(right);

            return expr;
        }

        public static ExpressionClip operator +(object left, ExpressionClip right)
        {
            var expr = new ParameterExpression(left, right.DataType);

            expr.Sql += " " + ExpressionHelper.ToString(ExpressionOperator.Add) + " ?";
            expr.ChildExpressions.Add(right);

            return expr;
        }

        public static ExpressionClip operator +(ExpressionClip left, object right)
        {
            var expr = (ExpressionClip)left.Clone();

            expr.Sql += " " + ExpressionHelper.ToString(ExpressionOperator.Add) + " ?";
            expr.ChildExpressions.Add(new ParameterExpression(right, left.DataType));

            return expr;
        }

        public static ExpressionClip operator -(ExpressionClip left, ExpressionClip right)
        {
            var expr = (ExpressionClip)left.Clone();

            expr.Sql += " " + ExpressionHelper.ToString(ExpressionOperator.Subtract) + " ?";
            expr.ChildExpressions.Add(right);

            return expr;
        }

        public static ExpressionClip operator -(object left, ExpressionClip right)
        {
            var expr = new ParameterExpression(left, right.DataType);

            expr.Sql += " " + ExpressionHelper.ToString(ExpressionOperator.Subtract) + " ?";
            expr.ChildExpressions.Add(right);

            return expr;
        }

        public static ExpressionClip operator -(ExpressionClip left, object right)
        {
            var expr = (ExpressionClip)left.Clone();

            expr.Sql += " " + ExpressionHelper.ToString(ExpressionOperator.Subtract) + " ?";
            expr.ChildExpressions.Add(new ParameterExpression(right, left.DataType));

            return expr;
        }

        public static ExpressionClip operator *(ExpressionClip left, ExpressionClip right)
        {
            var expr = (ExpressionClip)left.Clone();

            expr.Sql += " " + ExpressionHelper.ToString(ExpressionOperator.Multiply) + " ?";
            expr.ChildExpressions.Add(right);

            return expr;
        }

        public static ExpressionClip operator *(object left, ExpressionClip right)
        {
            var expr = new ParameterExpression(left, right.DataType);

            expr.Sql += " " + ExpressionHelper.ToString(ExpressionOperator.Multiply) + " ?";
            expr.ChildExpressions.Add(right);

            return expr;
        }

        public static ExpressionClip operator *(ExpressionClip left, object right)
        {
            var expr = (ExpressionClip)left.Clone();

            expr.Sql += " " + ExpressionHelper.ToString(ExpressionOperator.Multiply) + " ?";
            expr.ChildExpressions.Add(new ParameterExpression(right, left.DataType));

            return expr;
        }

        public static ExpressionClip operator /(ExpressionClip left, ExpressionClip right)
        {
            var expr = (ExpressionClip)left.Clone();

            expr.Sql += " " + ExpressionHelper.ToString(ExpressionOperator.Divide) + " ?";
            expr.ChildExpressions.Add(right);

            return expr;
        }

        public static ExpressionClip operator /(object left, ExpressionClip right)
        {
            var expr = new ParameterExpression(left, right.DataType);

            expr.Sql += " " + ExpressionHelper.ToString(ExpressionOperator.Divide) + " ?";
            expr.ChildExpressions.Add(right);

            return expr;
        }

        public static ExpressionClip operator /(ExpressionClip left, object right)
        {
            var expr = (ExpressionClip)left.Clone();

            expr.Sql += " " + ExpressionHelper.ToString(ExpressionOperator.Divide) + " ?";
            expr.ChildExpressions.Add(new ParameterExpression(right, left.DataType));

            return expr;
        }

        public static ExpressionClip operator %(ExpressionClip left, ExpressionClip right)
        {
            var expr = (ExpressionClip)left.Clone();

            expr.Sql += " " + ExpressionHelper.ToString(ExpressionOperator.Mod) + " ?";
            expr.ChildExpressions.Add(right);

            return expr;
        }

        public static ExpressionClip operator %(object left, ExpressionClip right)
        {
            var expr = new ParameterExpression(left, right.DataType);
            expr.Sql += " " + ExpressionHelper.ToString(ExpressionOperator.Mod) + " ?";
            expr.ChildExpressions.Add(right);
            return expr;
        }

        public static ExpressionClip operator %(ExpressionClip left, object right)
        {
            var expr = (ExpressionClip)left.Clone();
            expr.Sql += " " + ExpressionHelper.ToString(ExpressionOperator.Mod) + " ?";
            expr.ChildExpressions.Add(new ParameterExpression(right, left.DataType));

            return expr;
        }  

        #endregion

        #region Aggregation

        public ExpressionClip Avg()
        {
            var expr = (ExpressionClip)Clone();
            expr.Sql = "AVG(" + expr.Sql + ")";
            return expr;
        }

        public ExpressionClip Max()
        {
            var expr = (ExpressionClip)Clone();
            expr.Sql = "MAX(" + expr.Sql + ")";
            return expr;
        }

        public ExpressionClip Min()
        {
            var expr = (ExpressionClip)Clone();
            expr.Sql = "MIN(" + expr.Sql + ")";
            return expr;
        }

        public ExpressionClip Sum()
        {
            var expr = (ExpressionClip)Clone();
            expr.Sql = "SUM(" + expr.Sql + ")";
            return expr;
        }

        public ExpressionClip Length()
        {
            var expr = (ExpressionClip)Clone();
            expr.Sql = "LEN(" + expr.Sql + ")";
            return expr;
        }

        #endregion

        #region String Operations
         
        public Condition Like(string value)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentNullException("value");

            return new Condition(this, ExpressionOperator.Like, new ParameterExpression(value, DataType));
        }

        public Condition Escape(string escape)
        {
            if (string.IsNullOrEmpty(escape))
                throw new ArgumentNullException("escape");

            return new Condition(this, ExpressionOperator.Escape, new ExpressionClip(escape, DbType.String));
        }

        public Condition Contains(string subString)
        {
            return new Condition(this, ExpressionOperator.Like,
                                       new ParameterExpression(
                                           '%' + subString.Replace("%", "[%]").Replace("_", "[_]") + '%', DataType));
        }

        public Condition StartsWith(string prefix)
        {
            return new Condition(this, ExpressionOperator.Like,
                                       new ParameterExpression(
                                            prefix.Replace("%", "[%]").Replace("_", "[_]") + '%', DataType));
        }

        public Condition EndsWith(string suffix)
        {
            return new Condition(this, ExpressionOperator.Like,
                                 new ParameterExpression(
                                     '%' + suffix.Replace("%", "[%]").Replace("_", "[_]"), DataType));
        }

        public Condition Like(ExpressionClip value)
        {
            if (ReferenceEquals(value, null))
                throw new ArgumentNullException("value");

            return new Condition(this, ExpressionOperator.Like, value);
        }

        public ExpressionClip ToLower()
        {
            var expr = (ExpressionClip)Clone();

            expr.Sql = "LOWER(" + expr.Sql + ")";

            return expr;
        }

        public ExpressionClip ToUpper()
        {
            var expr = (ExpressionClip)Clone();

            expr.Sql = "UPPER(" + expr.Sql + ")";

            return expr;
        }

        public ExpressionClip Trim()
        {
            var expr = (ExpressionClip)Clone();

            expr.Sql = "TRIM(" + expr.Sql + ")";

            return expr;
        }
         
        #endregion

        #region Other 

        public ExpressionClip CustomMethodExtensions(System.Linq.Expressions.MethodCallExpression value)
        {
            if (value.Arguments.Count == 5)
            {
                if (value.Arguments[2].ExpressionToObject() is Boolean)
                {
                    var returndbtype = (DbType)value.Arguments[1].ExpressionToObject();
                    var isColumninMethod = (bool)value.Arguments[2].ExpressionToObject();
                    var methodName = value.Arguments[3].ExpressionToObject().ToString();
                    var pars = (object[])value.Arguments[4].ExpressionToObject();
                    var lf = CustomMethodExtensions(returndbtype, isColumninMethod, methodName, pars);
                    return lf;
                }
                else
                {
                    var returndbtype = (DbType)value.Arguments[1].ExpressionToObject();
                    var dboName = value.Arguments[2].ExpressionToObject().ToString();
                    var methodName = value.Arguments[3].ExpressionToObject().ToString();
                    var pars = (object[])value.Arguments[4].ExpressionToObject();
                    var lf = CustomMethodExtensions(returndbtype, dboName, methodName, pars);
                    return lf;
                }
            }
            else if (value.Arguments.Count == 6)
            {
                var returndbtype = (DbType)value.Arguments[1].ExpressionToObject();
                var dboName = value.Arguments[2].ExpressionToObject().ToString();
                var methodName = value.Arguments[3].ExpressionToObject().ToString();
                var isColumninMethod = (bool)value.Arguments[4].ExpressionToObject();
                var pars = (object[])value.Arguments[5].ExpressionToObject();
                var lf = CustomMethodExtensions(returndbtype, dboName, methodName, isColumninMethod, pars);
                return lf;
            }
            return null;
        }

        public ExpressionClip As(string columnName)
        {
            if (string.IsNullOrEmpty(columnName))
                throw new ArgumentNullException("columnName");

            var column = this as QueryColumn;
            if (!ReferenceEquals(column, null))
            {
                var asColumn = (QueryColumn)column.Clone();
                asColumn.ColumnName = columnName;
                return asColumn;
            }
            return new QueryColumn(this, columnName, DataType);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public Condition In(IEnumerable values)
        {
            var expr = new ExpressionCollection();
            if (values != null)
            {
                foreach (var value in values)
                {
                    expr.Add(new ParameterExpression(value, DataType));
                }
            }
            if (((ICollection<IExpression>)expr).Count == 0)
                throw new ArgumentNullException("values");
            return new Condition(this, ExpressionOperator.In, expr);
        }

        public Condition In(ExpressionClip exprs)
        {
            return new Condition(this, ExpressionOperator.In, exprs);
        }

        public ExpressionClip Count()
        {
            return Count(false);
        }

        public ExpressionClip Count(bool distinct)
        {
            var countExpr = (ExpressionClip)this.Clone();
            if (distinct)
                countExpr.Sql = "COUNT(DISTINCT " + countExpr.Sql + ")";
            else
                countExpr.Sql = "COUNT(" + countExpr.Sql + ")";
            return countExpr;
        } 

        public Condition NotIn(IEnumerable values)
        {
            return In(values).Not();
        }

        public Condition NotIn(ExpressionClip exprs)
        {
            return In(exprs).Not();
        }

        protected override object CreateInstance()
        {
            return new ExpressionClip();
        }

        public SubQuery ToSubQuery()
        {
            return new SubQuery(Sql);
        }

        #endregion

        #region CustomdboMethod

        /// <summary>
        /// default dboName=dbo
        /// </summary>
        /// <param name="returndbType"></param>
        /// <param name="methodName"></param>
        /// <param name="pars"></param>
        /// <returns></returns>
        public ExpressionClip CustomMethodExtensions(DbType returndbType, bool isSysMethod, string methodName, params object[] pars)
        {
            if (!isSysMethod)
                return CustomMethodExtensions(returndbType, "dbo", methodName, true, pars);
            return CustomMethodExtensions(returndbType, string.Empty, methodName, true, pars);
        }

        public ExpressionClip CustomMethodExtensions(DbType returndbType, string dboName, string methodName, params object[] pars)
        {
            return CustomMethodExtensions(returndbType, dboName, methodName, true, pars);
        }

        public ExpressionClip CustomMethodExtensions(DbType returndbType, string dboName, string methodName, bool isColumninMethod, params object[] pars)
        {
            var _expr = (ExpressionClip)this.Clone();
            _expr.DataType = returndbType;
            _expr.Sql = ColumnClipExtensions(_expr.Sql, dboName, isColumninMethod, methodName, pars);
            return _expr;
        }

        internal string ColumnClipExtensions(string columnName, string dboName, bool isColumninMethod, string dboMethod, params object[] pars)
        {
            var sb = new StringBuilder();
            if (dboName.Trim().Length != 0)
                sb = new StringBuilder(dboName + ".");
            sb.Append(dboMethod);
            sb.Append('(');
            if (isColumninMethod)
            {
                sb.Append(columnName);
            }
            if (pars!=null&&pars.Length > 0)
            {
                sb.Append(',');
                foreach (object item in pars)
                {
                    sb.AppendFormat("{0},", item);
                }
                sb = new StringBuilder(sb.ToString().TrimEnd(','));
            }
            sb.Append(')');
            return sb.ToString();
        }
        #endregion

        #region Orderby

        public OrderByClip Desc
        {
            get { return new OrderByClip(this, true); }
        }
        public OrderByClip Asc
        {
            get { return new OrderByClip(this, false); }
        }
         
        #endregion

        #region KnownTypes

        static Type[] KnownTypes()
        {
            return KnownTypeRegistry.Instance.KnownTypes;
        }

        #endregion
    }
}
