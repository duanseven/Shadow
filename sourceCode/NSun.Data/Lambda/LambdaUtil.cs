using System;
using System.Collections.Generic; 
using System.Linq;
using System.Linq.Expressions;
using System.Reflection; 

namespace NSun.Data.Lambda
{
    public static class LambdaUtil
    {       
        #region SelectColumn

        //CreateQuery(p=>new { p.id,p.name}); //internal
        public static ExpressionClip[] GetExpressionClip<T>(Expression<Func<T, object>> fun) where T : class,IBaseEntity
        {
            if (fun == null)
                throw new System.ArgumentNullException("Expression is Null");
            var list = NewObjectToName(fun);//get columns name
            return GetExpressionClip(list.ToArray());
        }

        private static ExpressionClip[] GetExpressionClip(params System.Collections.Generic.KeyValuePair<object, string>[] pars)
        {
            if (pars == null || pars.Length == 0) 
                throw new System.ArgumentNullException("KeyVal is Null"); 

            var list = new List<ExpressionClip>();
            foreach (var item in pars)
            {
                if (item.Key is ExpressionClip)
                {
                    if (item.Key is QueryColumn)
                    {
                        var namec = GetQueryColumnToPropertyName(((QueryColumn) item.Key));
                        if (namec.ToLower() == item.Value.ToLower())
                        {
                            list.Add(((ExpressionClip)item.Key));
                            continue;
                        }
                    }
                    list.Add(((ExpressionClip)item.Key).As(item.Value));
                }
            }
            return list.ToArray();
        }
        //join table
        internal static ExpressionClip[] GetExpressionClip<TTable, ITable>(Expression<Func<TTable, ITable, object>> fun)
            where TTable : class, IBaseEntity
            where ITable : class, IBaseEntity
        {
            if (fun == null)
                throw new System.ArgumentNullException("Expression is null");
            List<System.Collections.Generic.KeyValuePair<object, string>> list = NewObjectToName(fun);
            return GetExpressionClip(list.ToArray());
        }

        #endregion

        #region OrderByClip

        internal static OrderByClip[] GetOrderByClip<T>(Expression<Func<T, object>> fun) where T : class,IBaseEntity
        {
            if (fun == null) return null;
            var list = NewObjectToName(fun);
            return GetOrderByClip(list.ToArray());
        }

        private static OrderByClip[] GetOrderByClip(params System.Collections.Generic.KeyValuePair<object, string>[] pars)
        {
            if (pars == null || pars.Length == 0) return null;
            var list = new List<OrderByClip>();
            foreach (var item in pars)
            {
                if (item.Key is QueryColumn)
                {
                    var querycolumn = (QueryColumn)item.Key;
                    if (item.Value == "Asc") 
                        list.Add(querycolumn.Asc); 
                    else 
                        list.Add(querycolumn.Desc); 
                }
            }
            return list.ToArray();
        }

        internal static OrderByClip[] GetOrderByClip<T, I>(Expression<Func<T, I, object>> fun)
            where T : class,IBaseEntity
            where I : class,IBaseEntity
        {
            if (fun == null) return null;
            var list = NewObjectToName(fun);
            return GetOrderByClip(list.ToArray());
        }

        #endregion

        #region Group

        internal static QueryColumn[] GetQueryColumn<T>(Expression<Func<T, object>> fun) where T : class,IBaseEntity
        {
            if (fun == null) return null;
            var list = NewObjectToName(fun);
            return GetQueryColumn(list.ToArray());
        }

        internal static QueryColumn[] GetQueryColumn<T, I>(Expression<Func<T, I, object>> fun)
            where T : class,IBaseEntity
            where I : class,IBaseEntity
        {
            if (fun == null) return null;
            var list = NewObjectToName(fun);
            return GetQueryColumn(list.ToArray());
        }

        private static QueryColumn[] GetQueryColumn(params System.Collections.Generic.KeyValuePair<object, string>[] pars)
        {
            if (pars == null || pars.Length == 0) return null;
            List<QueryColumn> list = new List<QueryColumn>();
            foreach (var item in pars)
            {
                if (item.Key is QueryColumn)
                {
                    var querycolumn = item.Key as QueryColumn;
                    list.Add(querycolumn);
                }                 
            }
            return list.ToArray();
        }

        #endregion

        #region NewObjectToName
        //得到select order group 的字段
        private static List<System.Collections.Generic.KeyValuePair<object, string>> NewObjectToName<T>(Expression<Func<T, object>> fun)
            where T : class,IBaseEntity
        {
            var vals = new List<System.Collections.Generic.KeyValuePair<object, string>>();
            //select id,name from...
            //new{id=1,name=dc};
            if (fun.Body is NewExpression)
            {
                var newex = fun.Body as NewExpression;
                if (newex != null)
                    for (var i = 0; i < newex.Arguments.Count; i++)
                    {
                        object name;
                        //column name
                        if (newex.Arguments[i] is MethodCallExpression)
                        {
                            var mce = (MethodCallExpression)newex.Arguments[i];
                            var methodname = mce.Method.Name;
                            QueryColumn qc = mce.Arguments[0].GetQueryColumn<T>();
                            name = ExpressionUtil.ReturnExpression<T>(mce, methodname, qc);
                        }
                        else
                            name = newex.Arguments[i].GetQueryColumn<T>();
                        var asname = Notget(newex.Members[i].Name);
                        vals.Add(new System.Collections.Generic.KeyValuePair<object, string>(name,asname));
                    }
            }
            //order by...
            //new []{id.Asc,name.Desc};
            else if (fun.Body is NewArrayExpression)
            {
                var newarr = fun.Body as NewArrayExpression;
                if (newarr != null)
                    //foreach (Expression item in newarr.Expressions)
                    //{
                    //    var mce = item as MethodCallExpression;
                    //    if (mce != null)
                    //    {
                    //        var kv = new KeyVal
                    //        {
                    //            Key = Util.GetQueryColumn<T>(mce.Arguments[0]),
                    //            Val = mce.Method.Name == "Asc" ? "Asc" : "Desc"
                    //        };
                    //        vals.Add(kv);
                    //    }
                    //}
                    vals.AddRange(newarr.Expressions.OfType<MethodCallExpression>().Select(mce => new System.Collections.Generic.KeyValuePair<object, string>(mce.Arguments[0].GetQueryColumn<T>(),mce.Method.Name == "Asc" ? "Asc" : "Desc")));
            }
            //only one and order by..
            else if (fun.Body is MethodCallExpression)
            {
                var mec = fun.Body as MethodCallExpression;
                if (mec != null)
                {
                    var kv =
                        new System.Collections.Generic.KeyValuePair<object, string>(
                            mec.Arguments[0].GetQueryColumn<T>(), mec.Method.Name);
                    vals.Add(kv);
                }
            }
            //group by
            else if (fun.Body is MemberExpression || fun.Body is UnaryExpression)
            {
                MemberExpression me = fun.Body.GetMemberMethod();
                var kv = new System.Collections.Generic.KeyValuePair<object, string>(me.GetQueryColumn<T>(),
                                                                                     me.Member.Name);
                vals.Add(kv);
            }
            return vals;
        }

        private static List<System.Collections.Generic.KeyValuePair<object, string>> NewObjectToName<T, I>(Expression<Func<T, I, object>> fun)
            where T : class,IBaseEntity
            where I : class,IBaseEntity
        {
            var vals = new List<System.Collections.Generic.KeyValuePair<object, string>>();          
            if (fun.Body is NewExpression)
            {
                var newex = fun.Body as NewExpression;
                if (newex != null)
                    for (int i = 0; i < newex.Arguments.Count; i++)
                    {
                        object name;
                        //column name
                        if (newex.Arguments[i] is MethodCallExpression)
                        {
                            var mce = (MethodCallExpression)newex.Arguments[i];
                            var methodname = mce.Method.Name;
                            name = ExpressionUtil.ReturnExpression<T, I>(mce, methodname, mce.Arguments[0].GetMemberMethod());
                        }
                        else
                            name = newex.Arguments[i].GetMemberMethod().GetColumnByMember<T, I>();
                        var asname = Notget(newex.Members[i].Name);
                        vals.Add(new System.Collections.Generic.KeyValuePair<object, string>(name, asname));
                    }
            }
            else if (fun.Body is NewArrayExpression)
            {
                var newarr = fun.Body as NewArrayExpression;
                if (newarr != null)
                    //foreach (Expression item in newarr.Expressions)
                    //{
                    //    var mce = item as MethodCallExpression;
                    //    if (mce != null)
                    //    {
                    //        var ex = mce.Arguments[0];
                    //        var kv = new KeyVal
                    //        {
                    //            Key = Util.GetColumnByMember<T, I>(Util.GetMemberMethod(ex)),
                    //            Val = mce.Method.Name == "Asc" ? "Asc" : "Desc"
                    //        };
                    //        vals.Add(kv);
                    //    }
                    //}
                    vals.AddRange(from mce in newarr.Expressions.OfType<MethodCallExpression>()
                                  let ex = mce.Arguments[0]
                                  select new System.Collections.Generic.KeyValuePair<object, string>(ex.GetMemberMethod().GetColumnByMember<T, I>(), mce.Method.Name == "Asc" ? "Asc" : "Desc"));
            }
            else if (fun.Body is MethodCallExpression)
            {
                var mec = fun.Body as MethodCallExpression;
                if (mec != null)
                {
                    var kv =
                        new System.Collections.Generic.KeyValuePair<object, string>(
                            mec.Arguments[0].GetMemberMethod().GetColumnByMember<T, I>(), mec.Method.Name);
                    vals.Add(kv);
                }
            }
            else if (fun.Body is MemberExpression || fun.Body is UnaryExpression)
            {
                var me = fun.Body.GetMemberMethod();
                //var me = fun.Body as MemberExpression;
                if (me != null)
                {
                    var kv = new System.Collections.Generic.KeyValuePair<object, string>(me.GetColumnByMember<T, I>(),
                                                                                         me.Member.Name);
                    vals.Add(kv);
                }
            }
            //else if (fun.Body is UnaryExpression)
            //{            
            //    var me = ((UnaryExpression) fun.Body).Operand as MemberExpression;
            //    if (me != null)
            //    {
            //        var kv = new KeyVal {Key = Util.GetColumnByMember<T, I>(me), Val = me.Member.Name};
            //        vals.Add(kv);
            //    }
            //}
            return vals;
        }


        /// <summary>
        /// 去掉前缀get_属性方法
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        internal static string Notget(string name)
        {
            return name.TrimStart("get_".ToCharArray());
        }

        internal static string GetQueryColumnToPropertyName(QueryColumn qc)
        {
            return qc.ColumnName.Split('.')[1];
        }

        /// <summary>
        /// 获得列对应属性的值
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="colname"></param>
        /// <returns></returns>
        internal static object GetKeyValue(object obj, string colname)
        {
            return (from pinfo in obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.CreateInstance)
                    where pinfo.Name.ToLower() == colname.Split('.')[1].ToLower()
                    select pinfo.GetValue(obj, null)).FirstOrDefault();
        }
        #endregion

        #region Entity

        internal static T PersistenceInvoke<T>(T tag) where T : class, IBaseEntity
        {
            tag.SetIsPersistence(true);
            return (T)tag;
        }
         
        #endregion

        #region if DEBUG
        //public static List<KeyVal> NewObjectToAttr(object obj)
        //{
        //    List<KeyVal> vals = new List<KeyVal>();
        //    PropertyInfo[] pinfos = obj.GetType().GetProperties();
        //    foreach (PropertyInfo item in pinfos)
        //    {
        //        vals.Add(new KeyVal() { Key = item.Name, Val = item.GetValue(obj, null) == null ? string.Empty : item.GetValue(obj, null).ToString() });
        //    }
        //    return vals;
        //}
        #endregion
    }     
}
