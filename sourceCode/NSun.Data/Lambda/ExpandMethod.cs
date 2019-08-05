using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions; 
using Expression = System.Linq.Expressions.Expression;
using ParameterExpression = System.Linq.Expressions.ParameterExpression;

namespace NSun.Data.Lambda
{
    /// <summary>
    /// 扩展方法
    /// </summary>
    public static class ExpandMethod
    {
        /// <summary>
        /// 表达式转换为值
        /// </summary>
        /// <param name="ce"></param>
        /// <returns></returns>
        internal static object ExpressionToObject(this System.Linq.Expressions.Expression ce)
        {
            if (ce is UnaryExpression) //一元表达式
            {                
                object obj = ExpressionToObject(((UnaryExpression) (ce)).Operand);
                return obj.ToString();
            }
            if (ce is ConstantExpression) //常量
                return ((ConstantExpression)ce).Value;
            if (ce is MemberExpression) //属性值                
                return  System.Linq.Expressions.Expression.Lambda(ce).Compile().DynamicInvoke();
            if (ce is NewArrayExpression) //数组值 return object[];
            {
                var c = ce as NewArrayExpression;
                var list = c.Expressions.Select(ex => ex.ExpressionToObject()).ToList();
                object[] objs = list.Count > 0 ? list.ToArray() : null;
                return objs;
            }
            if (ce is MethodCallExpression)//执行方法结果
            {
                try
                {
                    return System.Linq.Expressions.Expression.Lambda(ce).Compile().DynamicInvoke();
                }
                catch (Exception e)
                {
                    throw new Exception("method invoke error", e);
                }
            }
            if (ce is ConditionalExpression)
            {
                return System.Linq.Expressions.Expression.Lambda(ce).Compile().DynamicInvoke();
            }
            //表达式方法内部计算 int? id = 21;
            //query.Where(p1 => p1.Name.Contains("1" + id + "1"));
            if (ce is BinaryExpression)//内部表达式
            {
                var c = ce as BinaryExpression;
                if (c.NodeType == ExpressionType.Coalesce) //这里id??1 直接返回右面还有左面
                {
                    return c.Left.ExpressionToObject() ?? c.Right.ExpressionToObject();
                }
                //method string join
                string left = c.Left.ExpressionToObject() == null ? string.Empty : c.Left.ExpressionToObject().ToString();
                string rigth = c.Right.ExpressionToObject() == null ? string.Empty : c.Right.ExpressionToObject().ToString();
                return left + rigth;
            }
          
            return null;
        }

        /// <summary>
        /// 表达式转换为名称
        /// </summary>
        /// <param name="ce">p=>p.Id  Get to "Id"</param>
        /// <returns></returns>
        internal static object ExpressionToName(this System.Linq.Expressions.Expression ce)
        {            
            return ce is UnaryExpression
                       ? ((UnaryExpression) (ce)).Operand.ExpressionToName()
                       : ((MemberExpression) ce).Member.Name;
        }

        #region Common

        internal static QueryColumn GetQueryColumn<T>(this System.Linq.Expressions.Expression bing) where T : class,IBaseEntity
        {            
            var me = GetMemberMethod(bing);
            //TODO 修改表达式
            if ((me != null))// && (me.Expression is ParameterExpression))
            {               
                return BaseDbQuery<T>.GetQueryColumn(me.Member.Name);
            }
            return null;
        }

        internal static QueryColumn GetQueryColumn<T>(string name) where T : class,IBaseEntity
        {
            QueryColumn left = BaseDbQuery<T>.GetQueryColumn(name);
            return left;
        }

        internal static MemberExpression GetMemberMethod(this System.Linq.Expressions.Expression bing)
        {
            MemberExpression me = null;
            if (bing is UnaryExpression)
            {
                var b = bing as UnaryExpression;
                me = GetMemberMethod(b.Operand);
            }
            else if (bing is LambdaExpression)
            {
                var lam = bing as LambdaExpression;
                me = lam.Body as MemberExpression;
            }
            else
            {
                me = bing as MemberExpression;
            } 
            return me;
        }

        internal static ConditionAndOr ConditionAndOrTypeCast(this ExpressionType type)
        {
            switch (type)
            {
                case ExpressionType.AndAlso:
                    return ConditionAndOr.And;
                case ExpressionType.OrElse:
                    return ConditionAndOr.Or;
                default:
                    return ConditionAndOr.And;
            }
        }

        internal static ExpressionOperator ExpressionTypeCast(this ExpressionType type)
        {
            switch (type)
            {                
                case ExpressionType.And:
                    return ExpressionOperator.BitwiseAnd; 
                case ExpressionType.Equal:
                    return ExpressionOperator.Equals;
                case ExpressionType.GreaterThan:
                    return ExpressionOperator.GreaterThan;
                case ExpressionType.GreaterThanOrEqual:
                    return ExpressionOperator.GreaterThanOrEquals;
                case ExpressionType.LessThan:
                    return ExpressionOperator.LessThan;
                case ExpressionType.LessThanOrEqual:
                    return ExpressionOperator.LessThanOrEquals;
                case ExpressionType.NotEqual:
                    return ExpressionOperator.NotEquals;
                case ExpressionType.Or:
                    return ExpressionOperator.BitwiseOr; 
                case ExpressionType.Add:
                case ExpressionType.AddChecked:
                    return ExpressionOperator.Add;
                case ExpressionType.Subtract:
                case ExpressionType.SubtractChecked:
                    return ExpressionOperator.Subtract;
                case ExpressionType.Divide:
                    return ExpressionOperator.Divide;
                case ExpressionType.Multiply:
                case ExpressionType.MultiplyChecked:
                    return ExpressionOperator.Multiply;
                case ExpressionType.Modulo:
                    return ExpressionOperator.Mod;
                case ExpressionType.ExclusiveOr:
                    return ExpressionOperator.BitwiseXor;
                case ExpressionType.Not:
                    return ExpressionOperator.BitwiseNot;                    
                default:
                    return ExpressionOperator.None;
            }
        }

        internal static List<object> NewArrayToObject(this NewArrayExpression ce)
        {
            return ce.Expressions.Select(ex => ex.ExpressionToObject()).ToList();
        }

        internal static QueryColumn GetColumnByMember<T, I>(this MemberExpression me)
            where T : class,IBaseEntity
            where I : class,IBaseEntity
        {
            return me.Member.ReflectedType.FullName == typeof(T).FullName ? GetQueryColumn<T>(me) : GetQueryColumn<I>(me);
        }

        #endregion
    } 
}
