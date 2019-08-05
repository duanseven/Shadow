using System;
using System.Runtime.Serialization;

namespace NSun.Data
{
    [Serializable]
    [DataContract(Namespace = "http://nsun-shadow.com")]
    public enum SprocParameterDirection
    {
        [EnumMember]
        Input,
        [EnumMember]
        InputOutput,
        [EnumMember]
        Output,
        [EnumMember]
        ReturnValue
    }
}
