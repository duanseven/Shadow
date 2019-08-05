using System;
using System.Collections.Specialized;
using System.Data;

namespace NSun.Data.Helper
{
	internal static class DataTypeParser
	{
		private static readonly HybridDictionary Types;

		static DataTypeParser()
		{
			Types = new HybridDictionary();

            Types[typeof(string)] = DbType.String;
            Types[typeof(DateTime)] = DbType.DateTime;
            Types[typeof(Date)] = DbType.Date;
            Types[typeof(Time)] = DbType.Time;
            Types[typeof(bool)] = DbType.Boolean;

            Types[typeof(byte)] = DbType.Byte;
            Types[typeof(sbyte)] = DbType.SByte;
            Types[typeof(decimal)] = DbType.Decimal;
            Types[typeof(double)] = DbType.Double;
            Types[typeof(float)] = DbType.Single;

            Types[typeof(int)] = DbType.Int32;
            Types[typeof(uint)] = DbType.UInt32;
            Types[typeof(long)] = DbType.Int64;
            Types[typeof(ulong)] = DbType.UInt64;
            Types[typeof(short)] = DbType.Int16;
            Types[typeof(ushort)] = DbType.UInt16;

            Types[typeof(Guid)] = DbType.Guid;
            Types[typeof(byte[])] = DbType.Binary;
            Types[typeof(Enum)] = DbType.Int32;
		    Types[typeof (TimeSpan)] = DbType.Double;

		}

		public static DbType Parse(object o)
		{
			return Parse(o.GetType());
		}

        public static DbType ParseType(DbType o)
        {
            return o;
        }

		public static DbType Parse(Type t)
		{
			if ( t.IsEnum )
			{
				t = typeof(Enum);
			}
			if ( Types.Contains(t) )
			{
				return (DbType)Types[t];
			}
            if ( Helper.NullableHelper.IsNullableType(t) )
            {
                return Helper.NullableHelper.GetDataType(t);
            }
			throw new ArgumentOutOfRangeException(t.ToString());
		}         
	}
}
