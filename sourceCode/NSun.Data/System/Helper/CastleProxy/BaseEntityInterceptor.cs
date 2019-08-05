using System;
using System.Linq;
using System.Reflection;

namespace NSun.Data.Helper
{
    public class BaseEntityInterceptor : Castle.DynamicProxy.StandardInterceptor
    {
        private DBQuery DB;
        private IModelHelper modelHelper;

        public BaseEntityInterceptor(DBQuery db)
        {
            DB = db;            
        }

        public IModelHelper GetModel()
        {
            return modelHelper ?? (modelHelper = new ModelHelper(DB));
        }
        //直接读
        protected void ReadProceed(Type targetType)
        {
            TableSchema tableschema = DBQueryFactory.GetTableSchema(targetType);
        }
        //懒加载
        protected override void PostProceed(Castle.DynamicProxy.IInvocation invocation)
        {
            if (invocation.Method.ReturnType.GetInterface(ConvertListUtil.CompareFullName) == null && !(invocation.Method.ReturnType.IsGenericType && invocation.Method.ReturnType.Namespace == "NSun.Data.Collection"))
            {
                return;
            }
            //赋值后的操作 
            if (invocation.Method.IsSpecialName &&
                invocation.Method.Name.StartsWith("get_"))
            {                
                if (invocation.ReturnValue != null)
                {
                    return;
                }
                TableSchema tableschema = DBQueryFactory.GetTableSchema(invocation.TargetType);
                RelationQueryColumn relationcolumn;
                string properyinfoname= invocation.Method.Name.TrimStart("get_".ToCharArray()).ToLower();
                if (tableschema.RelationColumn.TryGetValue(
                    properyinfoname
                    , out relationcolumn))
                {
                    //RelationTable 
                    var tableattr = AttributeUtils.GetAttribute<TableAttribute>(relationcolumn.RelationTable);
                    MemberInfo pkcolumn = GetPKMemberInfo(tableschema, relationcolumn);
                    var value = ReflectionUtils.GetFieldValue(invocation.InvocationTarget, pkcolumn);
                    SelectSqlSection query;
                    var handler = GetModel();
                    object returnvalue = null;
                    switch (relationcolumn.RelationType)
                    {
                        case RelationType.OneToOne:
                        case RelationType.ManyToOne:
                            query = DB.CreateQuery(tableattr.TableName).Where(relationcolumn.RelationColumn == value);
                            returnvalue = handler.CreateProxy(invocation.Method.ReturnType,
                                                              DB.ToEntity(invocation.Method.ReturnType, query));
                            break;
                        case RelationType.OneToMany:
                            query = DB.CreateQuery(tableattr.TableName).Where(relationcolumn.RelationColumn == value);
                            returnvalue = DB.ToList(invocation.Method.ReturnType.GetGenericArguments()[0], query);
                            break;                   
                        case RelationType.ManyToMany:
                            //OutTable
                            var tableOut = AttributeUtils.GetAttribute<TableAttribute>(relationcolumn.RelationOutTable);
                            query = DB.CreateQuery(tableOut.TableName).Where(relationcolumn.RelationOutColumn.In(
                                DB.CreateQuery(tableattr.TableName).Where(relationcolumn.RelationColumn == value).Select(relationcolumn.RelationMappingColumn).ToSubQuery()       
                                                                                 ));
                            returnvalue = DB.ToList(invocation.Method.ReturnType.GetGenericArguments()[0], query);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    SetReturnValue(invocation, tableschema, properyinfoname, returnvalue);
                    invocation.ReturnValue = returnvalue;
                }
            }  
        }

        private static MemberInfo GetPKMemberInfo(TableSchema tableschema, RelationQueryColumn relationcolumn)
        {
            var pkpropertyname= relationcolumn.PkColumn.PropertyName;
            MemberInfo pkcolumn =
                tableschema.ValuePropertys.Where(p => p.Name.ToLower() == pkpropertyname).FirstOrDefault();
            return pkcolumn;
        }

        private static void SetReturnValue(Castle.DynamicProxy.IInvocation invocation, TableSchema tableschema, string properyinfoname, object returnvalue)
        {
            PropertyInfo returninfo =
                tableschema.ValuePropertys.Where(p => p.Name.ToLower() == properyinfoname).FirstOrDefault();
            if (returninfo != null)
            {
                returninfo.SetValue(invocation.InvocationTarget, returnvalue, null);
            }
        }
    }

    public class BaseEntityProxy
    {
        protected object _target;
        protected DBQuery _db;
        protected Type _type;
        private IModelHelper modelHelper;

        public IModelHelper GetModel()
        {
            return modelHelper ?? (modelHelper = new ModelHelper(_db));
        }

        public BaseEntityProxy(Type type, object instance, DBQuery db)
        {
            _target = instance;
            _type = type;
            _db = db;
        }

        public BaseEntityProxy(Type type, DBQuery db)
            : this(type, null, db) { }
       
        public object ReadInvoke(System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, RelationQueryColumn>> readrelation)
        {
            TableSchema tableschema = DBQueryFactory.GetTableSchema(_type);
            foreach (var keyValuePair in readrelation)
            {
                RelationQueryColumn relationcolumn = keyValuePair.Value;
                var tableattr = AttributeUtils.GetAttribute<TableAttribute>(relationcolumn.RelationTable);
                MemberInfo pkcolumn = GetPKMemberInfo(tableschema, relationcolumn);//主键列属性
                var value = ReflectionUtils.GetFieldValue(_target, pkcolumn);//主键值
                var member = GetMemberInfo(tableschema, relationcolumn);//关系列对应的属性
                object returnValue;//得到当前列的值
                var handler = GetModel();
                switch (keyValuePair.Value.RelationType)
                {
                    case RelationType.OneToOne:
                    case RelationType.ManyToOne:
                        SelectSqlSection query = _db.CreateQuery(tableattr.TableName)
                            .Where(relationcolumn.RelationColumn == value);
                        returnValue = handler.CreateProxy(member.PropertyType,
                                                         _db.ToEntity(member.PropertyType, query));
                        break;
                    case RelationType.OneToMany:
                        query = _db.CreateQuery(tableattr.TableName).Where(relationcolumn.RelationColumn == value);
                        returnValue = _db.ToList(member.PropertyType.GetGenericArguments()[0], query);
                        break;
                    case RelationType.ManyToMany:
                        var tableOut = AttributeUtils.GetAttribute<TableAttribute>(relationcolumn.RelationOutTable);
                        query = _db.CreateQuery(tableOut.TableName).Where(relationcolumn.RelationOutColumn.In(
                             _db.CreateQuery(tableattr.TableName).Where(relationcolumn.RelationColumn == value).Select(relationcolumn.RelationMappingColumn).ToSubQuery()));
                        returnValue = _db.ToList(member.PropertyType.GetGenericArguments()[0], query);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                SetReturnValue(_target, tableschema, relationcolumn.PropertyName, returnValue);
            }
            return _target;
        }

        private static MemberInfo GetPKMemberInfo(TableSchema tableschema, RelationQueryColumn relationcolumn)
        {
            var pkpropertyname = relationcolumn.PkColumn.PropertyName;
            MemberInfo pkcolumn =
                tableschema.ValuePropertys.FirstOrDefault(p => p.Name.ToLower() == pkpropertyname);
            return pkcolumn;
        }

        private static PropertyInfo GetMemberInfo(TableSchema tableschema, RelationQueryColumn relationcolumn)
        {
            var pkpropertyname = relationcolumn.PropertyName;
            var property = tableschema.ValuePropertys.FirstOrDefault(p => p.Name.ToLower() == pkpropertyname.ToLower());
            return property;
        }

        private static void SetReturnValue(object invocation, TableSchema tableschema, string properyinfoname, object returnvalue)
        {
            PropertyInfo returninfo =
                tableschema.ValuePropertys.FirstOrDefault(p => p.Name.ToLower() == properyinfoname);
            if (returninfo != null)
            {
                returninfo.SetValue(invocation, returnvalue, null);
            }
        }
    }

    public class BaseEntityProxy<T> : BaseEntityProxy
    {
        public BaseEntityProxy(T instance, DBQuery db)
            : base(typeof(T), instance, db) { }

        public BaseEntityProxy(DBQuery db)
            : base(typeof(T), db) { }

        public T ReadInvoke<T>(System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, RelationQueryColumn>> readrelation) where T : class
        {
            return base.ReadInvoke(readrelation) as T;
        }
    }
}