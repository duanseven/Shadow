using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace NSun.Data.Mapper
{
    internal static class MappingHelper
    {
        public static bool IsNullableType(Type type)
        {
            return type.IsGenericType
                && type.GetGenericTypeDefinition().Equals(typeof(Nullable<>));
        }

        public static PropertyInfo GetPropertyInfo(Type type, string propertyName, bool ignoreCase, bool ignoreUnderscore)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            var baseType = type;
            while (baseType != typeof(object) && baseType != typeof(ValueType))
            {
                foreach (var property in baseType.GetProperties())
                {
                    //if (ignoreCase && ignoreUnderscore)
                    if (ignoreUnderscore)
                    {
                        if (string.Compare(
                            propertyName.Replace("_", string.Empty),
                            property.Name.Replace("_", string.Empty),
                            ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal) == 0)
                        {
                            return property;
                        }
                    }
                    else
                    {
                        if (string.Compare(
                            propertyName,
                            property.Name,
                            ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal) == 0)
                        {
                            return property;
                        }
                    }
                }

                baseType = baseType.BaseType;
            }

            return null;
        }

        public static FieldInfo GetFieldInfo(Type type, string fieldName, bool ignoreCase, bool ignoreUnderscore)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            var baseType = type;
            while (baseType != typeof(object) && baseType != typeof(ValueType))
            {
                foreach (var field in baseType.GetFields())
                {
                    if (ignoreCase && ignoreUnderscore)
                    {
                        if (string.Compare(
                            fieldName.Replace("_", string.Empty),
                            field.Name.Replace("_", string.Empty),
                            ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal) == 0)
                        {
                            return field;
                        }
                    }
                    else
                    {
                        if (string.Compare(
                            fieldName,
                            field.Name,
                            ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal) == 0)
                        {
                            return field;
                        }
                    }
                }

                baseType = baseType.BaseType;
            }

            return null;
        }

        public static Type GetElementType(Type type)
        {
            if (type.IsArray)
                return type.GetElementType();

            if (typeof(IDataReader).IsAssignableFrom(type))
                return typeof(IDataReader);

            if (type.IsGenericType)
            {
                var firstGenericArgument = type.GetGenericArguments()[0];
                if (typeof(IEnumerable<>).MakeGenericType(firstGenericArgument)
                    .IsAssignableFrom(type))
                {
                    return firstGenericArgument;
                }
            }

            return type.IsValueType ? typeof(ValueType) : typeof(object);
        }

        public static Type GetUnderlyingType(Type type)
        {
            if (type.IsEnum)
                return Enum.GetUnderlyingType(type);
            if (MappingHelper.IsNullableType(type))
            {
                Type firstArgType = type.GetGenericArguments()[0];
                if (firstArgType.IsEnum)
                    return typeof(Nullable<>).MakeGenericType(Enum.GetUnderlyingType(firstArgType));
            }

            return type.UnderlyingSystemType;
        }

        public static bool IsGuidType(Type type)
        {
            if (type == null || !type.IsValueType)
                return false;

            if (MappingHelper.IsNullableType(type)
                && type.GetGenericArguments()[0] == typeof(Guid))
            {
                return true;
            }
            else if (type == typeof(Guid))
            {
                return true;
            }

            return false;
        }

        public static bool IsEnumType(Type type)
        {
            if (type == null || !type.IsValueType)
                return false;

            if (MappingHelper.IsNullableType(type)
                && type.GetGenericArguments()[0].IsEnum)
            {
                return true;
            }
            else if (type.IsEnum)
            {
                return true;
            }

            return false;
        }
    }
}
