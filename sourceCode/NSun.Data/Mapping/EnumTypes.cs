using System;
using System.Runtime.Serialization;

namespace NSun.Data
{
    public enum LoadType
    {
        Lazy,
        Read
    }

    public enum ProxyType
    {
        Remoting,
        Castle
    }

    public enum SqlType
    {
        Sqlserver8 = 0,
        Sqlserver9 = 1,
        Sqlserver10 = 2,
        MsAccess = 3,
        Oracle = 4,
        Oracle9 = 5,
        MySql = 6,
        Db2 = 7,
        Sqlite = 8,
        PostgreSql = 9,
        MySql8 = 10
    }

    //实体状态
    public enum EntityState
    {
        Save,
        Update,
        Delele,
        None
    }
    //UnitofWork状态
    public enum UnitofWorkType
    {
        Add,
        Update,
        Del,
        Exec,
        Select,
        None
    }
    //关系枚举
    public enum RelationType
    {
        OneToOne,
        OneToMany,
        ManyToOne,
        ManyToMany
    }

    public enum UpdateCommand
    {
        Save = 0,
        Update = 1
    }

    public enum LogType
    {
        File = 0,
        Database = 1
    }

    [Serializable]
    [DataContract(Namespace = "http://nsun-shadow.com")]
    public enum ConditionAndOr
    {
        [EnumMember]
        And,
        [EnumMember]
        Or,
        [EnumMember]
        Space
    }

    public enum JoinType
    {
        Inner,
        Left,
        Right,
        Full
    }
}
