using System;
using System.Runtime.Serialization;

namespace NSun.Data
{ 
    public interface IBaseEntity : IQueryTable
    {
        bool Equals(object obj);
        int GetHashCode(); 
        Guid IdentityKey { get; set; }
        bool IsPersistence();
        void SetIsPersistence(bool ispersistence);
    }
}
