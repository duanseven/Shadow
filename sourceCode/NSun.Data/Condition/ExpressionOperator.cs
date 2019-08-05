using System;
using System.Runtime.Serialization;

namespace NSun.Data
{
    [Serializable]
    [DataContract(Namespace = "http://nsun-shadow.com")]
    public enum ExpressionOperator
    {
        [EnumMember]
        None,
        [EnumMember]
        Equals,
        [EnumMember]
        NotEquals,
        [EnumMember]
        In,
        [EnumMember]
        GreaterThan,
        [EnumMember]
        GreaterThanOrEquals,
        [EnumMember]
        LessThan,
        [EnumMember]
        LessThanOrEquals,
        [EnumMember]
        Like,
        [EnumMember]
        Escape,
        [EnumMember]
        Is,
        [EnumMember]
        IsNot,
        [EnumMember]
        Add,
        [EnumMember]
        Subtract,
        [EnumMember]
        Multiply,
        [EnumMember]
        Divide,
        [EnumMember]
        Mod,
        [EnumMember]
        BitwiseAnd,
        [EnumMember]
        BitwiseOr,
        [EnumMember]
        BitwiseXor,
        [EnumMember]
        BitwiseNot,
        [EnumMember]
        Exists,
        [EnumMember]
        Link,
        [EnumMember]
        As
    }
}
