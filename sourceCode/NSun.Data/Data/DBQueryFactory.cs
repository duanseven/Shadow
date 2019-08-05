using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection; 
using NSun.Data.Common;

namespace NSun.Data
{

    public class RelationOptions
    {
        internal Database _db;

        internal RelationOptions(Database db)
        {
            _db = db;
        }

        public RelationOptions()
        {            
        }         

        public ProxyType DynamicProxyType
        {
            get { return _db.DynamicProxyType; } 
            set { _db.DynamicProxyType = value; }
        }

        public bool IsUseRelation
        {
            get { return _db.IsUseRelation; }
            set { _db.IsUseRelation = value; }
        }
    }

    public class DBQueryFactory
    {
        #region Construction

        private RelationOptions options;

        public RelationOptions Options
        {
            get { return options; }
            set
            { 
                options = value;
                if (options._db == null)
                {
                    options._db = db;
                }
            }
        }

        //public bool IsUseRelation
        //{
        //    get { return db.IsUseRelation; }
        //    set { db.IsUseRelation = value; }
        //}

        /// <summary>
        /// 默认为ProxyType.Remoting 需要继承自BaseEntityRefObject->MarshalByRefObject
        /// ProxyType.Castle 需要继承BaseEntity 原生 但是属性需要virtual
        /// </summary>
        //public ProxyType DynamicProxyType { get { return db.DynamicProxyType; } set { db.DynamicProxyType = value; } }

        internal Database db { get; set; }

        private static System.Collections.Concurrent.ConcurrentDictionary<Type, TableSchema> TableSchemaMap =
            new System.Collections.Concurrent.ConcurrentDictionary<Type, TableSchema>();

        static DBQueryFactory()
        {            
            IsCachDouble = false;
        }

        public DBQueryFactory(string connectionStringName, SqlType sqltype)
        {
            db = new Database(connectionStringName, sqltype);
            Options = new RelationOptions(db);
        }

        public DBQueryFactory(SqlType sqltype,string connectionString)
        {
            db = new Database(sqltype, connectionString);
            Options = new RelationOptions(db);
        }
         
        #endregion

        #region DBQuery

        public DBQuery CreateDBQuery(bool isClonedb = true)
        {
            if (isClonedb)
            {
                var dbclone = (Database)db.Clone();
                return new DBQuery(dbclone, this);
            }
            else
            {
                return new DBQuery(db, this);
            }            
        }

        public DBQuery<TEntity> CreateDBQuery<TEntity>(bool isClonedb=true) where TEntity : class, IBaseEntity
        {
            RegisterTableSchema(typeof(TEntity));
            if (isClonedb)
            {
                var dbclone = (Database)db.Clone();
                return new DBQuery<TEntity>(dbclone, this);    
            }
            else
            {
                return new DBQuery<TEntity>(db, this);
            }            
        }

        #endregion
        
        #region Mapping

        private static readonly object _registersync = new object();

        public static void RegisterTableSchema<T>() where T :class, IBaseEntity
        {
            RegisterTableSchema(typeof(T));
        }

        public static void RegisterTableSchema(Type type)
        {
            if (TableSchemaMap.ContainsKey(type))
                return;
            lock (_registersync)
            {
                if (TableSchemaMap.ContainsKey(type))
                {
                    return;
                }
                TableSchemaMap.TryAdd(type, Init(type));
            }
        }

        public static TableSchema GetTableSchema(Type type)
        {
            if (TableSchemaMap.ContainsKey(type))
                return TableSchemaMap[type];           
            return TableSchemaMap.GetOrAdd(type, p =>
            {
                RegisterTableSchema(p);
                TableSchema table;
                TableSchemaMap.TryGetValue(p, out table);
                return table;
            }); 
        }

        public static TableSchema GetTableSchema<T>()
        {
            return GetTableSchema(typeof(T));   
        }

        internal static TableSchema Init(Type type)
        {
            TableSchema table = new TableSchema();
            table.EntityInfo = CreateBaesEntity(type);//实例化一个BaseEntity
            //是否是自身映射
            TableMappingAttribute tablemapping = AttributeUtils.GetAttribute<TableMappingAttribute>(type);//, typeof(TableMappingAttribute)) as TableMappingAttribute;
            if (tablemapping == null)//自身映射            
                table.MappingInfo = table.EntityInfo;
            else
                table.MappingInfo = CreateObject(tablemapping.MappingType);            

            //初始化QueryColumn
            table.FindFields = GetFieldInfoArray(table);
            //初始化Property
            table.ValuePropertys = GetPropertyInfoArray(table);
            //得到对应名称的QueryColumn
            table.QueryColumns = InitQueryColumn(table);

            InitIdentyFieldInfo(table);

            table.OptimisticLocking = InitOptimisticColumn(table);
            table.RelationColumn = InitRelationColumn(table);
            table.UpdateFields = table.CreateFields = InitCreateOrUpdate(table);
            return table;
        }
        
        internal static object CreateObject(Type type) {
            var info = Activator.CreateInstance(type);
            return info;
        }

        internal static IBaseEntity CreateBaesEntity(Type type)
        {
            var info = CreateObject(type);
            return info as IBaseEntity;
        }
        //QueryColumn
        internal static FieldInfo[] GetFieldInfoArray(TableSchema table)
        {           
            object entity = table.MappingInfo;
            List<FieldInfo> list = new List<FieldInfo>();
            foreach (FieldInfo c in entity.GetType().GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy))
            {               
                if ((c.FieldType.IsSubclassOf(typeof(QueryColumn)) || c.FieldType == typeof(QueryColumn)) && (c.FieldType != typeof(RelationQueryColumn)))
                    list.Add(c);
            }
            return
                list.ToArray();
        }

        internal static PropertyInfo[] GetPropertyInfoArray(TableSchema table)
        {
            IBaseEntity entity = table.EntityInfo;
            return
                (from c in
                     entity.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance |
                                                    BindingFlags.CreateInstance)
                 where  (c.GetSetMethod() != null && c.GetGetMethod() != null)
                 select c).ToArray();
        }

        internal static Dictionary<string, QueryColumn> InitQueryColumn(TableSchema table)
        {
            var queryColumnCache =
                new Dictionary<string, QueryColumn>();
            var fields = table.FindFields;
            
            if (fields != null && fields.Length != 0)
            {
                foreach (FieldInfo item in fields)
                {
                    var name = item.Name.TrimStart('_', '_').ToLower();
                    var column = (QueryColumn) item.GetValue(null);
                    if (string.IsNullOrEmpty(column.PropertyName))
                    {
                        column.PropertyName = name;
                    }
                    queryColumnCache.Add(name, column);
                }
            }
            //如果没有映射的列手动创建
            else
            {
                var propertys = table.ValuePropertys;
                foreach (var item in propertys)
                {
                    bool isunicode = false;
                    bool isidcolumn = false;
                    bool isversion = false;
                    var unicode = AttributeUtils.GetAttribute<UnicodeAttribute>(item);
                    if (unicode != null)
                    {
                        isunicode = unicode.IsUnicode;
                    }
                    var idcolumn = AttributeUtils.GetAttribute<PrimaryKeyAttribute>(item);
                    if (idcolumn!=null)
                    {
                        isidcolumn = true;
                    }
                    var versioncolumn = AttributeUtils.GetAttribute<PrimaryKeyAttribute>(item);
                    if (versioncolumn != null)
                    {
                        isversion = true;
                    }
                    var name = item.Name.ToLower();
                    QueryColumn column = null;
                    if (isidcolumn)
                    {
                        column = new IdQueryColumn(table.TableName.TrimStart('[').TrimEnd(']') + "." + item.Name.TrimStart('[').TrimEnd(']'),
                                                 ConvertListUtil.GetDbType(item.PropertyType, isunicode), idcolumn.IsAutoGenerated);
                    }
                    else if (isversion)
                    {
                        column = new VersionQueryColumn(table.TableName.TrimStart('[').TrimEnd(']') + "." + item.Name.TrimStart('[').TrimEnd(']'),
                                                 ConvertListUtil.GetDbType(item.PropertyType, isunicode));
                    }
                    else
                    {
                        column = new QueryColumn(table.TableName.TrimStart('[').TrimEnd(']') + "." + item.Name.TrimStart('[').TrimEnd(']'),
                                                 ConvertListUtil.GetDbType(item.PropertyType, isunicode));
                    }
                    if (string.IsNullOrEmpty(column.PropertyName))
                    {
                        column.PropertyName = name;
                    }
                    queryColumnCache.Add(name, column);
                }
            }
            return queryColumnCache;
        }

        internal static void InitIdentyFieldInfo(TableSchema table)
        {
            var keyvalue = table.QueryColumns.Where(p => p.Value.GetType() == typeof (IdQueryColumn)).FirstOrDefault();

            if (null != keyvalue.Key)
            {
                var idquerycolumn = keyvalue.Value as IdQueryColumn;
                table.IsIdentyFieldAutoGenerated = idquerycolumn.AutoGenerated;
                table.IdentyColumn = idquerycolumn;
                table.IdentyFieldMemberInfo =
                    table.ValuePropertys.Where(
                        p => p.Name.Equals(keyvalue.Key, StringComparison.CurrentCultureIgnoreCase)).First();
            }

            //foreach (PropertyInfo pi in table.ValuePropertys)
            //{
            //    var pka = AttributeUtils.GetAttribute<PrimaryKeyAttribute>(pi);
            //    if (pka != null)
            //    {
            //        table.IdentyFieldMemberInfo = pi;
            //        table.IsIdentyFieldAutoGenerated = pka.IsAutoGenerated;
            //        table.IdentyColumn = GetQueryColumnCache(table, table.IdentyFieldMemberInfo.Name.ToLower());
            //        return;
            //    }
            //}
            //table.IsIdentyFieldAutoGenerated = false;
            //table.IdentyFieldMemberInfo = null;
            //table.IdentyColumn = null;
        }

        internal static Dictionary<string, QueryColumn> InitOptimisticColumn(TableSchema table)
        {
            var optimisticLocking = new Dictionary<string, QueryColumn>();
            var fields = table.FindFields;

            if (fields != null && fields.Length != 0)
            {
                foreach (KeyValuePair<string, QueryColumn> keyValuePair in table.QueryColumns.Where(p=>p.Value.GetType()==typeof(VersionQueryColumn)))
                {
                    optimisticLocking.Add(keyValuePair.Key, keyValuePair.Value);
                }
            }
            else
            {
                foreach (PropertyInfo pi in table.ValuePropertys)
                {
                    var column = pi.Name.ToLower();
                    var oplock = AttributeUtils.GetAttribute<OptimisticLockingAttribute>(pi);
                    if (oplock != null)
                    {
                        optimisticLocking.Add(column, GetQueryColumn(table, column));
                    }
                }
            }
            return optimisticLocking;
        }
         
        internal static Dictionary<string, RelationQueryColumn> InitRelationColumn(TableSchema table)
        {
            var relationcolumn = new Dictionary<string, RelationQueryColumn>();
            var fields =
                (from c in
                     table.MappingInfo.GetType().GetFields(BindingFlags.Static |BindingFlags.Public|BindingFlags.NonPublic| BindingFlags.FlattenHierarchy)
                 where
                 c.FieldType == typeof(RelationQueryColumn)
                 select c).ToArray();

            if (fields.Length != 0)
            {
                foreach (FieldInfo fieldInfo in fields)
                {
                    var name = fieldInfo.Name.TrimStart('_', '_').ToLower();
                    var column = (RelationQueryColumn)fieldInfo.GetValue(null);                    
                    if (string.IsNullOrEmpty(column.PropertyName))
                    {
                        column.PropertyName = name;
                    }
                    relationcolumn.Add(name, column);
                }
                //foreach (KeyValuePair<string, QueryColumn> item in table.QueryColumns)
                //{
                //    var name = item.Key.TrimStart('_', '_').ToLower();
                //    var column = item.Value as RelationQueryColumn;
                //    if (column != null)
                //    {
                //        relationcolumn.Add(name, column);
                //    }
                //}
            }
            return relationcolumn;
        }

        internal static ActiveRecordFieldList InitCreateOrUpdate(TableSchema table)
        {
            var createOrUpdate = new ActiveRecordFieldList();

            if (table.FindFields.Length > 0)
            {
                foreach (var findField in table.FindFields)
                {
                    var fieldname = findField.Name.TrimStart('_', '_').ToLower();
                    if ((table.IsIdentyFieldAutoGenerated && fieldname == table.IdentyFieldMemberInfo.Name.ToLower()))
                        continue;
                    createOrUpdate.Add(fieldname, (QueryColumn)findField.GetValue(table.EntityInfo));
                }
            }
            return createOrUpdate;
        }

        internal static QueryColumn GetQueryColumn(TableSchema table, string namecolumn)
        {
            //if (table.QueryColumns.ContainsKey(namecolumn.ToLower()))
                return table.QueryColumns[namecolumn.ToLower()];
            //lock (_cachesync)
            //{
            //    if (table.QueryColumns.ContainsKey(namecolumn.ToLower()))
            //        return table.QueryColumns[namecolumn.ToLower()];

            //    var column = GetQueryColumn(table, namecolumn);
            //    if (column != null)
            //        table.QueryColumns.Add(namecolumn.ToLower(), column);
            //    return column;
            //}
        }

        //有问题参见其他得到列
        //internal static QueryColumn GetQueryColumn(TableSchema table, string namecolumn)
        //{
        //    var fields = GetFieldInfoArray(table);
        //    BaseEntity entity = table.EntityInfo;
        //    if (fields != null && fields.Length != 0)
        //    {
        //        foreach (FieldInfo item in fields)
        //        {
        //            if (namecolumn.ToLower() == item.Name.TrimStart('_', '_').ToLower())
        //            {
        //                return (QueryColumn)item.GetValue(entity);
        //            }
        //        }
        //    }
        //    else
        //    {
        //        var propertys = GetPropertyInfoArray(table);
        //        foreach (var item in propertys)
        //        {
        //            bool isunicode = false;
        //            var unicode = AttributeUtils.GetAttribute<UnicodeAttribute>(item);
        //            if (unicode != null)
        //            {
        //                isunicode = unicode.IsUnicode;
        //            }
        //            if (namecolumn.ToLower() == item.Name.ToLower())
        //            {
        //                return new QueryColumn(entity.GetTableName().TrimStart('[').TrimEnd(']') + "." + item.Name.TrimStart('[').TrimEnd(']'),
        //                                             ConvertListUtil.GetDbType(item.PropertyType, isunicode));
        //            }
        //        }
        //    }
        //    return null;
        //}

        #endregion

        #region DoubleCache

        private static readonly object _cachesync = new object();        

        public static bool IsCachDouble
        {
            get;
            set;
        }

        protected static System.Collections.Concurrent.ConcurrentDictionary<Type, LruDictionary<object, object>>
            _cacheDouble = new System.Collections.Concurrent.ConcurrentDictionary<Type, LruDictionary<object, object>>();

        protected static int CacheDoubleSize = 500;

        public static void SetCacheDoubleSize(int size)
        {
            CacheDoubleSize = size;
        }

        internal static void AddCacheDouble(object tag)
        {
            if (IsCachDouble)
            {
                var type = tag.GetType();
                object value = GetPkValue(tag);
                LruDictionary<object, object> cache = _cacheDouble.GetOrAdd(
                    type,
                    new LruDictionary<object, object>(CacheDoubleSize));
                cache.Add(value, tag);
            }
        }

        internal static void AddCacheDouble(IEnumerable<object> tags)
        {
            if (IsCachDouble)
            {
                foreach (var tag in tags)
                {
                    AddCacheDouble(tag);
                }
            }
        }

        internal static void RemoveCacheDouble(Type type, object value)
        {
            if (IsCachDouble)
            {
                if (_cacheDouble.ContainsKey(type))
                {
                    var cache = _cacheDouble[type];
                    cache.Remove(value);
                }
            }
        }

        internal static void RemoveCacheDouble(Type type, object[] value)
        {
            if (IsCachDouble)
            {
                foreach (var o in value)
                {
                    RemoveCacheDouble(type, o);
                }
            }
        }

        internal static object GetCacheDouble(Type type, object key)
        {
            if (IsCachDouble)
            {
                if (_cacheDouble.ContainsKey(type))
                {
                    var cache = _cacheDouble[type];
                    var obj = cache[key];
                    if (obj != null)
                    {
                        return obj;
                    }
                }
                return null;
            }
            return null;
        }

        internal static void ClearCacheDouble()
        {
            foreach (var cache in _cacheDouble)
            {
                cache.Value.Clear();
            }
        }

        internal static void ClearCacheDouble(Type type)
        {
            if (_cacheDouble.ContainsKey(type))
            {
                var cache = _cacheDouble[type];
                cache.Clear();
            }
        }

        internal static object GetPkValue(object tag)
        {
            TableSchema table = DBQueryFactory.GetTableSchema(tag.GetType());
            var info = (table.EntityInfo);
            if (info == null) throw new Exception("T isn't baseEntity");
            return ReflectionUtils.GetFieldValue(tag, table.IdentyFieldMemberInfo);
        }

        #endregion 
    } 
}
