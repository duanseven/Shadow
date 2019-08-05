using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Reflection.Emit;

namespace NSun.Data
{ 
    internal class DynamicBuilder
    {
        private int _fieldCount = 0;

          /*
           * Dictionary<KeyValuePair<string, string>, string> k = 
           * new Dictionary<KeyValuePair<string, string>, string>();
           * k.Add(new KeyValuePair<string, string>("1", "2"), "3");
           * Console.WriteLine(k.ContainsKey(new KeyValuePair<string, string>("1", "2")));
           */
        private static Dictionary<KeyValuePair<Type, string>, Load> _cacheColumn = new Dictionary<KeyValuePair<Type, string>, Load>();

        private object _lockcache = new object();

        //GetOrdinal
        private readonly MethodInfo getValueMethod = typeof(IDataRecord).GetMethod("get_Item", new Type[] { typeof(int) });

        private readonly MethodInfo isDBNullMethod = typeof(IDataRecord).GetMethod("IsDBNull", new Type[] { typeof(int) });

        public delegate object Load(IDataRecord dataRecord);

        private Load handler;

        public object Build(Type type, IDataRecord dataRecord, string columnstr)
        {
            var key= new KeyValuePair<Type, string>(type, columnstr);
            if(_cacheColumn.ContainsKey(key))
            {
                return _cacheColumn[key](dataRecord);
            }
            lock (_lockcache)
            {
                if (_cacheColumn.ContainsKey(key))
                    return _cacheColumn[key](dataRecord);
                var hand = CreateBuilder(type, dataRecord);
                _cacheColumn.Add(key, hand);
                return hand(dataRecord);
            }
        }

        public Load CreateBuilder(Type type, IDataRecord dataRecord)
        {
            DynamicMethod method = new DynamicMethod("DynamicCreate", type, new Type[] { typeof(IDataRecord) }, type, true);
            ILGenerator generator = method.GetILGenerator();

            LocalBuilder result = generator.DeclareLocal(type);

            generator.Emit(OpCodes.Newobj, type.GetConstructor(Type.EmptyTypes));
            generator.Emit(OpCodes.Stloc, result);
            _fieldCount = dataRecord.FieldCount;
            for (int i = 0; i < _fieldCount; i++)
            {
                PropertyInfo propertyInfo = null;
                Label endIfLabel = generator.DefineLabel();
                string proname = dataRecord.GetName(i);

                propertyInfo = type.GetProperty(proname, BindingFlags.Public | BindingFlags.Instance | BindingFlags.CreateInstance | BindingFlags.IgnoreCase);

                if (propertyInfo != null && propertyInfo.GetSetMethod() != null)
                {
                    generator.Emit(OpCodes.Ldarg_0);
                    generator.Emit(OpCodes.Ldc_I4, i);
                    generator.Emit(OpCodes.Callvirt, isDBNullMethod);

                    generator.Emit(OpCodes.Brtrue, endIfLabel);

                    generator.Emit(OpCodes.Ldloc, result);

                    generator.Emit(OpCodes.Ldarg_0);
                    generator.Emit(OpCodes.Ldc_I4, i);
                    generator.Emit(OpCodes.Callvirt, getValueMethod);

                    generator.Emit(OpCodes.Unbox_Any, propertyInfo.PropertyType);

                    generator.Emit(OpCodes.Callvirt, propertyInfo.GetSetMethod());

                    generator.MarkLabel(endIfLabel);
                }
            }

            generator.Emit(OpCodes.Ldloc, result);
            generator.Emit(OpCodes.Ret);

            return (Load)method.CreateDelegate(typeof(Load));
        }
    }

    internal static class DynamicBuilder<T>
    {
        private static int _fieldCount = 0;

        private static Dictionary<string, Load> _cacheColumn = new Dictionary<string, Load>();

        private static object _lockcache = new object();

        //GetOrdinal
        private static readonly MethodInfo getValueMethod = typeof(IDataRecord).GetMethod("get_Item", new Type[] { typeof(int) });

        private static readonly MethodInfo isDBNullMethod = typeof(IDataRecord).GetMethod("IsDBNull", new Type[] { typeof(int) });

        public delegate T Load(IDataRecord dataRecord);

        private static Load handler;

        //public static T Build(IDataRecord dataRecord)
        //{
        //    if (handler == null)
        //    {
        //        handler = CreateBuilder(dataRecord);
        //    }
        //    return handler(dataRecord);
        //}

        public static T Build(IDataRecord dataRecord, string columnstr)
        {
            if (handler == null && string.IsNullOrEmpty(columnstr))
            {
                handler = CreateBuilder(dataRecord);
                return handler(dataRecord);
            }
            if (handler != null && string.IsNullOrEmpty(columnstr))
            {
                return handler(dataRecord);
            }
            if (_cacheColumn.ContainsKey(columnstr))
                return _cacheColumn[columnstr](dataRecord);
            lock (_lockcache)
            {
                if (_cacheColumn.ContainsKey(columnstr))
                    return _cacheColumn[columnstr](dataRecord);
                var hand = CreateBuilder(dataRecord);
                _cacheColumn.Add(columnstr, hand);
                return hand(dataRecord);
            }
        }

        public static Load CreateBuilder(IDataRecord dataRecord)
        {
            DynamicMethod method = new DynamicMethod("DynamicCreate", typeof(T), new Type[] { typeof(IDataRecord) }, typeof(T), true);
            ILGenerator generator = method.GetILGenerator();

            LocalBuilder result = generator.DeclareLocal(typeof(T));

            generator.Emit(OpCodes.Newobj, typeof(T).GetConstructor(Type.EmptyTypes));
            generator.Emit(OpCodes.Stloc, result);
            _fieldCount = dataRecord.FieldCount;
            for (int i = 0; i < _fieldCount; i++)
            {
                PropertyInfo propertyInfo = null;
                Label endIfLabel = generator.DefineLabel();
                string proname = dataRecord.GetName(i);

                propertyInfo = typeof(T).GetProperty(proname, BindingFlags.Public | BindingFlags.Instance | BindingFlags.CreateInstance | BindingFlags.IgnoreCase);

                if (propertyInfo != null && propertyInfo.GetSetMethod() != null)
                {
                    generator.Emit(OpCodes.Ldarg_0);
                    generator.Emit(OpCodes.Ldc_I4, i);
                    generator.Emit(OpCodes.Callvirt, isDBNullMethod);

                    generator.Emit(OpCodes.Brtrue, endIfLabel);

                    generator.Emit(OpCodes.Ldloc, result);

                    generator.Emit(OpCodes.Ldarg_0);
                    generator.Emit(OpCodes.Ldc_I4, i);
                    generator.Emit(OpCodes.Callvirt, getValueMethod);

                    generator.Emit(OpCodes.Unbox_Any, propertyInfo.PropertyType);

                    generator.Emit(OpCodes.Callvirt, propertyInfo.GetSetMethod());

                    generator.MarkLabel(endIfLabel);
                }
            }

            generator.Emit(OpCodes.Ldloc, result);
            generator.Emit(OpCodes.Ret);

            return (Load)method.CreateDelegate(typeof(Load));
        }
    }
}
