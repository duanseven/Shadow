using System;
using System.Linq;
using System.Linq.Expressions;
namespace NSun.Data.Helper
{
    public class ModelRefObjectHelper : NSun.Data.Helper.IModelHelper
    {
        private DBQuery _db;

        public ModelRefObjectHelper(DBQuery db)
        {
            Check.Assert(db != null);
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
                    var beelp = new BaseEntityRefObjectProxy<T>(tag, DB);
                    tag = beelp.ReadInvoke<T>(readrelation);
                }
                //如果有懒加载就去做代理
                if (lazyrelation.Any())// count >0
                {
                    var beelp = new BaseEntityRefObjectProxy<T>(tag, DB);
                    tag = beelp.GetTransparentProxy() as T;
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
                    var beelp = new BaseEntityRefObjectProxy(type, tag, DB);
                    tag = beelp.ReadInvoke(readrelation);
                }
                //如果有懒加载就去做代理
                if (lazyrelation.Any())// count >0
                {
                    var beelp = new BaseEntityRefObjectProxy(type, tag, DB);
                    tag = beelp.GetTransparentProxy();
                }
                return tag;
            }
            return obj;
        }

        public DBQuery DB
        {
            get { return _db; }
        }

        public object CreateProxy(Type type)
        {
            return new BaseEntityRefObjectProxy(type, DB).GetTransparentProxy();
        }

        public T CreateProxy<T>() where T : class
        {
            return new BaseEntityRefObjectProxy<T>(DB).GetTransparentProxy() as T;
        }
    }
}
