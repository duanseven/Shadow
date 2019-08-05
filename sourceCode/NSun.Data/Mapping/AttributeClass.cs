﻿using System;
using System.Data;

//NSun All Entity Attribute
namespace NSun.Data
{
    [Serializable, AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class PrimaryKeyAttribute : Attribute
    {
        private readonly bool _isAutoGenerated;

        public bool IsAutoGenerated
        {
            get
            {
                return _isAutoGenerated;
            }
        }

        public PrimaryKeyAttribute()
        {
            _isAutoGenerated = false;
        }

        public PrimaryKeyAttribute(bool isAutoGenerated)
        {
            _isAutoGenerated = isAutoGenerated;
        }
    }

    [Serializable, AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class UnicodeAttribute : Attribute
    {
        private readonly bool _isUnicode;

        public bool IsUnicode
        {
            get
            {
                return _isUnicode;
            }
        }

        public UnicodeAttribute()
        {
            _isUnicode = false;
        }

        public UnicodeAttribute(bool isUnicode)
        {
            _isUnicode = isUnicode;
        }
    }

    [Serializable, AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class TableAttribute : Attribute
    {
        private readonly string _tablename;

        public string TableName
        {
            get
            {
                return _tablename;
            }
        }

        public TableAttribute(string tablename)
        {
            _tablename = tablename;
        }
    }

    [Serializable, AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class TableMappingAttribute : Attribute
    {
        public Type MappingType { get; set; }

        public TableMappingAttribute(Type type)
        {
            MappingType = type;
        }

        public TableMappingAttribute(string type)
        {
            var c = type.Split(',');

            MappingType = System.Reflection.Assembly.Load(c[0]).GetType(c[1]);
        }
    }

    #region 存储过程映射

    [Serializable, AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class ProcedureAttribute : Attribute
    {
        private readonly string _procedurename;

        public string ProcedureName
        {
            get
            {
                return _procedurename;
            }
        }

        public ProcedureAttribute(string procedurename)
        {
            _procedurename = procedurename;
        }
    }

    [Serializable, AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ParameterAttribute : Attribute
    {
        #region Property

        private string _parameterName;

        public string ParameterName
        {
            get { return _parameterName; }
            set { _parameterName = value; }
        }

        private System.Data.DbType _dbType;

        public System.Data.DbType DbType
        {
            get { return _dbType; }
            set { _dbType = value; }
        }

        #endregion

        //new QueryColumn("USERS.NAME", System.Data.DbType.String);
        public ParameterAttribute(string parameterName, System.Data.DbType dbType)
        {
            _parameterName = parameterName;
            _dbType = dbType;
        }
    }

    [Serializable, AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class InputParameterAttribute : ParameterAttribute
    {
        public InputParameterAttribute(string parameterName, System.Data.DbType dbType) : base(parameterName, dbType)           { }
    }

    [Serializable, AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class OutputParameterAttribute : ParameterAttribute
    {
        private int _size;

        public int Size
        {
            get { return _size; }
            set { _size = value; }
        }

        public OutputParameterAttribute(string parameterName, System.Data.DbType dbType, int size)
            : base(parameterName, dbType)
        {
            _size = size;
        }
    }

    [Serializable, AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class InputOutputParameterAttribute : OutputParameterAttribute
    {
        public InputOutputParameterAttribute(string parameterName, System.Data.DbType dbType, int size) : base(parameterName, dbType, size) { }
    }

    [Serializable, AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class ReturnParameterAttribute : OutputParameterAttribute
    {
        public ReturnParameterAttribute(string parameterName, System.Data.DbType dbType, int size) : base(parameterName, dbType, size) { }
    } 

    [Serializable, AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class OptimisticLockingAttribute : Attribute{ }

    #endregion

    //TODO 需要继续修改映射属性的特性
    [Serializable, AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class HashSetMappingAttribute : Attribute
    {
        public HashSetMappingAttribute(Type type, string fkcolumnfieldname, string pkcolumnfieldname, bool isLazy)
        {
            _isLazy = isLazy;
            FkType = type;
            FkQueryColumn = fkcolumnfieldname;
            PkQueryColumn = pkcolumnfieldname;
        }
        public HashSetMappingAttribute(Type type, string fkcolumnfieldname, string pkcolumnfieldname)
            : this(type, fkcolumnfieldname, pkcolumnfieldname, true)
        {
        }

        public string FkQueryColumn { get; set; }

        public string PkQueryColumn { get; set; }

        public Type FkType { get; set; }

        private readonly bool _isLazy;

        public bool IsLazy
        {
            get
            {
                return _isLazy;
            }
        }
    }
}
