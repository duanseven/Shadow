using System;
using System.Collections.Generic;
using System.Linq; 
using System.Linq.Expressions; 
using Expression = System.Linq.Expressions.Expression;

namespace NSun.Data.Lambda
{
    public static class ExpressionUtil
    {
        #region T

        internal static Condition Eval<T>(Expression<Func<T, bool>> ex) where T : class,IBaseEntity
        {
            var be = ex.Body as BinaryExpression;
            return be != null
                       ? BinarExpressionProvider<T>(be.Left, be.Right, be.NodeType)
                       : (ex.Body is MethodCallExpression ? ColumnFunction<T>((MethodCallExpression)ex.Body) : new Condition());
        }
        /// <summary>
        /// 解析表达式
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private static Condition BinarExpressionProvider<T>(System.Linq.Expressions.Expression left, System.Linq.Expressions.Expression right, ExpressionType type) where T : class,IBaseEntity
        {

            ExpressionClip li = ExpressionRouter<T>(left);
            ExpressionClip ri = ExpressionRouter<T>(right);

            var le = li as Condition;
            if (!ReferenceEquals(le, null))
            {
                ConditionAndOr op = type.ConditionAndOrTypeCast();
                le.LinkedConditionAndOrs.Add(op);
                le.LinkedConditions.Add((Condition)ri);
                return le;
            }
            else
            {
                ExpressionOperator op = type.ExpressionTypeCast();
                return new Condition(li, op, ri);
            }
        }
        /// <summary>
        /// 解析表达式左右
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="exp"></param>
        /// <returns></returns>
        private static ExpressionClip ExpressionRouter<T>(System.Linq.Expressions.Expression exp) where T : class,IBaseEntity
        {
            if (exp is BinaryExpression)
            {
                var be = ((BinaryExpression)exp);
                switch (be.NodeType)
                {
                    case ExpressionType.Coalesce:
                        return ExpressionRouter<T>(be.Left.ExpressionToObject() == null ? be.Right : be.Left);
                    case ExpressionType.ArrayIndex:
                        return
                            new NSun.Data.ParameterExpression(System.Linq.Expressions.Expression.Lambda(be).Compile().DynamicInvoke(),
                                                              ConvertListUtil.GetDbType(be.Type, true));
                }
                return BinarExpressionProvider<T>(be.Left, be.Right, be.NodeType);
            }
            if (exp is MemberExpression)
            {
                var me = ((MemberExpression)exp);
                if (me.Expression is ConstantExpression)
                {
                    var value = me.ExpressionToObject();
                    return
                        new NSun.Data.ParameterExpression(value, ConvertListUtil.GetDbType(exp.Type, true));
                }
                var querycolumn = exp.GetQueryColumn<T>();
                if (ReferenceEquals(querycolumn, null))
                {
                    var value = me.ExpressionToObject();
                    return new NSun.Data.ParameterExpression(value, ConvertListUtil.GetDbType(me.Type, true));
                }
                return querycolumn;
            }
            if (exp is MethodCallExpression)
            {
                var mce = (MethodCallExpression)exp;
                var methodname = mce.Method.Name;
                QueryColumn qc = mce.Arguments.Count == 0 ? null : mce.Arguments[0].GetQueryColumn<T>();
                return ReturnExpression<T>(mce, methodname, qc);
            }
            if (exp is ConstantExpression)
            {
                return
                    new NSun.Data.ParameterExpression(exp.ExpressionToObject(), ConvertListUtil.GetDbType(exp.Type, true));
            }
            if (exp is UnaryExpression)
            {
                if (exp.NodeType == ExpressionType.ArrayLength)
                {
                    return
                        new NSun.Data.ParameterExpression(System.Linq.Expressions.Expression.Lambda(exp).Compile().DynamicInvoke(), ConvertListUtil.GetDbType(exp.Type, true));
                }
                var ue = ((UnaryExpression)exp);
                return ExpressionRouter<T>(ue.Operand);
            }
            if (exp is ConditionalExpression)
            {
                var ce = (ConditionalExpression)exp;
                if (Convert.ToBoolean(ce.Test.ExpressionToObject()))
                {
                    return ExpressionRouter<T>(ce.IfTrue);
                }
                return ExpressionRouter<T>(ce.IfFalse);
            }
            return null;
        }
        /// <summary>
        /// 返回方法特定表达式
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="mce"></param>
        /// <param name="methodname"></param>
        /// <param name="qc"></param>
        /// <returns></returns>
        internal static ExpressionClip ReturnExpression<T>(MethodCallExpression mce, string methodname, QueryColumn qc) where T : class,IBaseEntity
        {
            if (ReferenceEquals(qc, null))
            {
                qc = mce.Object.GetQueryColumn<T>();
                if (ReferenceEquals(qc, null))
                {
                    return
                        new NSun.Data.ParameterExpression(mce.ExpressionToObject(),
                                                          ConvertListUtil.GetDbType(mce.Type, true));
                }
            }

            #region String
 
            if (methodname == "ToUpper")
            {
                return qc.ToUpper();
            }
            else if (methodname == "ToLower")
            {
                return qc.ToLower();
            }
            else if (methodname == "Trim")
            {
                return qc.Trim();
            } 

            #endregion
 
            #region Aggregation
 
            else if (methodname == "Count")
            {
                if (mce.Arguments.Count == 2)
                {
                    return qc.Count((bool)mce.Arguments[1].ExpressionToObject());
                }
                else
                    return qc.Count();
            }
            else if (methodname == "Sum")
            {
                return qc.Sum();
            }
            else if (methodname == "Min")
            {
                return qc.Min();
            }
            else if (methodname == "Max")
            {
                return qc.Max();
            }
            else if (methodname == "Avg")
            {
                return qc.Avg();
            }
            else if (methodname == "Length")
            {
                return qc.Length();
            }
            else if (methodname == "ToNumber")
            {
                return NSun.Data.OracleClient.OracleExtensionMethods.ToNumber(qc);
            }
            else if (methodname == "CustomMethodExtensions")
            {
                return qc.CustomMethodExtensions(mce);
            }
            #endregion

            //else
            //{
            //    new ExpressionClip("LEN(" + tag + ")", System.Data.DbType.String)
            //    List<object> list = new List<object>();
            //    foreach (var arg in mce.Arguments)
            //    {
            //        list.Add(arg.ExpressionToName());
            //    }

            //    return (ExpressionClip)mce.Method.Invoke(null, list.ToArray());
            //}

            return ColumnFunction<T>(mce);
        }

        /// <summary>
        /// 返回特定查询表达
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="exp"></param>
        /// <returns></returns>
        private static Condition ColumnFunction<T>(MethodCallExpression exp) where T : class,IBaseEntity
        {
            var mce = (MethodCallExpression)exp;
            QueryColumn qc = mce.Arguments[0].GetQueryColumn<T>();
            var methodname = mce.Method.Name;
            switch (methodname)
            {
                case "Like":
                    {
                        object value = mce.Arguments[1].ExpressionToObject();
                        return qc.Like(value.ToString());
                    }         
                case "Contains":
                case "Equals":
                case "StartsWith":
                case "EndsWith":
                    {
                        object value = mce.Arguments[0].ExpressionToObject();
                        qc = mce.Object.GetQueryColumn<T>();
                        return qc.EndsWith(value.ToString());
                    }
                case "In":
                case "NotIn":
                    {
                        var o = (object[])mce.Arguments[1].ExpressionToObject();
                        return qc.In(o);
                    }
                case "Between":
                    {
                        var list = new List<object>();
                        for (int i = 1; i < mce.Arguments.Count; i++)
                        {
                            list.Add(mce.Arguments[i].ExpressionToObject());
                        }
                        return qc.Between(list[0], list[1], true, true);
                    } 
            }
            return new Condition();
        }

        #endregion

        #region T,I

        internal static Condition Eval<T, I>(Expression<Func<T, I, bool>> ex)
            where T : class,IBaseEntity
            where I : class,IBaseEntity
        {
            var be = ex.Body as BinaryExpression;
            return be != null
                       ? BinarExpressionProvider<T, I>(be.Left, be.Right, be.NodeType)
                       : (ex.Body is MethodCallExpression ? ColumnFunction<T, I>((MethodCallExpression)ex.Body) : new Condition());
        }

        private static Condition BinarExpressionProvider<T, I>(System.Linq.Expressions.Expression left, System.Linq.Expressions.Expression right, ExpressionType type)
            where T : class,IBaseEntity
            where I : class,IBaseEntity
        {
            ExpressionClip li = ExpressionRouter<T, I>(left);
            ExpressionClip ri = ExpressionRouter<T, I>(right);

            var le = li as Condition;
            if (!ReferenceEquals(le, null))
            {
                ConditionAndOr op = type.ConditionAndOrTypeCast();
                le.LinkedConditionAndOrs.Add(op);
                le.LinkedConditions.Add((Condition)ri);
                return le;
            }
            else
            {
                ExpressionOperator op = type.ExpressionTypeCast();
                return new Condition(li, op, ri);
            }
        }

        private static ExpressionClip ExpressionRouter<T, I>(System.Linq.Expressions.Expression exp)
            where T : class,IBaseEntity
            where I : class,IBaseEntity
        {
            if (exp is BinaryExpression)
            {
                var be = ((BinaryExpression)exp);
                switch (be.NodeType)
                {
                    case ExpressionType.Coalesce:
                        return ExpressionRouter<T, I>(be.Left.ExpressionToObject() == null ? be.Right : be.Left);
                    case ExpressionType.ArrayIndex:
                        return
                            new NSun.Data.ParameterExpression(
                                System.Linq.Expressions.Expression.Lambda(be).Compile().DynamicInvoke(),
                                ConvertListUtil.GetDbType(be.Type, true)
                                );
                }
                return BinarExpressionProvider<T, I>(be.Left, be.Right, be.NodeType);
            }
            if (exp is MemberExpression)
            {
                MemberExpression me = ((MemberExpression)exp);
                if (me.Expression is ConstantExpression)
                {
                    var value = me.ExpressionToObject();
                    return
                        new NSun.Data.ParameterExpression(value,
                            ConvertListUtil.GetDbType(exp.Type, true)
                            );
                }
                var querycolumn = me.GetColumnByMember<T, I>();
                if (ReferenceEquals(querycolumn, null))
                {
                    var value = me.ExpressionToObject();
                    return
                        new NSun.Data.ParameterExpression(
                            value, ConvertListUtil.GetDbType(me.Type, true)
                            );
                }
                return querycolumn;
            }
            if (exp is MethodCallExpression)
            {
                var mce = (MethodCallExpression)exp;
                var methodname = mce.Method.Name;
                var me = ((MemberExpression)mce.Arguments[0]);
                return ReturnExpression<T, I>(mce, methodname, me);
            }
            if (exp is ConstantExpression)
            {
                return
                     new NSun.Data.ParameterExpression(
                            exp.ExpressionToObject(), ConvertListUtil.GetDbType(exp.Type, true)
                            );
            }
            if (exp is UnaryExpression)
            {
                if (exp.NodeType == ExpressionType.ArrayLength)
                {
                    return
                          new NSun.Data.ParameterExpression(
                            System.Linq.Expressions.Expression.Lambda(exp).Compile().DynamicInvoke(), ConvertListUtil.GetDbType(exp.Type, true)
                            );
                }
                var ue = ((UnaryExpression)exp);
                return ExpressionRouter<T, I>(ue.Operand);
            }
            if (exp is ConditionalExpression)
            {
                var ce = (ConditionalExpression)exp;
                if (Convert.ToBoolean(ce.Test.ExpressionToObject()))
                {
                    return ExpressionRouter<T, I>(ce.IfTrue);
                }
                return ExpressionRouter<T, I>(ce.IfFalse);
            }
            return null;
        }

        private static Condition ColumnFunction<T, I>(MethodCallExpression mce)
            where T : class,IBaseEntity
            where I : class,IBaseEntity
        {
            QueryColumn qc = mce.Arguments[0].GetMemberMethod().GetColumnByMember<T, I>();
            var methodname = mce.Method.Name;
            switch (methodname)
            {
                case "Like":
                    return qc.Like(mce.Arguments[1].ExpressionToObject().ToString());
                case "Contains":
                    qc = ((MemberExpression)mce.Object).GetColumnByMember<T, I>();
                    return qc.Contains(mce.Arguments[0].ExpressionToObject().ToString());
                case "StartsWith":
                    qc = ((MemberExpression)mce.Object).GetColumnByMember<T, I>();
                    return qc.StartsWith(mce.Arguments[0].ExpressionToObject().ToString());
                case "EndsWith":
                    qc = ((MemberExpression)mce.Object).GetColumnByMember<T, I>();
                    return qc.EndsWith(mce.Arguments[0].ExpressionToObject().ToString());
                case "In":
                case "NotIn":
                    {
                        var o = (object[])mce.Arguments[1].ExpressionToObject();
                        return qc.In(o);
                    }
                case "Between":
                    {
                        var list = mce.Arguments.Select(item => item.ExpressionToObject()).ToList();
                        return qc.Between(list[0], list[1], true, true);
                    }
            }
            return new Condition();
        }

        internal static ExpressionClip ReturnExpression<T, I>(MethodCallExpression mce, string methodname, MemberExpression me)
            where T : class,IBaseEntity
            where I : class,IBaseEntity
        {
            QueryColumn qc = me.GetColumnByMember<T, I>();
            return me.Member.ReflectedType.FullName == typeof(T).FullName ? ReturnExpression<T>(mce, methodname, qc) : ReturnExpression<I>(mce, methodname, qc);
        }
        #endregion

    }
}
