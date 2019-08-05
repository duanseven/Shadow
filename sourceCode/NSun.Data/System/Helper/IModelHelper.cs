using System;
namespace NSun.Data.Helper
{
    public interface IModelHelper
    {
        DBQuery DB { get; }
        object CreateProxy(Type type);
        object CreateProxy(Type type, object obj);
        T CreateProxy<T>() where T : class;
        T CreateProxy<T>(T obj) where T : class;
    }
}
