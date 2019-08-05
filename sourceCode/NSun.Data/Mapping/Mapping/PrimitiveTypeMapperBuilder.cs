using System;

namespace NSun.Data.Mapper
{
    internal sealed class PrimitiveTypeMapperBuilder : MapperBuilder
    {
        private readonly MapperCacheKey _cacheKey;
        private readonly bool _isFromTypeNullable;
        private readonly Type _fromType;
        private readonly TypeCode _fromTypeCode;
        private readonly bool _isToTypeNullable;
        private readonly Type _toType;
        private readonly TypeCode _toTypeCode;

        #region Constructors

        public PrimitiveTypeMapperBuilder(Type fromType, Type toType)
        {
            if (fromType == null)
                throw new ArgumentNullException("fromType");
            if (toType == null)
                throw new ArgumentNullException("toType");

            _cacheKey = new MapperCacheKey(fromType, toType);
            _isFromTypeNullable = MappingHelper.IsNullableType(fromType);
            _fromType = _isFromTypeNullable ? fromType.GetGenericArguments()[0] : fromType;
            _fromTypeCode = Type.GetTypeCode(_fromType);
            _isToTypeNullable = MappingHelper.IsNullableType(_cacheKey.ToType);
            _toType = _isToTypeNullable ? toType.GetGenericArguments()[0] : toType;
            _toTypeCode = Type.GetTypeCode(_toType);

            if (_fromTypeCode == TypeCode.Object || _toTypeCode == TypeCode.Object)
                throw new ArgumentException("fromType and toType could only be primitive types for constructor of PrimitiveTypeMapperBuilder");
        }

        #endregion

        #region Public Methods

        public static bool IsPrimitiveTypeMapping(Type fromType, Type toType)
        {
            if (fromType == null)
                return false;
            if (toType == null)
                return false;
            if (MappingHelper.IsNullableType(fromType))
                fromType = fromType.GetGenericArguments()[0];
            if (Type.GetTypeCode(fromType) == TypeCode.Object)
                return false;
            if (MappingHelper.IsNullableType(toType))
                toType = toType.GetGenericArguments()[0];
            if (Type.GetTypeCode(toType) == TypeCode.Object)
                return false;

            return true;
        }

        #endregion

        #region Non-Public Methods

        internal override MapperCacheKey CacheKey
        {
            get { return _cacheKey; }
        }

        internal override Delegate BuildMapper()
        {
            Delegate result = null;

            switch (_fromTypeCode)
            {
                #region case Boolean

                case TypeCode.Boolean:
                    switch (_toTypeCode)
                    {
                        case TypeCode.Boolean:
                            if (!_isFromTypeNullable && !_isToTypeNullable)
                                result = new InternalMapper<bool, bool>(delegate(MapperFactory fac, bool from, ref bool to) { to = from; });
                            else if (_isFromTypeNullable && _isToTypeNullable)
                                result = new InternalMapper<bool?, bool?>(delegate(MapperFactory fac, bool? from, ref bool? to) { to = from; });
                            else if (_isFromTypeNullable && !_isToTypeNullable)
                                result = new InternalMapper<bool?, bool>(delegate(MapperFactory fac, bool? from, ref bool to) { to = from.HasValue ? from.Value : default(bool); });
                            else if (!_isFromTypeNullable && _isToTypeNullable)
                                result = new InternalMapper<bool, bool?>(delegate(MapperFactory fac, bool from, ref bool? to) { to = from; });
                            break;
                    }
                    break;

                #endregion

                #region case Byte

                case TypeCode.Byte:
                    switch (_toTypeCode)
                    {
                        case TypeCode.Byte:
                            if (!_isFromTypeNullable && !_isToTypeNullable)
                                result = new InternalMapper<byte, byte>(delegate(MapperFactory fac, byte from, ref byte to) { to = from; });
                            else if (_isFromTypeNullable && _isToTypeNullable)
                                result = new InternalMapper<byte?, byte?>(delegate(MapperFactory fac, byte? from, ref byte? to) { to = from; });
                            else if (_isFromTypeNullable && !_isToTypeNullable)
                                result = new InternalMapper<byte?, byte>(delegate(MapperFactory fac, byte? from, ref byte to) { to = from.HasValue ? from.Value : default(byte); });
                            else if (!_isFromTypeNullable && _isToTypeNullable)
                                result = new InternalMapper<byte, byte?>(delegate(MapperFactory fac, byte from, ref byte? to) { to = from; });
                            break;
                        case TypeCode.Decimal:
                            if (!_isFromTypeNullable && !_isToTypeNullable)
                                result = new InternalMapper<byte, decimal>(delegate(MapperFactory fac, byte from, ref decimal to) { to = from; });
                            else if (_isFromTypeNullable && _isToTypeNullable)
                                result = new InternalMapper<byte?, decimal?>(delegate(MapperFactory fac, byte? from, ref decimal? to) { to = from; });
                            else if (_isFromTypeNullable && !_isToTypeNullable)
                                result = new InternalMapper<byte?, decimal>(delegate(MapperFactory fac, byte? from, ref decimal to) { to = from.HasValue ? from.Value : default(byte); });
                            else if (!_isFromTypeNullable && _isToTypeNullable)
                                result = new InternalMapper<byte, decimal?>(delegate(MapperFactory fac, byte from, ref decimal? to) { to = from; });
                            break;
                        case TypeCode.Double:
                            if (!_isFromTypeNullable && !_isToTypeNullable)
                                result = new InternalMapper<byte, double>(delegate(MapperFactory fac, byte from, ref double to) { to = from; });
                            else if (_isFromTypeNullable && _isToTypeNullable)
                                result = new InternalMapper<byte?, double?>(delegate(MapperFactory fac, byte? from, ref double? to) { to = from; });
                            else if (_isFromTypeNullable && !_isToTypeNullable)
                                result = new InternalMapper<byte?, double>(delegate(MapperFactory fac, byte? from, ref double to) { to = from.HasValue ? from.Value : default(byte); });
                            else if (!_isFromTypeNullable && _isToTypeNullable)
                                result = new InternalMapper<byte, double?>(delegate(MapperFactory fac, byte from, ref double? to) { to = from; });
                            break;
                        case TypeCode.Int16:
                            if (!_isFromTypeNullable && !_isToTypeNullable)
                                result = new InternalMapper<byte, short>(delegate(MapperFactory fac, byte from, ref short to) { to = from; });
                            else if (_isFromTypeNullable && _isToTypeNullable)
                                result = new InternalMapper<byte?, short?>(delegate(MapperFactory fac, byte? from, ref short? to) { to = from; });
                            else if (_isFromTypeNullable && !_isToTypeNullable)
                                result = new InternalMapper<byte?, short>(delegate(MapperFactory fac, byte? from, ref short to) { to = from.HasValue ? from.Value : default(byte); });
                            else if (!_isFromTypeNullable && _isToTypeNullable)
                                result = new InternalMapper<byte, short?>(delegate(MapperFactory fac, byte from, ref short? to) { to = from; });
                            break;
                        case TypeCode.UInt16:
                            if (!_isFromTypeNullable && !_isToTypeNullable)
                                result = new InternalMapper<byte, ushort>(delegate(MapperFactory fac, byte from, ref ushort to) { to = from; });
                            else if (_isFromTypeNullable && _isToTypeNullable)
                                result = new InternalMapper<byte?, ushort?>(delegate(MapperFactory fac, byte? from, ref ushort? to) { to = from; });
                            else if (_isFromTypeNullable && !_isToTypeNullable)
                                result = new InternalMapper<byte?, ushort>(delegate(MapperFactory fac, byte? from, ref ushort to) { to = from.HasValue ? from.Value : default(byte); });
                            else if (!_isFromTypeNullable && _isToTypeNullable)
                                result = new InternalMapper<byte, ushort?>(delegate(MapperFactory fac, byte from, ref ushort? to) { to = from; });
                            break;
                        case TypeCode.Int32:
                            if (!_isFromTypeNullable && !_isToTypeNullable)
                                result = new InternalMapper<byte, int>(delegate(MapperFactory fac, byte from, ref int to) { to = from; });
                            else if (_isFromTypeNullable && _isToTypeNullable)
                                result = new InternalMapper<byte?, int?>(delegate(MapperFactory fac, byte? from, ref int? to) { to = from; });
                            else if (_isFromTypeNullable && !_isToTypeNullable)
                                result = new InternalMapper<byte?, int>(delegate(MapperFactory fac, byte? from, ref int to) { to = from.HasValue ? from.Value : default(byte); });
                            else if (!_isFromTypeNullable && _isToTypeNullable)
                                result = new InternalMapper<byte, int?>(delegate(MapperFactory fac, byte from, ref int? to) { to = from; });
                            break;
                        case TypeCode.UInt32:
                            if (!_isFromTypeNullable && !_isToTypeNullable)
                                result = new InternalMapper<byte, uint>(delegate(MapperFactory fac, byte from, ref uint to) { to = from; });
                            else if (_isFromTypeNullable && _isToTypeNullable)
                                result = new InternalMapper<byte?, uint?>(delegate(MapperFactory fac, byte? from, ref uint? to) { to = from; });
                            else if (_isFromTypeNullable && !_isToTypeNullable)
                                result = new InternalMapper<byte?, uint>(delegate(MapperFactory fac, byte? from, ref uint to) { to = from.HasValue ? from.Value : default(byte); });
                            else if (!_isFromTypeNullable && _isToTypeNullable)
                                result = new InternalMapper<byte, uint?>(delegate(MapperFactory fac, byte from, ref uint? to) { to = from; });
                            break;
                        case TypeCode.Int64:
                            if (!_isFromTypeNullable && !_isToTypeNullable)
                                result = new InternalMapper<byte, long>(delegate(MapperFactory fac, byte from, ref long to) { to = from; });
                            else if (_isFromTypeNullable && _isToTypeNullable)
                                result = new InternalMapper<byte?, long?>(delegate(MapperFactory fac, byte? from, ref long? to) { to = from; });
                            else if (_isFromTypeNullable && !_isToTypeNullable)
                                result = new InternalMapper<byte?, long>(delegate(MapperFactory fac, byte? from, ref long to) { to = from.HasValue ? from.Value : default(byte); });
                            else if (!_isFromTypeNullable && _isToTypeNullable)
                                result = new InternalMapper<byte, long?>(delegate(MapperFactory fac, byte from, ref long? to) { to = from; });
                            break;
                        case TypeCode.UInt64:
                            if (!_isFromTypeNullable && !_isToTypeNullable)
                                result = new InternalMapper<byte, ulong>(delegate(MapperFactory fac, byte from, ref ulong to) { to = from; });
                            else if (_isFromTypeNullable && _isToTypeNullable)
                                result = new InternalMapper<byte?, ulong?>(delegate(MapperFactory fac, byte? from, ref ulong? to) { to = from; });
                            else if (_isFromTypeNullable && !_isToTypeNullable)
                                result = new InternalMapper<byte?, ulong>(delegate(MapperFactory fac, byte? from, ref ulong to) { to = from.HasValue ? from.Value : default(byte); });
                            else if (!_isFromTypeNullable && _isToTypeNullable)
                                result = new InternalMapper<byte, ulong?>(delegate(MapperFactory fac, byte from, ref ulong? to) { to = from; });
                            break;
                        case TypeCode.Single:
                            if (!_isFromTypeNullable && !_isToTypeNullable)
                                result = new InternalMapper<byte, float>(delegate(MapperFactory fac, byte from, ref float to) { to = from; });
                            else if (_isFromTypeNullable && _isToTypeNullable)
                                result = new InternalMapper<byte?, float?>(delegate(MapperFactory fac, byte? from, ref float? to) { to = from; });
                            else if (_isFromTypeNullable && !_isToTypeNullable)
                                result = new InternalMapper<byte?, float>(delegate(MapperFactory fac, byte? from, ref float to) { to = from.HasValue ? from.Value : default(byte); });
                            else if (!_isFromTypeNullable && _isToTypeNullable)
                                result = new InternalMapper<byte, float?>(delegate(MapperFactory fac, byte from, ref float? to) { to = from; });
                            break;
                    }
                    break;

                #endregion

                #region case Char

                case TypeCode.Char:
                    switch (_toTypeCode)
                    {
                        case TypeCode.Char:
                            if (!_isFromTypeNullable && !_isToTypeNullable)
                                result = new InternalMapper<char, char>(delegate(MapperFactory fac, char from, ref char to) { to = from; });
                            else if (_isFromTypeNullable && _isToTypeNullable)
                                result = new InternalMapper<char?, char?>(delegate(MapperFactory fac, char? from, ref char? to) { to = from; });
                            else if (_isFromTypeNullable && !_isToTypeNullable)
                                result = new InternalMapper<char?, char>(delegate(MapperFactory fac, char? from, ref char to) { to = from.HasValue ? from.Value : default(char); });
                            else if (!_isFromTypeNullable && _isToTypeNullable)
                                result = new InternalMapper<char, char?>(delegate(MapperFactory fac, char from, ref char? to) { to = from; });
                            break;
                    }
                    break;

                #endregion

                #region case DateTime

                case TypeCode.DateTime:
                    switch (_toTypeCode)
                    {
                        case TypeCode.DateTime:
                            if (!_isFromTypeNullable && !_isToTypeNullable)
                                result = new InternalMapper<DateTime, DateTime>(delegate(MapperFactory fac, DateTime from, ref DateTime to) { to = from; });
                            else if (_isFromTypeNullable && _isToTypeNullable)
                                result = new InternalMapper<DateTime?, DateTime?>(delegate(MapperFactory fac, DateTime? from, ref DateTime? to) { to = from; });
                            else if (_isFromTypeNullable && !_isToTypeNullable)
                                result = new InternalMapper<DateTime?, DateTime>(delegate(MapperFactory fac, DateTime? from, ref DateTime to) { to = from.HasValue ? from.Value : default(DateTime); });
                            else if (!_isFromTypeNullable && _isToTypeNullable)
                                result = new InternalMapper<DateTime, DateTime?>(delegate(MapperFactory fac, DateTime from, ref DateTime? to) { to = from; });
                            break;
                    }
                    break;

                #endregion

                #region case Decimal

                case TypeCode.Decimal:
                    switch (_toTypeCode)
                    {
                        case TypeCode.Decimal:
                            if (!_isFromTypeNullable && !_isToTypeNullable)
                                result = new InternalMapper<decimal, decimal>(delegate(MapperFactory fac, decimal from, ref decimal to) { to = from; });
                            else if (_isFromTypeNullable && _isToTypeNullable)
                                result = new InternalMapper<decimal?, decimal?>(delegate(MapperFactory fac, decimal? from, ref decimal? to) { to = from; });
                            else if (_isFromTypeNullable && !_isToTypeNullable)
                                result = new InternalMapper<decimal?, decimal>(delegate(MapperFactory fac, decimal? from, ref decimal to) { to = from.HasValue ? from.Value : default(decimal); });
                            else if (!_isFromTypeNullable && _isToTypeNullable)
                                result = new InternalMapper<decimal, decimal?>(delegate(MapperFactory fac, decimal from, ref decimal? to) { to = from; });
                            break;
                    }
                    break;

                #endregion

                #region case Double

                case TypeCode.Double:
                    switch (_toTypeCode)
                    {
                        case TypeCode.Double:
                            if (!_isFromTypeNullable && !_isToTypeNullable)
                                result = new InternalMapper<double, double>(delegate(MapperFactory fac, double from, ref double to) { to = from; });
                            else if (_isFromTypeNullable && _isToTypeNullable)
                                result = new InternalMapper<double?, double?>(delegate(MapperFactory fac, double? from, ref double? to) { to = from; });
                            else if (_isFromTypeNullable && !_isToTypeNullable)
                                result = new InternalMapper<double?, double>(delegate(MapperFactory fac, double? from, ref double to) { to = from.HasValue ? from.Value : default(double); });
                            else if (!_isFromTypeNullable && _isToTypeNullable)
                                result = new InternalMapper<double, double?>(delegate(MapperFactory fac, double from, ref double? to) { to = from; });
                            break;
                    }
                    break;

                #endregion

                #region case Int16

                case TypeCode.Int16:
                    switch (_toTypeCode)
                    {
                        case TypeCode.Decimal:
                            if (!_isFromTypeNullable && !_isToTypeNullable)
                                result = new InternalMapper<short, decimal>(delegate(MapperFactory fac, short from, ref decimal to) { to = from; });
                            else if (_isFromTypeNullable && _isToTypeNullable)
                                result = new InternalMapper<short?, decimal?>(delegate(MapperFactory fac, short? from, ref decimal? to) { to = from; });
                            else if (_isFromTypeNullable && !_isToTypeNullable)
                                result = new InternalMapper<short?, decimal>(delegate(MapperFactory fac, short? from, ref decimal to) { to = from.HasValue ? from.Value : default(short); });
                            else if (!_isFromTypeNullable && _isToTypeNullable)
                                result = new InternalMapper<short, decimal?>(delegate(MapperFactory fac, short from, ref decimal? to) { to = from; });
                            break;
                        case TypeCode.Double:
                            if (!_isFromTypeNullable && !_isToTypeNullable)
                                result = new InternalMapper<short, double>(delegate(MapperFactory fac, short from, ref double to) { to = from; });
                            else if (_isFromTypeNullable && _isToTypeNullable)
                                result = new InternalMapper<short?, double?>(delegate(MapperFactory fac, short? from, ref double? to) { to = from; });
                            else if (_isFromTypeNullable && !_isToTypeNullable)
                                result = new InternalMapper<short?, double>(delegate(MapperFactory fac, short? from, ref double to) { to = from.HasValue ? from.Value : default(short); });
                            else if (!_isFromTypeNullable && _isToTypeNullable)
                                result = new InternalMapper<short, double?>(delegate(MapperFactory fac, short from, ref double? to) { to = from; });
                            break;
                        case TypeCode.Int16:
                            if (!_isFromTypeNullable && !_isToTypeNullable)
                                result = new InternalMapper<short, short>(delegate(MapperFactory fac, short from, ref short to) { to = from; });
                            else if (_isFromTypeNullable && _isToTypeNullable)
                                result = new InternalMapper<short?, short?>(delegate(MapperFactory fac, short? from, ref short? to) { to = from; });
                            else if (_isFromTypeNullable && !_isToTypeNullable)
                                result = new InternalMapper<short?, short>(delegate(MapperFactory fac, short? from, ref short to) { to = from.HasValue ? from.Value : default(short); });
                            else if (!_isFromTypeNullable && _isToTypeNullable)
                                result = new InternalMapper<short, short?>(delegate(MapperFactory fac, short from, ref short? to) { to = from; });
                            break;
                        case TypeCode.Int32:
                            if (!_isFromTypeNullable && !_isToTypeNullable)
                                result = new InternalMapper<short, int>(delegate(MapperFactory fac, short from, ref int to) { to = from; });
                            else if (_isFromTypeNullable && _isToTypeNullable)
                                result = new InternalMapper<short?, int?>(delegate(MapperFactory fac, short? from, ref int? to) { to = from; });
                            else if (_isFromTypeNullable && !_isToTypeNullable)
                                result = new InternalMapper<short?, int>(delegate(MapperFactory fac, short? from, ref int to) { to = from.HasValue ? from.Value : default(short); });
                            else if (!_isFromTypeNullable && _isToTypeNullable)
                                result = new InternalMapper<short, int?>(delegate(MapperFactory fac, short from, ref int? to) { to = from; });
                            break;
                        case TypeCode.Int64:
                            if (!_isFromTypeNullable && !_isToTypeNullable)
                                result = new InternalMapper<short, long>(delegate(MapperFactory fac, short from, ref long to) { to = from; });
                            else if (_isFromTypeNullable && _isToTypeNullable)
                                result = new InternalMapper<short?, long?>(delegate(MapperFactory fac, short? from, ref long? to) { to = from; });
                            else if (_isFromTypeNullable && !_isToTypeNullable)
                                result = new InternalMapper<short?, long>(delegate(MapperFactory fac, short? from, ref long to) { to = from.HasValue ? from.Value : default(short); });
                            else if (!_isFromTypeNullable && _isToTypeNullable)
                                result = new InternalMapper<short, long?>(delegate(MapperFactory fac, short from, ref long? to) { to = from; });
                            break;
                        case TypeCode.Single:
                            if (!_isFromTypeNullable && !_isToTypeNullable)
                                result = new InternalMapper<short, float>(delegate(MapperFactory fac, short from, ref float to) { to = from; });
                            else if (_isFromTypeNullable && _isToTypeNullable)
                                result = new InternalMapper<short?, float?>(delegate(MapperFactory fac, short? from, ref float? to) { to = from; });
                            else if (_isFromTypeNullable && !_isToTypeNullable)
                                result = new InternalMapper<short?, float>(delegate(MapperFactory fac, short? from, ref float to) { to = from.HasValue ? from.Value : default(short); });
                            else if (!_isFromTypeNullable && _isToTypeNullable)
                                result = new InternalMapper<short, float?>(delegate(MapperFactory fac, short from, ref float? to) { to = from; });
                            break;
                    }
                    break;

                #endregion

                #region case Int32

                case TypeCode.Int32:
                    switch (_toTypeCode)
                    {
                        case TypeCode.Decimal:
                            if (!_isFromTypeNullable && !_isToTypeNullable)
                                result = new InternalMapper<int, decimal>(delegate(MapperFactory fac, int from, ref decimal to) { to = from; });
                            else if (_isFromTypeNullable && _isToTypeNullable)
                                result = new InternalMapper<int?, decimal?>(delegate(MapperFactory fac, int? from, ref decimal? to) { to = from; });
                            else if (_isFromTypeNullable && !_isToTypeNullable)
                                result = new InternalMapper<int?, decimal>(delegate(MapperFactory fac, int? from, ref decimal to) { to = from.HasValue ? from.Value : default(int); });
                            else if (!_isFromTypeNullable && _isToTypeNullable)
                                result = new InternalMapper<int, decimal?>(delegate(MapperFactory fac, int from, ref decimal? to) { to = from; });
                            break;
                        case TypeCode.Double:
                            if (!_isFromTypeNullable && !_isToTypeNullable)
                                result = new InternalMapper<int, double>(delegate(MapperFactory fac, int from, ref double to) { to = from; });
                            else if (_isFromTypeNullable && _isToTypeNullable)
                                result = new InternalMapper<int?, double?>(delegate(MapperFactory fac, int? from, ref double? to) { to = from; });
                            else if (_isFromTypeNullable && !_isToTypeNullable)
                                result = new InternalMapper<int?, double>(delegate(MapperFactory fac, int? from, ref double to) { to = from.HasValue ? from.Value : default(int); });
                            else if (!_isFromTypeNullable && _isToTypeNullable)
                                result = new InternalMapper<int, double?>(delegate(MapperFactory fac, int from, ref double? to) { to = from; });
                            break;
                        case TypeCode.Int32:
                            if (!_isFromTypeNullable && !_isToTypeNullable)
                                result = new InternalMapper<int, int>(delegate(MapperFactory fac, int from, ref int to) { to = from; });
                            else if (_isFromTypeNullable && _isToTypeNullable)
                                result = new InternalMapper<int?, int?>(delegate(MapperFactory fac, int? from, ref int? to) { to = from; });
                            else if (_isFromTypeNullable && !_isToTypeNullable)
                                result = new InternalMapper<int?, int>(delegate(MapperFactory fac, int? from, ref int to) { to = from.HasValue ? from.Value : default(int); });
                            else if (!_isFromTypeNullable && _isToTypeNullable)
                                result = new InternalMapper<int, int?>(delegate(MapperFactory fac, int from, ref int? to) { to = from; });
                            break;
                        case TypeCode.Int64:
                            if (!_isFromTypeNullable && !_isToTypeNullable)
                                result = new InternalMapper<int, long>(delegate(MapperFactory fac, int from, ref long to) { to = from; });
                            else if (_isFromTypeNullable && _isToTypeNullable)
                                result = new InternalMapper<int?, long?>(delegate(MapperFactory fac, int? from, ref long? to) { to = from; });
                            else if (_isFromTypeNullable && !_isToTypeNullable)
                                result = new InternalMapper<int?, long>(delegate(MapperFactory fac, int? from, ref long to) { to = from.HasValue ? from.Value : default(int); });
                            else if (!_isFromTypeNullable && _isToTypeNullable)
                                result = new InternalMapper<int, long?>(delegate(MapperFactory fac, int from, ref long? to) { to = from; });
                            break;
                        case TypeCode.Single:
                            if (!_isFromTypeNullable && !_isToTypeNullable)
                                result = new InternalMapper<int, float>(delegate(MapperFactory fac, int from, ref float to) { to = from; });
                            else if (_isFromTypeNullable && _isToTypeNullable)
                                result = new InternalMapper<int?, float?>(delegate(MapperFactory fac, int? from, ref float? to) { to = from; });
                            else if (_isFromTypeNullable && !_isToTypeNullable)
                                result = new InternalMapper<int?, float>(delegate(MapperFactory fac, int? from, ref float to) { to = from.HasValue ? from.Value : default(int); });
                            else if (!_isFromTypeNullable && _isToTypeNullable)
                                result = new InternalMapper<int, float?>(delegate(MapperFactory fac, int from, ref float? to) { to = from; });
                            break;
                    }
                    break;

                #endregion

                #region case Int64

                case TypeCode.Int64:
                    switch (_toTypeCode)
                    {
                        case TypeCode.Decimal:
                            if (!_isFromTypeNullable && !_isToTypeNullable)
                                result = new InternalMapper<long, decimal>(delegate(MapperFactory fac, long from, ref decimal to) { to = from; });
                            else if (_isFromTypeNullable && _isToTypeNullable)
                                result = new InternalMapper<long?, decimal?>(delegate(MapperFactory fac, long? from, ref decimal? to) { to = from; });
                            else if (_isFromTypeNullable && !_isToTypeNullable)
                                result = new InternalMapper<long?, decimal>(delegate(MapperFactory fac, long? from, ref decimal to) { to = from.HasValue ? from.Value : default(long); });
                            else if (!_isFromTypeNullable && _isToTypeNullable)
                                result = new InternalMapper<long, decimal?>(delegate(MapperFactory fac, long from, ref decimal? to) { to = from; });
                            break;
                        case TypeCode.Double:
                            if (!_isFromTypeNullable && !_isToTypeNullable)
                                result = new InternalMapper<long, double>(delegate(MapperFactory fac, long from, ref double to) { to = from; });
                            else if (_isFromTypeNullable && _isToTypeNullable)
                                result = new InternalMapper<long?, double?>(delegate(MapperFactory fac, long? from, ref double? to) { to = from; });
                            else if (_isFromTypeNullable && !_isToTypeNullable)
                                result = new InternalMapper<long?, double>(delegate(MapperFactory fac, long? from, ref double to) { to = from.HasValue ? from.Value : default(long); });
                            else if (!_isFromTypeNullable && _isToTypeNullable)
                                result = new InternalMapper<long, double?>(delegate(MapperFactory fac, long from, ref double? to) { to = from; });
                            break;
                        case TypeCode.Int64:
                            if (!_isFromTypeNullable && !_isToTypeNullable)
                                result = new InternalMapper<long, long>(delegate(MapperFactory fac, long from, ref long to) { to = from; });
                            else if (_isFromTypeNullable && _isToTypeNullable)
                                result = new InternalMapper<long?, long?>(delegate(MapperFactory fac, long? from, ref long? to) { to = from; });
                            else if (_isFromTypeNullable && !_isToTypeNullable)
                                result = new InternalMapper<long?, long>(delegate(MapperFactory fac, long? from, ref long to) { to = from.HasValue ? from.Value : default(long); });
                            else if (!_isFromTypeNullable && _isToTypeNullable)
                                result = new InternalMapper<long, long?>(delegate(MapperFactory fac, long from, ref long? to) { to = from; });
                            break;
                        case TypeCode.Single:
                            if (!_isFromTypeNullable && !_isToTypeNullable)
                                result = new InternalMapper<long, float>(delegate(MapperFactory fac, long from, ref float to) { to = from; });
                            else if (_isFromTypeNullable && _isToTypeNullable)
                                result = new InternalMapper<long?, float?>(delegate(MapperFactory fac, long? from, ref float? to) { to = from; });
                            else if (_isFromTypeNullable && !_isToTypeNullable)
                                result = new InternalMapper<long?, float>(delegate(MapperFactory fac, long? from, ref float to) { to = from.HasValue ? from.Value : default(long); });
                            else if (!_isFromTypeNullable && _isToTypeNullable)
                                result = new InternalMapper<long, float?>(delegate(MapperFactory fac, long from, ref float? to) { to = from; });
                            break;
                    }
                    break;

                #endregion

                #region case SByte

                case TypeCode.SByte:
                    switch (_toTypeCode)
                    {
                        case TypeCode.SByte:
                            if (!_isFromTypeNullable && !_isToTypeNullable)
                                result = new InternalMapper<sbyte, sbyte>(delegate(MapperFactory fac, sbyte from, ref sbyte to) { to = from; });
                            else if (_isFromTypeNullable && _isToTypeNullable)
                                result = new InternalMapper<sbyte?, sbyte?>(delegate(MapperFactory fac, sbyte? from, ref sbyte? to) { to = from; });
                            else if (_isFromTypeNullable && !_isToTypeNullable)
                                result = new InternalMapper<sbyte?, sbyte>(delegate(MapperFactory fac, sbyte? from, ref sbyte to) { to = from.HasValue ? from.Value : default(sbyte); });
                            else if (!_isFromTypeNullable && _isToTypeNullable)
                                result = new InternalMapper<sbyte, sbyte?>(delegate(MapperFactory fac, sbyte from, ref sbyte? to) { to = from; });
                            break;
                        case TypeCode.Decimal:
                            if (!_isFromTypeNullable && !_isToTypeNullable)
                                result = new InternalMapper<sbyte, decimal>(delegate(MapperFactory fac, sbyte from, ref decimal to) { to = from; });
                            else if (_isFromTypeNullable && _isToTypeNullable)
                                result = new InternalMapper<sbyte?, decimal?>(delegate(MapperFactory fac, sbyte? from, ref decimal? to) { to = from; });
                            else if (_isFromTypeNullable && !_isToTypeNullable)
                                result = new InternalMapper<sbyte?, decimal>(delegate(MapperFactory fac, sbyte? from, ref decimal to) { to = from.HasValue ? from.Value : default(sbyte); });
                            else if (!_isFromTypeNullable && _isToTypeNullable)
                                result = new InternalMapper<sbyte, decimal?>(delegate(MapperFactory fac, sbyte from, ref decimal? to) { to = from; });
                            break;
                        case TypeCode.Double:
                            if (!_isFromTypeNullable && !_isToTypeNullable)
                                result = new InternalMapper<sbyte, double>(delegate(MapperFactory fac, sbyte from, ref double to) { to = from; });
                            else if (_isFromTypeNullable && _isToTypeNullable)
                                result = new InternalMapper<sbyte?, double?>(delegate(MapperFactory fac, sbyte? from, ref double? to) { to = from; });
                            else if (_isFromTypeNullable && !_isToTypeNullable)
                                result = new InternalMapper<sbyte?, double>(delegate(MapperFactory fac, sbyte? from, ref double to) { to = from.HasValue ? from.Value : default(sbyte); });
                            else if (!_isFromTypeNullable && _isToTypeNullable)
                                result = new InternalMapper<sbyte, double?>(delegate(MapperFactory fac, sbyte from, ref double? to) { to = from; });
                            break;
                        case TypeCode.Int16:
                            if (!_isFromTypeNullable && !_isToTypeNullable)
                                result = new InternalMapper<sbyte, short>(delegate(MapperFactory fac, sbyte from, ref short to) { to = from; });
                            else if (_isFromTypeNullable && _isToTypeNullable)
                                result = new InternalMapper<sbyte?, short?>(delegate(MapperFactory fac, sbyte? from, ref short? to) { to = from; });
                            else if (_isFromTypeNullable && !_isToTypeNullable)
                                result = new InternalMapper<sbyte?, short>(delegate(MapperFactory fac, sbyte? from, ref short to) { to = from.HasValue ? from.Value : default(sbyte); });
                            else if (!_isFromTypeNullable && _isToTypeNullable)
                                result = new InternalMapper<sbyte, short?>(delegate(MapperFactory fac, sbyte from, ref short? to) { to = from; });
                            break;
                        case TypeCode.Int32:
                            if (!_isFromTypeNullable && !_isToTypeNullable)
                                result = new InternalMapper<sbyte, int>(delegate(MapperFactory fac, sbyte from, ref int to) { to = from; });
                            else if (_isFromTypeNullable && _isToTypeNullable)
                                result = new InternalMapper<sbyte?, int?>(delegate(MapperFactory fac, sbyte? from, ref int? to) { to = from; });
                            else if (_isFromTypeNullable && !_isToTypeNullable)
                                result = new InternalMapper<sbyte?, int>(delegate(MapperFactory fac, sbyte? from, ref int to) { to = from.HasValue ? from.Value : default(sbyte); });
                            else if (!_isFromTypeNullable && _isToTypeNullable)
                                result = new InternalMapper<sbyte, int?>(delegate(MapperFactory fac, sbyte from, ref int? to) { to = from; });
                            break;
                        case TypeCode.Int64:
                            if (!_isFromTypeNullable && !_isToTypeNullable)
                                result = new InternalMapper<sbyte, long>(delegate(MapperFactory fac, sbyte from, ref long to) { to = from; });
                            else if (_isFromTypeNullable && _isToTypeNullable)
                                result = new InternalMapper<sbyte?, long?>(delegate(MapperFactory fac, sbyte? from, ref long? to) { to = from; });
                            else if (_isFromTypeNullable && !_isToTypeNullable)
                                result = new InternalMapper<sbyte?, long>(delegate(MapperFactory fac, sbyte? from, ref long to) { to = from.HasValue ? from.Value : default(sbyte); });
                            else if (!_isFromTypeNullable && _isToTypeNullable)
                                result = new InternalMapper<sbyte, long?>(delegate(MapperFactory fac, sbyte from, ref long? to) { to = from; });
                            break;
                        case TypeCode.Single:
                            if (!_isFromTypeNullable && !_isToTypeNullable)
                                result = new InternalMapper<sbyte, float>(delegate(MapperFactory fac, sbyte from, ref float to) { to = from; });
                            else if (_isFromTypeNullable && _isToTypeNullable)
                                result = new InternalMapper<sbyte?, float?>(delegate(MapperFactory fac, sbyte? from, ref float? to) { to = from; });
                            else if (_isFromTypeNullable && !_isToTypeNullable)
                                result = new InternalMapper<sbyte?, float>(delegate(MapperFactory fac, sbyte? from, ref float to) { to = from.HasValue ? from.Value : default(sbyte); });
                            else if (!_isFromTypeNullable && _isToTypeNullable)
                                result = new InternalMapper<sbyte, float?>(delegate(MapperFactory fac, sbyte from, ref float? to) { to = from; });
                            break;
                    }
                    break;

                #endregion

                #region case Single

                case TypeCode.Single:
                    switch (_toTypeCode)
                    {
                        case TypeCode.Double:
                            if (!_isFromTypeNullable && !_isToTypeNullable)
                                result = new InternalMapper<float, double>(delegate(MapperFactory fac, float from, ref double to) { to = from; });
                            else if (_isFromTypeNullable && _isToTypeNullable)
                                result = new InternalMapper<float?, double?>(delegate(MapperFactory fac, float? from, ref double? to) { to = from; });
                            else if (_isFromTypeNullable && !_isToTypeNullable)
                                result = new InternalMapper<float?, double>(delegate(MapperFactory fac, float? from, ref double to) { to = from.HasValue ? from.Value : default(float); });
                            else if (!_isFromTypeNullable && _isToTypeNullable)
                                result = new InternalMapper<float, double?>(delegate(MapperFactory fac, float from, ref double? to) { to = from; });
                            break;
                        case TypeCode.Single:
                            if (!_isFromTypeNullable && !_isToTypeNullable)
                                result = new InternalMapper<float, float>(delegate(MapperFactory fac, float from, ref float to) { to = from; });
                            else if (_isFromTypeNullable && _isToTypeNullable)
                                result = new InternalMapper<float?, float?>(delegate(MapperFactory fac, float? from, ref float? to) { to = from; });
                            else if (_isFromTypeNullable && !_isToTypeNullable)
                                result = new InternalMapper<float?, float>(delegate(MapperFactory fac, float? from, ref float to) { to = from.HasValue ? from.Value : default(float); });
                            else if (!_isFromTypeNullable && _isToTypeNullable)
                                result = new InternalMapper<float, float?>(delegate(MapperFactory fac, float from, ref float? to) { to = from; });
                            break;
                    }
                    break;

                #endregion

                #region case String

                case TypeCode.String:
                    switch (_toTypeCode)
                    {
                        case TypeCode.String:
                            result = new InternalMapper<string, string>(delegate(MapperFactory fac, string from, ref string to) { to = from; });
                            break;
                    }
                    break;

                #endregion

                #region case UInt16

                case TypeCode.UInt16:
                    switch (_toTypeCode)
                    {
                        case TypeCode.UInt16:
                            if (!_isFromTypeNullable && !_isToTypeNullable)
                                result = new InternalMapper<ushort, ushort>(delegate(MapperFactory fac, ushort from, ref ushort to) { to = from; });
                            else if (_isFromTypeNullable && _isToTypeNullable)
                                result = new InternalMapper<ushort?, ushort?>(delegate(MapperFactory fac, ushort? from, ref ushort? to) { to = from; });
                            else if (_isFromTypeNullable && !_isToTypeNullable)
                                result = new InternalMapper<ushort?, ushort>(delegate(MapperFactory fac, ushort? from, ref ushort to) { to = from.HasValue ? from.Value : default(ushort); });
                            else if (!_isFromTypeNullable && _isToTypeNullable)
                                result = new InternalMapper<ushort, ushort?>(delegate(MapperFactory fac, ushort from, ref ushort? to) { to = from; });
                            break;
                        case TypeCode.Decimal:
                            if (!_isFromTypeNullable && !_isToTypeNullable)
                                result = new InternalMapper<ushort, decimal>(delegate(MapperFactory fac, ushort from, ref decimal to) { to = from; });
                            else if (_isFromTypeNullable && _isToTypeNullable)
                                result = new InternalMapper<ushort?, decimal?>(delegate(MapperFactory fac, ushort? from, ref decimal? to) { to = from; });
                            else if (_isFromTypeNullable && !_isToTypeNullable)
                                result = new InternalMapper<ushort?, decimal>(delegate(MapperFactory fac, ushort? from, ref decimal to) { to = from.HasValue ? from.Value : default(ushort); });
                            else if (!_isFromTypeNullable && _isToTypeNullable)
                                result = new InternalMapper<ushort, decimal?>(delegate(MapperFactory fac, ushort from, ref decimal? to) { to = from; });
                            break;
                        case TypeCode.Double:
                            if (!_isFromTypeNullable && !_isToTypeNullable)
                                result = new InternalMapper<ushort, double>(delegate(MapperFactory fac, ushort from, ref double to) { to = from; });
                            else if (_isFromTypeNullable && _isToTypeNullable)
                                result = new InternalMapper<ushort?, double?>(delegate(MapperFactory fac, ushort? from, ref double? to) { to = from; });
                            else if (_isFromTypeNullable && !_isToTypeNullable)
                                result = new InternalMapper<ushort?, double>(delegate(MapperFactory fac, ushort? from, ref double to) { to = from.HasValue ? from.Value : default(ushort); });
                            else if (!_isFromTypeNullable && _isToTypeNullable)
                                result = new InternalMapper<ushort, double?>(delegate(MapperFactory fac, ushort from, ref double? to) { to = from; });
                            break;
                        case TypeCode.Int32:
                            if (!_isFromTypeNullable && !_isToTypeNullable)
                                result = new InternalMapper<ushort, int>(delegate(MapperFactory fac, ushort from, ref int to) { to = from; });
                            else if (_isFromTypeNullable && _isToTypeNullable)
                                result = new InternalMapper<ushort?, int?>(delegate(MapperFactory fac, ushort? from, ref int? to) { to = from; });
                            else if (_isFromTypeNullable && !_isToTypeNullable)
                                result = new InternalMapper<ushort?, int>(delegate(MapperFactory fac, ushort? from, ref int to) { to = from.HasValue ? from.Value : default(ushort); });
                            else if (!_isFromTypeNullable && _isToTypeNullable)
                                result = new InternalMapper<ushort, int?>(delegate(MapperFactory fac, ushort from, ref int? to) { to = from; });
                            break;
                        case TypeCode.UInt32:
                            if (!_isFromTypeNullable && !_isToTypeNullable)
                                result = new InternalMapper<ushort, uint>(delegate(MapperFactory fac, ushort from, ref uint to) { to = from; });
                            else if (_isFromTypeNullable && _isToTypeNullable)
                                result = new InternalMapper<ushort?, uint?>(delegate(MapperFactory fac, ushort? from, ref uint? to) { to = from; });
                            else if (_isFromTypeNullable && !_isToTypeNullable)
                                result = new InternalMapper<ushort?, uint>(delegate(MapperFactory fac, ushort? from, ref uint to) { to = from.HasValue ? from.Value : default(ushort); });
                            else if (!_isFromTypeNullable && _isToTypeNullable)
                                result = new InternalMapper<ushort, uint?>(delegate(MapperFactory fac, ushort from, ref uint? to) { to = from; });
                            break;
                        case TypeCode.Int64:
                            if (!_isFromTypeNullable && !_isToTypeNullable)
                                result = new InternalMapper<ushort, long>(delegate(MapperFactory fac, ushort from, ref long to) { to = from; });
                            else if (_isFromTypeNullable && _isToTypeNullable)
                                result = new InternalMapper<ushort?, long?>(delegate(MapperFactory fac, ushort? from, ref long? to) { to = from; });
                            else if (_isFromTypeNullable && !_isToTypeNullable)
                                result = new InternalMapper<ushort?, long>(delegate(MapperFactory fac, ushort? from, ref long to) { to = from.HasValue ? from.Value : default(ushort); });
                            else if (!_isFromTypeNullable && _isToTypeNullable)
                                result = new InternalMapper<ushort, long?>(delegate(MapperFactory fac, ushort from, ref long? to) { to = from; });
                            break;
                        case TypeCode.UInt64:
                            if (!_isFromTypeNullable && !_isToTypeNullable)
                                result = new InternalMapper<ushort, ulong>(delegate(MapperFactory fac, ushort from, ref ulong to) { to = from; });
                            else if (_isFromTypeNullable && _isToTypeNullable)
                                result = new InternalMapper<ushort?, ulong?>(delegate(MapperFactory fac, ushort? from, ref ulong? to) { to = from; });
                            else if (_isFromTypeNullable && !_isToTypeNullable)
                                result = new InternalMapper<ushort?, ulong>(delegate(MapperFactory fac, ushort? from, ref ulong to) { to = from.HasValue ? from.Value : default(ushort); });
                            else if (!_isFromTypeNullable && _isToTypeNullable)
                                result = new InternalMapper<ushort, ulong?>(delegate(MapperFactory fac, ushort from, ref ulong? to) { to = from; });
                            break;
                        case TypeCode.Single:
                            if (!_isFromTypeNullable && !_isToTypeNullable)
                                result = new InternalMapper<ushort, float>(delegate(MapperFactory fac, ushort from, ref float to) { to = from; });
                            else if (_isFromTypeNullable && _isToTypeNullable)
                                result = new InternalMapper<ushort?, float?>(delegate(MapperFactory fac, ushort? from, ref float? to) { to = from; });
                            else if (_isFromTypeNullable && !_isToTypeNullable)
                                result = new InternalMapper<ushort?, float>(delegate(MapperFactory fac, ushort? from, ref float to) { to = from.HasValue ? from.Value : default(ushort); });
                            else if (!_isFromTypeNullable && _isToTypeNullable)
                                result = new InternalMapper<ushort, float?>(delegate(MapperFactory fac, ushort from, ref float? to) { to = from; });
                            break;
                    }
                    break;

                #endregion

                #region case UInt32

                case TypeCode.UInt32:
                    switch (_toTypeCode)
                    {
                        case TypeCode.UInt32:
                            if (!_isFromTypeNullable && !_isToTypeNullable)
                                result = new InternalMapper<uint, uint>(delegate(MapperFactory fac, uint from, ref uint to) { to = from; });
                            else if (_isFromTypeNullable && _isToTypeNullable)
                                result = new InternalMapper<uint?, uint?>(delegate(MapperFactory fac, uint? from, ref uint? to) { to = from; });
                            else if (_isFromTypeNullable && !_isToTypeNullable)
                                result = new InternalMapper<uint?, uint>(delegate(MapperFactory fac, uint? from, ref uint to) { to = from.HasValue ? from.Value : default(uint); });
                            else if (!_isFromTypeNullable && _isToTypeNullable)
                                result = new InternalMapper<uint, uint?>(delegate(MapperFactory fac, uint from, ref uint? to) { to = from; });
                            break;
                        case TypeCode.Decimal:
                            if (!_isFromTypeNullable && !_isToTypeNullable)
                                result = new InternalMapper<uint, decimal>(delegate(MapperFactory fac, uint from, ref decimal to) { to = from; });
                            else if (_isFromTypeNullable && _isToTypeNullable)
                                result = new InternalMapper<uint?, decimal?>(delegate(MapperFactory fac, uint? from, ref decimal? to) { to = from; });
                            else if (_isFromTypeNullable && !_isToTypeNullable)
                                result = new InternalMapper<uint?, decimal>(delegate(MapperFactory fac, uint? from, ref decimal to) { to = from.HasValue ? from.Value : default(uint); });
                            else if (!_isFromTypeNullable && _isToTypeNullable)
                                result = new InternalMapper<uint, decimal?>(delegate(MapperFactory fac, uint from, ref decimal? to) { to = from; });
                            break;
                        case TypeCode.Double:
                            if (!_isFromTypeNullable && !_isToTypeNullable)
                                result = new InternalMapper<uint, double>(delegate(MapperFactory fac, uint from, ref double to) { to = from; });
                            else if (_isFromTypeNullable && _isToTypeNullable)
                                result = new InternalMapper<uint?, double?>(delegate(MapperFactory fac, uint? from, ref double? to) { to = from; });
                            else if (_isFromTypeNullable && !_isToTypeNullable)
                                result = new InternalMapper<uint?, double>(delegate(MapperFactory fac, uint? from, ref double to) { to = from.HasValue ? from.Value : default(uint); });
                            else if (!_isFromTypeNullable && _isToTypeNullable)
                                result = new InternalMapper<uint, double?>(delegate(MapperFactory fac, uint from, ref double? to) { to = from; });
                            break;
                        case TypeCode.Int64:
                            if (!_isFromTypeNullable && !_isToTypeNullable)
                                result = new InternalMapper<uint, long>(delegate(MapperFactory fac, uint from, ref long to) { to = from; });
                            else if (_isFromTypeNullable && _isToTypeNullable)
                                result = new InternalMapper<uint?, long?>(delegate(MapperFactory fac, uint? from, ref long? to) { to = from; });
                            else if (_isFromTypeNullable && !_isToTypeNullable)
                                result = new InternalMapper<uint?, long>(delegate(MapperFactory fac, uint? from, ref long to) { to = from.HasValue ? from.Value : default(uint); });
                            else if (!_isFromTypeNullable && _isToTypeNullable)
                                result = new InternalMapper<uint, long?>(delegate(MapperFactory fac, uint from, ref long? to) { to = from; });
                            break;
                        case TypeCode.UInt64:
                            if (!_isFromTypeNullable && !_isToTypeNullable)
                                result = new InternalMapper<uint, ulong>(delegate(MapperFactory fac, uint from, ref ulong to) { to = from; });
                            else if (_isFromTypeNullable && _isToTypeNullable)
                                result = new InternalMapper<uint?, ulong?>(delegate(MapperFactory fac, uint? from, ref ulong? to) { to = from; });
                            else if (_isFromTypeNullable && !_isToTypeNullable)
                                result = new InternalMapper<uint?, ulong>(delegate(MapperFactory fac, uint? from, ref ulong to) { to = from.HasValue ? from.Value : default(uint); });
                            else if (!_isFromTypeNullable && _isToTypeNullable)
                                result = new InternalMapper<uint, ulong?>(delegate(MapperFactory fac, uint from, ref ulong? to) { to = from; });
                            break;
                        case TypeCode.Single:
                            if (!_isFromTypeNullable && !_isToTypeNullable)
                                result = new InternalMapper<uint, float>(delegate(MapperFactory fac, uint from, ref float to) { to = from; });
                            else if (_isFromTypeNullable && _isToTypeNullable)
                                result = new InternalMapper<uint?, float?>(delegate(MapperFactory fac, uint? from, ref float? to) { to = from; });
                            else if (_isFromTypeNullable && !_isToTypeNullable)
                                result = new InternalMapper<uint?, float>(delegate(MapperFactory fac, uint? from, ref float to) { to = from.HasValue ? from.Value : default(uint); });
                            else if (!_isFromTypeNullable && _isToTypeNullable)
                                result = new InternalMapper<uint, float?>(delegate(MapperFactory fac, uint from, ref float? to) { to = from; });
                            break;
                    }
                    break;

                #endregion

                #region case UInt64

                case TypeCode.UInt64:
                    switch (_toTypeCode)
                    {
                        case TypeCode.UInt64:
                            if (!_isFromTypeNullable && !_isToTypeNullable)
                                result = new InternalMapper<ulong, ulong>(delegate(MapperFactory fac, ulong from, ref ulong to) { to = from; });
                            else if (_isFromTypeNullable && _isToTypeNullable)
                                result = new InternalMapper<ulong?, ulong?>(delegate(MapperFactory fac, ulong? from, ref ulong? to) { to = from; });
                            else if (_isFromTypeNullable && !_isToTypeNullable)
                                result = new InternalMapper<ulong?, ulong>(delegate(MapperFactory fac, ulong? from, ref ulong to) { to = from.HasValue ? from.Value : default(ulong); });
                            else if (!_isFromTypeNullable && _isToTypeNullable)
                                result = new InternalMapper<ulong, ulong?>(delegate(MapperFactory fac, ulong from, ref ulong? to) { to = from; });
                            break;
                        case TypeCode.Decimal:
                            if (!_isFromTypeNullable && !_isToTypeNullable)
                                result = new InternalMapper<ulong, decimal>(delegate(MapperFactory fac, ulong from, ref decimal to) { to = from; });
                            else if (_isFromTypeNullable && _isToTypeNullable)
                                result = new InternalMapper<ulong?, decimal?>(delegate(MapperFactory fac, ulong? from, ref decimal? to) { to = from; });
                            else if (_isFromTypeNullable && !_isToTypeNullable)
                                result = new InternalMapper<ulong?, decimal>(delegate(MapperFactory fac, ulong? from, ref decimal to) { to = from.HasValue ? from.Value : default(ulong); });
                            else if (!_isFromTypeNullable && _isToTypeNullable)
                                result = new InternalMapper<ulong, decimal?>(delegate(MapperFactory fac, ulong from, ref decimal? to) { to = from; });
                            break;
                        case TypeCode.Double:
                            if (!_isFromTypeNullable && !_isToTypeNullable)
                                result = new InternalMapper<ulong, double>(delegate(MapperFactory fac, ulong from, ref double to) { to = from; });
                            else if (_isFromTypeNullable && _isToTypeNullable)
                                result = new InternalMapper<ulong?, double?>(delegate(MapperFactory fac, ulong? from, ref double? to) { to = from; });
                            else if (_isFromTypeNullable && !_isToTypeNullable)
                                result = new InternalMapper<ulong?, double>(delegate(MapperFactory fac, ulong? from, ref double to) { to = from.HasValue ? from.Value : default(ulong); });
                            else if (!_isFromTypeNullable && _isToTypeNullable)
                                result = new InternalMapper<ulong, double?>(delegate(MapperFactory fac, ulong from, ref double? to) { to = from; });
                            break;
                        case TypeCode.Single:
                            if (!_isFromTypeNullable && !_isToTypeNullable)
                                result = new InternalMapper<ulong, float>(delegate(MapperFactory fac, ulong from, ref float to) { to = from; });
                            else if (_isFromTypeNullable && _isToTypeNullable)
                                result = new InternalMapper<ulong?, float?>(delegate(MapperFactory fac, ulong? from, ref float? to) { to = from; });
                            else if (_isFromTypeNullable && !_isToTypeNullable)
                                result = new InternalMapper<ulong?, float>(delegate(MapperFactory fac, ulong? from, ref float to) { to = from.HasValue ? from.Value : default(ulong); });
                            else if (!_isFromTypeNullable && _isToTypeNullable)
                                result = new InternalMapper<ulong, float?>(delegate(MapperFactory fac, ulong from, ref float? to) { to = from; });
                            break;
                    }
                    break;

                #endregion
            }

            return result;
        }

        #endregion
    }
}
