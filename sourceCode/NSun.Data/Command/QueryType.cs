using System;
using System.Runtime.Serialization;

namespace NSun.Data
{
    [Serializable]
    [DataContract(Namespace = "http://nsun-shadow.com")]
    public enum QueryType
    {
        [EnumMember]
        Select,
        [EnumMember]
        Insert,
        [EnumMember]
        Update,
        [EnumMember]
        Delete,
        [EnumMember]
        Sproc,
        [EnumMember]
        Custom
    }
}