using System;

namespace NSun.Data.Mapper
{
    internal struct MapperCacheKey
    {
        public Type FromType;
        public Type ToType;

        public MapperCacheKey(Type fromType, Type toType)
        {
            FromType = fromType;
            ToType = toType;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (obj.GetType() != typeof(MapperCacheKey))
                return false;

            var right = (MapperCacheKey)obj;
            return right.FromType == FromType && right.ToType == ToType;
        }

        public override int GetHashCode()
        {
            return FromType.GetHashCode() + ToType.GetHashCode();
        }
    }
}
