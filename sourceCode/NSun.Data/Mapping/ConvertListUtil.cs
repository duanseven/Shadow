using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
namespace NSun.Data
{
    public static class ConvertListUtil
    {
        public const string CompareFullName = "NSun.Data.IBaseEntity";

        #region Convert Method

        //internal static IEnumerable<T> DataToIEnumerable<T>(IDataReader dr)
        //{
        //    if (typeof(T).IsSubclassOf(typeof(BaseEntity)))
        //    {
        //        using (dr)
        //        {
        //            while (dr.Read())
        //            {
        //                yield return DynamicBuilder<T>.Build(dr);
        //            }
        //        }
        //    }
        //    else
        //    {
        //        using (dr)
        //        {
        //            while (dr.Read())
        //            {
        //                yield return (T)dr.GetValue(0);
        //            }
        //        }
        //    }
        //}

        internal static IEnumerable<T> DataToIEnumerable<T>(IDataReader dr, string commandtext)
        {

            if (typeof(T).GetInterface(CompareFullName)!=null)
            {
                using (dr)
                {
                    while (dr.Read())
                    {
                        yield return DynamicBuilder<T>.Build(dr, commandtext);
                    }
                }
            }
            else
            {
                using (dr)
                {
                    while (dr.Read())
                    {
                        yield return (T)dr.GetValue(0);
                    }
                }
            }
        }

        //internal static List<T> DataToList<T>(IDataReader dr)
        //{
        //    if (typeof(T).IsSubclassOf(typeof(BaseEntity)))
        //    {
        //        List<T> result = new List<T>();
        //        using (dr)
        //        {
        //            while (dr.Read())
        //            {
        //                result.Add(DynamicBuilder<T>.Build(dr));
        //            }
        //        }
        //        return result;
        //    }
        //    return DataToListNotEntity<T>(dr);
        //}

        internal static List<T> DataToList<T>(IDataReader dr, string commandtext)
        {
            if (typeof(T).GetInterface(CompareFullName) != null)
            {
                List<T> result = new List<T>();
                using (dr)
                {
                    while (dr.Read())
                    {
                        result.Add(DynamicBuilder<T>.Build(dr, commandtext));
                    }
                }
                return result;
            }
            return DataToListNotEntity<T>(dr);
        }

        internal static System.Collections.IList DataToList(Type type, IDataReader dr, DBQuery db, string commandtext)
        {
            if (type.GetInterface(CompareFullName) != null)
            {
                var dyb = new DynamicBuilder();
                Type constructed = typeof(NSun.Data.Collection.List<>).MakeGenericType(type);
                MethodInfo info = constructed.GetMethod("Add", new Type[] { type });

                var result = Activator.CreateInstance(constructed, db);
                using (dr)
                {
                    while (dr.Read())
                    {
                        info.Invoke(result, new[] { dyb.Build(type, dr, commandtext) });
                    }
                }
                return (System.Collections.IList)result;
            }
            return null;//DataToListNotEntity<T>(dr);
        }

        //internal static T DataToEntity<T>(IDataReader dr)
        //{
        //    T _t = default(T);
        //    using (dr)
        //    {
        //        if (dr.Read())
        //        {
        //            _t = DynamicBuilder<T>.Build(dr);
        //        }
        //    }
        //    if (null != _t && _t is BaseEntity)
        //    {
        //        var s = _t as BaseEntity;
        //        s.SetIsPersistence(true);
        //    }
        //    else
        //    {
        //        _t = Activator.CreateInstance<T>();
        //    }
        //    return _t;
        //}

        internal static T DataToEntity<T>(IDataReader dr, string commandtext)
        {
            T t = default(T);
            using (dr)
            {
                if (dr.Read())
                {
                    t = DynamicBuilder<T>.Build(dr, commandtext);
                }
            }
            if (!ReferenceEquals(t, null))
            {
                var s = t as IBaseEntity;
                if (s != null)
                    s.SetIsPersistence(true);
            }
            else
            {
                t = Activator.CreateInstance<T>();
            }
            return t;
        }

        internal static object DataToEntity(Type type, IDataReader dr, string commandtext)
        {
            //T t = default();
            object t = CommonUtils.DefaultValue(type);
            DynamicBuilder dyb = new DynamicBuilder();
            using (dr)
            {
                if (dr.Read())
                {
                    t = dyb.Build(type, dr, commandtext);
                }
            }
            if (!ReferenceEquals(t, null))
            {
                var s = t as IBaseEntity;
                if (s != null)
                    s.SetIsPersistence(true);
            }
            else
            {
                t = Activator.CreateInstance(type); //Activator.CreateInstance<T>();
            }
            return t;
        }

        internal static object DataToEntityOrDefault(Type type, IDataReader dr, string commandtext)
        {
            object t = CommonUtils.DefaultValue(type);
            DynamicBuilder dyb = new DynamicBuilder();
            using (dr)
            {
                if (dr.Read())
                {
                    t = dyb.Build(type, dr, commandtext);
                }
            }
            if (!ReferenceEquals(t, null))
            {
                var s = t as IBaseEntity;
                if (s != null)
                    s.SetIsPersistence(true);
            }
            return t;
        }

        internal static T DataToEntityOrDefault<T>(IDataReader dr, string commandtext)
        {
            T t = default(T);
            using (dr)
            {
                if (dr.Read())
                {
                    t = DynamicBuilder<T>.Build(dr, commandtext);
                }
            }
            if (!ReferenceEquals(t, null))
            {
                var s = t as IBaseEntity;
                if (s != null)
                    s.SetIsPersistence(true);
            }
            return t;
        }

        internal static List<T> DataToListNotEntity<T>(IDataReader dr)
        {
            List<T> result = new List<T>();
            using (dr)
            {
                while (dr.Read())
                {
                    result.Add((T)dr.GetValue(0));
                }
            }
            return result;
        }

        /// <summary>
        /// DataTable to list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static IList<T> DataTableToList<T>(DataTable dt)
        {
            if (dt == null)
                return null;
            IList<T> result = new List<T>();
            for (int j = 0; j < dt.Rows.Count; j++)
            {
                T _t = Activator.CreateInstance<T>();
                PropertyInfo[] propertys = _t.GetType().GetProperties();
                foreach (PropertyInfo pi in propertys)
                {
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        // 属性与字段名称一致的进行赋值 
                        if (pi.Name.ToLower().Equals(dt.Columns[i].ColumnName.ToLower()))
                        {
                            if (dt.Rows[j][i] != DBNull.Value)
                            {
                                pi.SetValue(_t, dt.Rows[j][i], null);
                            }
                            else
                                pi.SetValue(_t, null, null);
                            break;
                        }
                    }
                }
                result.Add(_t);
            }
            return result;
        }

        /// <summary>
        /// Ilist<T> to DataSet
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static DataTable ConvertToDataTable<T>(IList<T> list)
        {
            if (list == null || list.Count <= 0)
            {
                return null;
            }
            DataTable dt = new DataTable(typeof(T).Name);
            DataColumn column;
            DataRow row;
            PropertyInfo[] myPropertyInfo = typeof(T).GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            foreach (T t in list)
            {
                if (t == null)
                    continue;
                row = dt.NewRow();
                for (int i = 0, j = myPropertyInfo.Length; i < j; i++)
                {
                    PropertyInfo pi = myPropertyInfo[i];
                    string name = pi.Name;
                    if (dt.Columns[name] == null)
                    {
                        column = new DataColumn(name, pi.PropertyType);
                        dt.Columns.Add(column);
                    }
                    row[name] = pi.GetValue(t, null);
                }
                dt.Rows.Add(row);
            }

            return dt;
        }
        #endregion

        #region Convert Type


        public static DbType GetDbType(Type type, bool isunicode)
        {
            if (type.IsEnum)
                return DbType.Int32;
            if (type == typeof(long) || type == typeof(long?))
                return DbType.Int64;
            if (type == typeof(int) || type == typeof(int?))
                return DbType.Int32;
            if (type == typeof(short) || type == typeof(short?))
                return DbType.Int16;
            if (type == typeof(byte) || type == typeof(byte?))
                return DbType.Byte;
            if (type == typeof(bool) || type == typeof(bool?))
                return DbType.Boolean;
            if (type == typeof(decimal) || type == typeof(decimal?))
                return DbType.Decimal;
            if (type == typeof(float) || type == typeof(float?))
                return DbType.Double;
            if (type == typeof(double) || type == typeof(double?))
                return DbType.Double;
            if (type == typeof(string))
                return isunicode ? DbType.String : DbType.AnsiString;
            if (type == typeof(DateTime) || type == typeof(DateTime?))
                return DbType.DateTime;
            if (type == typeof(char) || type == typeof(char?))
                return DbType.Byte;
            if (type == typeof(byte[]))
                return DbType.Binary;
            if (type == typeof(Guid) || type == typeof(Guid?))
                return DbType.Guid;
            return DbType.String;
        }

        public static string GetTypeName(string p, bool p_2)
        {
            var s = GetTypeName(p);
            if (p_2)
            {
                if (s == typeof(string).Name || s == typeof(byte[]).Name)
                {
                    return s;
                }
                return s + "?";
            }
            return s;
        }

        public static Type GetType(string p, bool p_2)
        {
            var s = GetType(p);
            if (p_2)
            {
                if (s == typeof(string) || s == typeof(byte[]))
                {
                    return s;
                }
                return NSun.Data.Helper.NullableHelper.GetUnderlyingNullType(s);
            }
            return s;
        }

        public static string GetTypeName(string dbType)
        {
            return GetType(dbType).Name;
        }

        public static Type GetType(string dbType)
        {
            switch (dbType)
            {
                case "AnsiString":
                case "AnsiStringFixedLength":
                case "String":
                case "StringFixedLength":
                case "Xml":
                    return typeof(string);
                case "Binary":
                case "Byte[]":
                    return typeof(byte[]);
                case "Boolean":
                    return typeof(bool);
                case "Byte":
                    return typeof(byte);
                case "Currency":
                case "Decimal":
                    return typeof(decimal);
                case "Date":
                case "DateTime":
                case "Time":
                    return typeof(DateTime);//+ (chktimeask.Checked ? "?" : string.Empty);
                case "Double":
                case "VarNumeric":
                    return typeof(double);
                case "Single":
                    return typeof(float);
                case "Guid":
                    return typeof(Guid);
                case "Int16":
                    return typeof(short);
                case "Int32":
                    return typeof(int);
                case "Int64":
                    return typeof(long);
                case "SByte":
                    return typeof(sbyte);
                case "UInt16":
                    return typeof(ushort);
                case "UInt32":
                    return typeof(uint);
                case "UInt64":
                    return typeof(ulong);
            }
            return typeof(string);
        }

        public static string GetDbType(string type)
        {
            switch (type)
            {
                case "Byte[]":
                    type = "System.Data.DbType.Binary";
                    break;
                default: type = "System.Data.DbType." + type;
                    break;
            }
            return type;
        }

        public static Type GetTypeFromDbType(DbType dbType)
        {
            switch (dbType)
            {
                case DbType.AnsiString:
                case DbType.AnsiStringFixedLength:
                case DbType.String:
                case DbType.StringFixedLength:
                case DbType.Xml:
                    return typeof(string);
                case DbType.Binary:
                    return typeof(byte[]);
                case DbType.Boolean:
                    return typeof(bool);
                case DbType.Byte:
                    return typeof(byte);
                case DbType.Currency:
                case DbType.Decimal:
                case DbType.VarNumeric:
                    return typeof(decimal);
                case DbType.Date:
                case DbType.DateTime:
                case DbType.DateTime2:
                case DbType.DateTimeOffset:
                case DbType.Time:
                    return typeof(DateTime);
                case DbType.Double:
                    return typeof(double);
                case DbType.Single:
                    return typeof(float);
                case DbType.Guid:
                    return typeof(Guid);
                case DbType.Int16:
                    return typeof(short);
                case DbType.Int32:
                    return typeof(int);
                case DbType.Int64:
                    return typeof(long);
                case DbType.SByte:
                    return typeof(sbyte);
                case DbType.UInt16:
                    return typeof(ushort);
                case DbType.UInt32:
                    return typeof(uint);
                case DbType.UInt64:
                    return typeof(ulong);
                case DbType.Object:
                    return typeof(object);
            }
            return typeof(string);
        }

        public static Type SqlType2CsharpType(SqlDbType sqlType)
        {
            switch (sqlType)
            {
                case SqlDbType.BigInt:
                    return typeof(Int64);
                case SqlDbType.Binary:
                    return typeof(Object);
                case SqlDbType.Bit:
                    return typeof(Boolean);
                case SqlDbType.Char:
                    return typeof(String);
                case SqlDbType.DateTime:
                    return typeof(DateTime);
                case SqlDbType.Decimal:
                    return typeof(Decimal);
                case SqlDbType.Float:
                    return typeof(Double);
                case SqlDbType.Image:
                    return typeof(Object);
                case SqlDbType.Int:
                    return typeof(Int32);
                case SqlDbType.Money:
                    return typeof(Decimal);
                case SqlDbType.NChar:
                    return typeof(String);
                case SqlDbType.NText:
                    return typeof(String);
                case SqlDbType.NVarChar:
                    return typeof(String);
                case SqlDbType.Real:
                    return typeof(Single);
                case SqlDbType.SmallDateTime:
                    return typeof(DateTime);
                case SqlDbType.SmallInt:
                    return typeof(Int16);
                case SqlDbType.SmallMoney:
                    return typeof(Decimal);
                case SqlDbType.Text:
                    return typeof(String);
                case SqlDbType.Timestamp:
                    return typeof(Object);
                case SqlDbType.TinyInt:
                    return typeof(Byte);
                case SqlDbType.Udt://自定义的数据类型
                    return typeof(Object);
                case SqlDbType.UniqueIdentifier:
                    return typeof(Object);
                case SqlDbType.VarBinary:
                    return typeof(Object);
                case SqlDbType.VarChar:
                    return typeof(String);
                case SqlDbType.Variant:
                    return typeof(Object);
                case SqlDbType.Xml:
                    return typeof(Object);
                default:
                    return null;
            }
        }

        public static SqlDbType SqlTypeString2SqlType(string sqlTypeString)
        {
            SqlDbType dbType = SqlDbType.Variant;//默认为Objectswitch 
            switch (sqlTypeString)
            {
                case
                    "int": dbType =
            SqlDbType.Int;
                    break;
                case "varchar":
                    dbType = SqlDbType.VarChar;
                    break;
                case
                    "bit": dbType =
            SqlDbType.Bit;
                    break;
                case "datetime":
                    dbType = SqlDbType.DateTime;
                    break;
                case
                    "decimal": dbType =
            SqlDbType.Decimal;
                    break;
                case "float":
                    dbType = SqlDbType.Float;
                    break;
                case
                    "image": dbType =
            SqlDbType.Image;
                    break;
                case "money":
                    dbType = SqlDbType.Money;
                    break;
                case
                    "ntext": dbType =
            SqlDbType.NText;
                    break;
                case "nvarchar":
                    dbType = SqlDbType.NVarChar;
                    break;
                case
                    "smalldatetime": dbType =
            SqlDbType.SmallDateTime;
                    break;
                case "smallint":
                    dbType = SqlDbType.SmallInt;
                    break;
                case
                    "text": dbType =
            SqlDbType.Text;
                    break;
                case "bigint":
                    dbType = SqlDbType.BigInt;
                    break;
                case
                    "binary": dbType =
            SqlDbType.Binary;
                    break;
                case "char":
                    dbType = SqlDbType.Char;
                    break;
                case
                    "nchar": dbType =
            SqlDbType.NChar;
                    break;
                case "numeric":
                    dbType = SqlDbType.Decimal;
                    break;
                case
                    "real": dbType =
            SqlDbType.Real;
                    break;
                case
                    "smallmoney": dbType =
            SqlDbType.SmallMoney;
                    break;
                case
                    "sql_variant": dbType =
            SqlDbType.Variant;
                    break;
                case
                    "timestamp": dbType =
            SqlDbType.Timestamp;
                    break;
                case "tinyint":
                    dbType = SqlDbType.TinyInt;
                    break;
                case
                    "uniqueidentifier": dbType =
            SqlDbType.UniqueIdentifier;
                    break;
                case
                    "varbinary": dbType =
            SqlDbType.VarBinary;
                    break;
                case "xml":
                    dbType = SqlDbType.Xml;
                    break;
            } return dbType;
        }
        #endregion

    }
}
