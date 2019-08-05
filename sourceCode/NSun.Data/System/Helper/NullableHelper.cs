using System;
using System.Collections.Specialized;
using System.Data;
using System.Reflection;

namespace NSun.Data.Helper
{
    internal class NullableHelper
    {
        private static readonly HybridDictionary NullableTypesToDataType;
        private static readonly HybridDictionary NullableTypesToUnderlyingType;
        private static readonly HybridDictionary NullableTypeToUnderlyingTypes;

        static NullableHelper()
        {
            NullableTypesToDataType = new HybridDictionary();
            NullableTypesToDataType[typeof(Nullable<bool>)] = DbType.Boolean;
            NullableTypesToDataType[typeof(Nullable<byte>)] = DbType.Byte;
            NullableTypesToDataType[typeof(Nullable<DateTime>)] = DbType.DateTime;
            NullableTypesToDataType[typeof(Nullable<Date>)] = DbType.Date;
            NullableTypesToDataType[typeof(Nullable<Time>)] = DbType.Time;
            NullableTypesToDataType[typeof(Nullable<decimal>)] = DbType.Decimal;
            NullableTypesToDataType[typeof(Nullable<double>)] = DbType.Double;
            NullableTypesToDataType[typeof(Nullable<float>)] = DbType.Single;
            NullableTypesToDataType[typeof(Nullable<sbyte>)] = DbType.SByte;
            NullableTypesToDataType[typeof(Nullable<int>)] = DbType.Int32;
            NullableTypesToDataType[typeof(Nullable<long>)] = DbType.Int64;
            NullableTypesToDataType[typeof(Nullable<short>)] = DbType.Int16;
            NullableTypesToDataType[typeof(Nullable<uint>)] = DbType.UInt32;
            NullableTypesToDataType[typeof(Nullable<ulong>)] = DbType.UInt64;
            NullableTypesToDataType[typeof(Nullable<ushort>)] = DbType.UInt16;
            NullableTypesToDataType[typeof(Nullable<Guid>)] = DbType.Guid;
            NullableTypesToDataType[typeof(Nullable<TimeSpan>)] = DbType.Double;

            NullableTypesToUnderlyingType = new HybridDictionary();
            NullableTypesToUnderlyingType[typeof(Nullable<bool>)] = typeof(Boolean);
            NullableTypesToUnderlyingType[typeof(Nullable<byte>)] = typeof(Byte);
            NullableTypesToUnderlyingType[typeof(Nullable<DateTime>)] = typeof(DateTime);
            NullableTypesToUnderlyingType[typeof(Nullable<Date>)] = typeof(Date);
            NullableTypesToUnderlyingType[typeof(Nullable<Time>)] = typeof(Time);
            NullableTypesToUnderlyingType[typeof(Nullable<decimal>)] = typeof(Decimal);
            NullableTypesToUnderlyingType[typeof(Nullable<double>)] = typeof(Double);
            NullableTypesToUnderlyingType[typeof(Nullable<float>)] = typeof(Single);
            NullableTypesToUnderlyingType[typeof(Nullable<int>)] = typeof(Int32);
            NullableTypesToUnderlyingType[typeof(Nullable<long>)] = typeof(Int64);
            NullableTypesToUnderlyingType[typeof(Nullable<sbyte>)] = typeof(SByte);
            NullableTypesToUnderlyingType[typeof(Nullable<short>)] = typeof(Int16);
            NullableTypesToUnderlyingType[typeof(Nullable<uint>)] = typeof(UInt32);
            NullableTypesToUnderlyingType[typeof(Nullable<ulong>)] = typeof(UInt64);
            NullableTypesToUnderlyingType[typeof(Nullable<ushort>)] = typeof(UInt16);
            NullableTypesToUnderlyingType[typeof(Nullable<Guid>)] = typeof(Guid);
            NullableTypesToUnderlyingType[typeof(Nullable<TimeSpan>)] = typeof(Double);

            NullableTypeToUnderlyingTypes = new HybridDictionary();
            NullableTypeToUnderlyingTypes[typeof(Boolean)] = typeof(Nullable<bool>);
            NullableTypeToUnderlyingTypes[typeof(Byte[])] = typeof(Nullable<byte>[]);
            NullableTypeToUnderlyingTypes[typeof(Byte)] = typeof(Nullable<byte>);
            NullableTypeToUnderlyingTypes[typeof(DateTime)] = typeof(Nullable<DateTime>);
            NullableTypeToUnderlyingTypes[typeof(Date)] = typeof(Nullable<Date>);
            NullableTypeToUnderlyingTypes[typeof(Time)] = typeof(Nullable<Time>);
            NullableTypeToUnderlyingTypes[typeof(Decimal)] = typeof(Nullable<decimal>);
            NullableTypeToUnderlyingTypes[typeof(Double)] = typeof(Nullable<double>);
            NullableTypeToUnderlyingTypes[typeof(Single)] = typeof(Nullable<float>);
            NullableTypeToUnderlyingTypes[typeof(Int32)] = typeof(Nullable<int>);
            NullableTypeToUnderlyingTypes[typeof(Int64)] = typeof(Nullable<long>);
            NullableTypeToUnderlyingTypes[typeof(SByte)] = typeof(Nullable<sbyte>);
            NullableTypeToUnderlyingTypes[typeof(Int16)] = typeof(Nullable<short>);
            NullableTypeToUnderlyingTypes[typeof(UInt32)] = typeof(Nullable<uint>);
            NullableTypeToUnderlyingTypes[typeof(UInt64)] = typeof(Nullable<ulong>);
            NullableTypeToUnderlyingTypes[typeof(UInt16)] = typeof(Nullable<ushort>);
            NullableTypeToUnderlyingTypes[typeof(Guid)] = typeof(Nullable<Guid>);
            NullableTypeToUnderlyingTypes[typeof(Double)] = typeof(Nullable<TimeSpan>);
        }

        public static bool IsNullableType(Type t)
        {
            return NullableTypesToDataType.Contains(t);
        }

        public static DbType GetDataType(Type nullableType)
        {
            return (DbType)NullableTypesToDataType[nullableType];
        }

        public static Type GetUnderlyingType(Type nullableType)
        {
            return (Type)NullableTypesToUnderlyingType[nullableType];
        }

        public static Type GetUnderlyingNullType(Type type)
        {
            return (Type)NullableTypeToUnderlyingTypes[type];
        } 

        public static ConstructorInfo GetConstructorInfo(Type nullableType)
        {
            if (!IsNullableType(nullableType))
            {
                throw new ArgumentOutOfRangeException();
            }
            ConstructorInfo ci = nullableType.GetConstructor(
                new[] { GetUnderlyingType(nullableType) });
            return ci;
        }

        public static object CreateNullableObject(Type nullableType, object value)
        {
            ConstructorInfo ci = GetConstructorInfo(nullableType);
            return ci.Invoke(new[] { value });
        }
    }
}
