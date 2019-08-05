using System;
using System.Linq;

namespace NSun.Data.Helper
{
    public class ModelHelper : NSun.Data.Helper.IModelHelper
    {
        private Castle.DynamicProxy.ProxyGenerator ProxyGenerator;
        private BaseEntityInterceptor Interceptor;
        private DBQuery _db;

        public ModelHelper(DBQuery db)
        {
            Check.Assert(db != null);

            ProxyGenerator = new Castle.DynamicProxy.ProxyGenerator();
            Interceptor = new BaseEntityInterceptor(db);
            _db = db;
        }

        public T CreateProxy<T>(T obj) where T : class
        {
            if (DB.Db.IsUseRelation)
            {
                T tag = obj;
                var tableschema = DBQueryFactory.GetTableSchema<T>();
                //直接读的
                var readrelation = tableschema.RelationColumn.Where(p => p.Value.LoadRelationType == LoadType.Read);
                //懒加载的
                var lazyrelation = tableschema.RelationColumn.Where(p => p.Value.LoadRelationType == LoadType.Lazy);
                //如果没有懒加载就直接读
                if (readrelation.Any())
                {
                    BaseEntityProxy<T> beit = new BaseEntityProxy<T>(tag, DB);
                    tag = beit.ReadInvoke<T>(readrelation);
                }
                //如果有懒加载就去做代理
                if (lazyrelation.Any())// count >0
                {
                    tag = ProxyGenerator.CreateClassProxyWithTarget(tag, Interceptor);
                }
                return tag;
            }
            return obj;
        }

        public object CreateProxy(Type type, object obj)
        {
            if (DB.Db.IsUseRelation)
            {
                object tag = obj;
                var tableschema = DBQueryFactory.GetTableSchema(type);
                //直接读的
                var readrelation = tableschema.RelationColumn.Where(p => p.Value.LoadRelationType == LoadType.Read);
                //懒加载的
                var lazyrelation = tableschema.RelationColumn.Where(p => p.Value.LoadRelationType == LoadType.Lazy);
                //如果没有懒加载就直接读
                if (readrelation.Any())
                {
                    BaseEntityProxy beit = new BaseEntityProxy(type, tag, DB);
                    tag = beit.ReadInvoke(readrelation);
                }
                //如果有懒加载就去做代理
                if (lazyrelation.Any())// count >0
                {
                    tag = ProxyGenerator.CreateClassProxyWithTarget(type, tag, Interceptor);
                }
                return tag;
            }
            return obj;
        }

        public object CreateProxy(Type type)
        {
            return ProxyGenerator.CreateClassProxy(type, Interceptor);
        }

        public T CreateProxy<T>() where T : class
        {
            return ProxyGenerator.CreateClassProxy<T>(Interceptor);
        }


        #region IModelHelper 成员

        public DBQuery DB
        {
            get { return _db; }
        }

        #endregion
    }
}
