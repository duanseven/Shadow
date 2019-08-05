using System;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Proxies;

namespace NSun.Data.Helper
{
    public class BaseEntityRefObjectProxy : RealProxy
    {
        protected object _target;
        protected DBQuery _db;
        protected Type _type;
        private IModelHelper modelHelper;

        public IModelHelper GetModel()
        {
            return modelHelper ?? (modelHelper = new ModelRefObjectHelper(_db));
        }

        public BaseEntityRefObjectProxy(Type type, object instance, DBQuery db)
            : base(type)
        {
            _target = instance;
            _type = type;
            _db = db;
        }

        public BaseEntityRefObjectProxy(Type type, DBQuery db)
            : this(type, null, db) { }         

        public override IMessage Invoke(IMessage msg)
        {
            IMethodCallMessage callMessage = (IMethodCallMessage)msg;
            object returnValue = callMessage.MethodBase.Invoke(this._target, callMessage.Args);
            var methodname = msg.Properties["__MethodName"].ToString();
            if (methodname.StartsWith("get_"))
            {                              
                if (returnValue == null)
                {
                    TableSchema tableschema = DBQueryFactory.GetTableSchema(_type);
                    RelationQueryColumn relationcolumn;
                    string properyinfoname = methodname.TrimStart("get_".ToCharArray()).ToLower();
                    if (tableschema.RelationColumn.TryGetValue(
                    properyinfoname
                    , out relationcolumn))
                    {
                        //RelationTable 
                        var tableattr = AttributeUtils.GetAttribute<TableAttribute>(relationcolumn.RelationTable);
                        MemberInfo pkcolumn = GetPKMemberInfo(tableschema, relationcolumn);
                        var value = ReflectionUtils.GetFieldValue(_target, pkcolumn);
                        SelectSqlSection query;
                        var handler = GetModel(); 
                        var returntype = _type.GetMethod(methodname).ReturnType;
                        switch (relationcolumn.RelationType)
                        {
                            case RelationType.OneToOne:
                            case RelationType.ManyToOne:
                                query = _db.CreateQuery(tableattr.TableName).Where(relationcolumn.RelationColumn == value);
                                returnValue = handler.CreateProxy(returntype,
                                _db.ToEntity(returntype, query));
                                break;
                            case RelationType.OneToMany:
                                query = _db.CreateQuery(tableattr.TableName).Where(relationcolumn.RelationColumn == value);
                                returnValue = _db.ToList(returntype.GetGenericArguments()[0], query);
                                break;
                            case RelationType.ManyToMany:
                                //OutTable
                                var tableOut = AttributeUtils.GetAttribute<TableAttribute>(relationcolumn.RelationOutTable);
                                query = _db.CreateQuery(tableOut.TableName).Where(relationcolumn.RelationOutColumn.In(
                                     _db.CreateQuery(tableattr.TableName).Where(relationcolumn.RelationColumn == value).Select(relationcolumn.RelationMappingColumn).ToSubQuery()));
                                returnValue = _db.ToList(returntype.GetGenericArguments()[0], query);
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                        SetReturnValue(_target, tableschema, properyinfoname, returnValue);
                        return new ReturnMessage(returnValue, new object[0], 0, null, callMessage);
                    }
                }
            }            
            return new ReturnMessage(returnValue, new object[0], 0, null, callMessage);           
        }

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

    public class BaseEntityRefObjectProxy<T> : BaseEntityRefObjectProxy
    {
        public BaseEntityRefObjectProxy(T instance, DBQuery db)
            : base(typeof(T), instance, db) { }

        public BaseEntityRefObjectProxy(DBQuery db)
            : base(typeof(T), db) { }

        public T ReadInvoke<T>(System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, RelationQueryColumn>> readrelation) where T : class
        {
            return base.ReadInvoke(readrelation) as T;
        }
    }
}