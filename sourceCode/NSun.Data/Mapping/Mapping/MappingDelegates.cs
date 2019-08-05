namespace NSun.Data.Mapper
{
    internal delegate void InternalMapperProxy(object fac, object from, ref object to);

    /// <summary>
    /// The mapper delegate represents a mapper from TFrom type to TTo type
    /// </summary>
    public delegate TTo Mapper<TFrom, TTo>(TFrom from);

    internal delegate void InternalMapper<TFrom, TTo>(MapperFactory fac, TFrom from, ref TTo to);

    /// <summary>
    /// Represents get a value from a TFrom type instance
    /// </summary>
    public delegate TValue MappingFrom<TFrom, TValue>(TFrom from);

    /// <summary>
    /// Represents set a value on the TTo instance
    /// </summary>
    public delegate TTo MappingTo<TTo, TValue>(TTo to, TValue value);
}
