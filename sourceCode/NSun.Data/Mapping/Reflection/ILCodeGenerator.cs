using System;
using System.Globalization;
using System.Reflection;
using System.Reflection.Emit;
using System.Collections.Generic;

namespace NSun.Data.Reflection
{
    /// <summary>
    /// Helper class to simplify method body emitting
    /// </summary>
    public sealed class ILCodeGenerator
    {
        private readonly ILGenerator _gen;
        private readonly Stack<IfState> _ifStateStack = new Stack<IfState>();

        #region MethodInfos

        private static readonly MethodInfo _objectReferenceEqualsMethod
            = typeof(object).GetMethod("ReferenceEquals", BindingFlags.Public | BindingFlags.Static);

        private static readonly MethodInfo _objectEqualsMethod
            = typeof(object).GetMethod("Equals", BindingFlags.Public | BindingFlags.Static);

        private static readonly MethodInfo _objectToStringMethod
            = typeof(object).GetMethod("ToString", Type.EmptyTypes);

        private static readonly MethodInfo _stringConcat2Method
            = typeof(string).GetMethod("Concat", new[] { typeof(string), typeof(string) });

        private static readonly MethodInfo _stringConcat3Method
            = typeof(string).GetMethod("Concat", new[] { typeof(string), typeof(string), typeof(string) });

        private static readonly MethodInfo _stringFormatMethod
            = typeof(string).GetMethod("Format", new[] { typeof(string), typeof(object[]) });

        private static readonly MethodInfo _typeGetTypeFromHandleMethod
            = typeof(Type).GetMethod("GetTypeFromHandle");

        #endregion

        #region Constructors

        private ILCodeGenerator(ILGenerator gen)
        {
            _gen = gen;
        }

        #endregion

        #region Factory Methods

        /// <summary>
        /// Creates the dynamic method code generator.
        /// </summary>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="delegateType">Type of the delegate.</param>
        /// <param name="method">The method.</param>
        /// <returns></returns>
        public static ILCodeGenerator CreateDynamicMethodCodeGenerator(
            string methodName, Type delegateType, out DynamicMethod method)
        {
            if (string.IsNullOrEmpty(methodName))
                throw new ArgumentNullException("methodName");
            if (delegateType == null)
                throw new ArgumentNullException("delegateType");
            if (!typeof(Delegate).IsAssignableFrom(delegateType))
                throw new ArgumentException(
                    delegateType.FullName + " is not a delegate type");

            var invokeMethod = delegateType.GetMethod("Invoke");
            method = new DynamicMethod(methodName, invokeMethod.ReturnType, 
                GetParameterTypes(invokeMethod.GetParameters()), true);
            return new ILCodeGenerator(method.GetILGenerator());
        }

        /// <summary>
        /// Creates the default constructor code generator.
        /// </summary>
        /// <param name="defaultConstructor">The default constructor.</param>
        /// <param name="typeBuilder">The type builder.</param>
        /// <param name="constructorBuilder">The constructor builder.</param>
        /// <returns></returns>
        public static ILCodeGenerator CreateDefaultConstructorCodeGenerator(
            ConstructorInfo defaultConstructor, 
            TypeBuilder typeBuilder, 
            out ConstructorBuilder constructorBuilder)
        {
            if (defaultConstructor == null)
                throw new ArgumentNullException("defaultConstructor");
            var parameters = defaultConstructor.GetParameters();
            if (parameters.Length == 0)
                throw new ArgumentException("Specified defaultConstructor is not a default contractor");
            if (typeBuilder == null)
                throw new ArgumentNullException("typeBuilder");

            constructorBuilder = typeBuilder.DefineDefaultConstructor(defaultConstructor.Attributes);
            return new ILCodeGenerator(constructorBuilder.GetILGenerator());
        }

        /// <summary>
        /// Creates the constructor code generator.
        /// </summary>
        /// <param name="constructor">The constructor.</param>
        /// <param name="typeBuilder">The type builder.</param>
        /// <param name="constructorBuilder">The constructor builder.</param>
        /// <returns></returns>
        public static ILCodeGenerator CreateConstructorCodeGenerator(
            ConstructorInfo constructor, 
            TypeBuilder typeBuilder, 
            out ConstructorBuilder constructorBuilder)
        {
            if (constructor == null)
                throw new ArgumentNullException("constructor");
            if (typeBuilder == null)
                throw new ArgumentNullException("typeBuilder");

            var parameters = constructor.GetParameters();
            constructorBuilder = typeBuilder.DefineConstructor(
                constructor.Attributes, constructor.CallingConvention,
                GetParameterTypes(parameters));
            return new ILCodeGenerator(constructorBuilder.GetILGenerator());
        }

        /// <summary>
        /// Creates the method code generator.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <param name="typeBuilder">The type builder.</param>
        /// <param name="methodBuilder">The method builder.</param>
        /// <returns></returns>
        public static ILCodeGenerator CreateMethodCodeGenerator(
            MethodInfo method, TypeBuilder typeBuilder, out MethodBuilder methodBuilder)
        {
            if (method == null)
                throw new ArgumentNullException("method");
            if (typeBuilder == null)
                throw new ArgumentNullException("typeBuilder");

            var parameters = method.GetParameters();
            methodBuilder = typeBuilder.DefineMethod(
                method.Name,
                method.Attributes,
                method.CallingConvention,
                method.ReturnType,
                GetParameterTypes(parameters));
            return new ILCodeGenerator(methodBuilder.GetILGenerator());
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the internal IL generator.
        /// </summary>
        /// <value>The internal IL generator.</value>
        public ILGenerator InternalILGenerator
        {
            get { return _gen; }
        }

        #endregion

        #region Public Methods

        #region Load Value Methods

        public ILCodeGenerator Load(ILExpression val)
        {
            if (val == null)
                return LoadNull();

            val(this);
            return this;
        }

        public ILCodeGenerator LoadNull()
        {
            _gen.Emit(OpCodes.Ldnull);

            return this;
        }

        public ILCodeGenerator Load(bool boolVar)
        {
            if (boolVar)
                _gen.Emit(OpCodes.Ldc_I4_1);
            else
                _gen.Emit(OpCodes.Ldc_I4_0);

            return this;
        }

        public ILCodeGenerator Load(double doubleVal)
        {
            _gen.Emit(OpCodes.Ldc_R8, doubleVal);

            return this;
        }

        public ILCodeGenerator Load(int intVar)
        {
            switch (intVar)
            {
                case -1:
                    _gen.Emit(OpCodes.Ldc_I4_M1);
                    break;
                case 0:
                    _gen.Emit(OpCodes.Ldc_I4_0);
                    break;
                case 1:
                    _gen.Emit(OpCodes.Ldc_I4_1);
                    break;
                case 2:
                    _gen.Emit(OpCodes.Ldc_I4_2);
                    break;
                case 3:
                    _gen.Emit(OpCodes.Ldc_I4_3);
                    break;
                case 4:
                    _gen.Emit(OpCodes.Ldc_I4_4);
                    break;
                case 5:
                    _gen.Emit(OpCodes.Ldc_I4_5);
                    break;
                case 6:
                    _gen.Emit(OpCodes.Ldc_I4_6);
                    break;
                case 7:
                    _gen.Emit(OpCodes.Ldc_I4_7);
                    break;
                case 8:
                    _gen.Emit(OpCodes.Ldc_I4_8);
                    break;
                default:
                    _gen.Emit(OpCodes.Ldc_I4, intVar);
                    break;
            }

            return this;
        }

        public ILCodeGenerator Load(long longVal)
        {
            _gen.Emit(OpCodes.Ldc_I8, longVal);

            return this;
        }

        public ILCodeGenerator Load(float floatVal)
        {
            _gen.Emit(OpCodes.Ldc_R4, floatVal);

            return this;
        }

        public ILCodeGenerator Load(string strVar)
        {
            if (strVar == null)
                return LoadNull();

            _gen.Emit(OpCodes.Ldstr, strVar);

            return this;
        }

        public ILCodeGenerator LoadTypeOf(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            CallStaticMethod(_typeGetTypeFromHandleMethod, 
                gen => gen.InternalILGenerator.Emit(OpCodes.Ldtoken, type));

            return this;
        }

        public ILCodeGenerator LoadDefaultValue(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            if (!type.IsValueType)
                return LoadNull();

            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Boolean:
                    return Load(false);
                case TypeCode.Char:
                case TypeCode.SByte:
                case TypeCode.Byte:
                case TypeCode.Int16:
                case TypeCode.UInt16:
                case TypeCode.Int32:
                case TypeCode.UInt32:
                    return Load(0);
                case TypeCode.Int64:
                case TypeCode.UInt64:
                    return Load((long)0);
                case TypeCode.Single:
                    return Load((float)0);
                case TypeCode.Double:
                    return Load((double)0);
                default: //initialize other value type
                    var builder = DeclareLocalVariable(type);
                    InitializeValueTypeObject(type, 
                        gen => gen.LoadLocalVariableAddress(builder.LocalType, builder.LocalIndex));
                    return LoadLocalVariable(builder.LocalIndex);
            }
        }

        #endregion

        #region Local Variable Methods

        public LocalBuilder DeclareLocalVariable(Type type)
        {
            return DeclareLocalVariable(type, false);
        }

        public LocalBuilder DeclareLocalVariable(Type type, bool isPinned)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            var builder = _gen.DeclareLocal(type, isPinned);
            return builder;
        }

        public ILCodeGenerator LoadLocalVariable(int slot)
        {
            switch (slot)
            {
                case 0:
                    _gen.Emit(OpCodes.Ldloc_0);
                    break;
                case 1:
                    _gen.Emit(OpCodes.Ldloc_1);
                    break;
                case 2:
                    _gen.Emit(OpCodes.Ldloc_2);
                    break;
                case 3:
                    _gen.Emit(OpCodes.Ldloc_3);
                    break;
                default:
                    if (slot <= 0xff)
                        _gen.Emit(OpCodes.Ldloc_S, slot);
                    else
                        _gen.Emit(OpCodes.Ldloc, slot);
                    break;
            }

            return this;
        }

        public ILCodeGenerator LoadLocalVariable(LocalBuilder local)
        {
            if (local == null)
                throw new ArgumentNullException("local");

            return LoadLocalVariable(local.LocalIndex);
        }

        public ILCodeGenerator LoadLocalVariableAddress(Type localType, int slot)
        {
            if (localType == null)
                throw new ArgumentNullException("localType");

            if (localType.IsValueType)
            {
                InternalLoadLocalVariableAddress(slot);
                return this;
            }

            return LoadLocalVariable(slot);
        }

        public ILCodeGenerator LoadLocalVariableAddress(LocalBuilder local)
        {
            if (local == null)
                throw new ArgumentNullException("local");

            return LoadLocalVariableAddress(local.LocalType, local.LocalIndex);
        }

        public ILCodeGenerator StoreLocalVariable(int slot, ILExpression val)
        {
            Load(val);

            switch (slot)
            {
                case 0:
                    _gen.Emit(OpCodes.Stloc_0);
                    break;
                case 1:
                    _gen.Emit(OpCodes.Stloc_1);
                    break;
                case 2:
                    _gen.Emit(OpCodes.Stloc_2);
                    break;
                case 3:
                    _gen.Emit(OpCodes.Stloc_3);
                    break;
                default:
                    if (slot <= 0xff)
                        _gen.Emit(OpCodes.Stloc_S, slot);
                    else
                        _gen.Emit(OpCodes.Stloc, slot);
                    break;
            }

            return this;
        }

        public ILCodeGenerator StoreLocalVariable(LocalBuilder local, ILExpression val)
        {
            if (local == null)
                throw new ArgumentNullException("local");

            return StoreLocalVariable(local.LocalIndex, val);
        }


        #endregion

        #region Argument Methods

        public ILCodeGenerator LoadArgument(int slot)
        {
            switch (slot)
            {
                case 0:
                    _gen.Emit(OpCodes.Ldarg_0);
                    break;
                case 1:
                    _gen.Emit(OpCodes.Ldarg_1);
                    break;
                case 2:
                    _gen.Emit(OpCodes.Ldarg_2);
                    break;
                case 3:
                    _gen.Emit(OpCodes.Ldarg_3);
                    break;
                default:
                    if (slot <= 0xff)
                        _gen.Emit(OpCodes.Ldarg_S, slot);
                    else
                        _gen.Emit(OpCodes.Ldarg, slot);
                    break;
            }

            return this;
        }

        public ILCodeGenerator LoadArgumentAddress(int slot)
        {
            if (slot <= 0xff)
                _gen.Emit(OpCodes.Ldarga_S, slot);
            else
                _gen.Emit(OpCodes.Ldarga, slot);

            return this;
        }

        public ILCodeGenerator StoreArgument(int slot, ILExpression val)
        {
            Load(val);

            if (slot <= 0xff)
                _gen.Emit(OpCodes.Starg_S, slot);
            else
                _gen.Emit(OpCodes.Starg, slot);

            return this;
        }

        public ILCodeGenerator LoadArgumentIndirectly(int slot, Type type)
        {
            LoadArgument(slot);

            if (type == typeof(sbyte))
                _gen.Emit(OpCodes.Ldind_I1);
            else if (type == typeof(short))
                _gen.Emit(OpCodes.Ldind_I2);
            else if (type == typeof(int))
                _gen.Emit(OpCodes.Ldind_I4);
            else if (type == typeof(byte))
                _gen.Emit(OpCodes.Ldind_U1);
            else if (type == typeof(ushort))
                _gen.Emit(OpCodes.Ldind_U2);
            else if (type == typeof(uint))
                _gen.Emit(OpCodes.Ldind_U4);
            else if (type == typeof(long) || type == typeof(ulong))
                _gen.Emit(OpCodes.Ldind_I8);
            else if (type == typeof(float))
                _gen.Emit(OpCodes.Ldind_R4);
            else if (type == typeof(double))
                _gen.Emit(OpCodes.Ldind_R8);
            else if (type.IsValueType)
                LoadValueTypeObjectFromStack(type);
            else
                _gen.Emit(OpCodes.Ldind_Ref);

            return this;
        }

        public ILCodeGenerator StoreArgumentIndirectly(int slot, Type type, ILExpression val)
        {
            LoadArgument(slot);
            Load(val);

            if (type == typeof(sbyte) || type == typeof(byte))
                _gen.Emit(OpCodes.Stind_I1);
            else if (type == typeof(short) || type == typeof(ushort))
                _gen.Emit(OpCodes.Stind_I2);
            else if (type == typeof(int) || type == typeof(uint))
                _gen.Emit(OpCodes.Stind_I4);
            else if (type == typeof(long) || type == typeof(ulong))
                _gen.Emit(OpCodes.Stind_I8);
            else if (type == typeof(float))
                _gen.Emit(OpCodes.Stind_R4);
            else if (type == typeof(double))
                _gen.Emit(OpCodes.Stind_R8);
            else if (type.IsValueType)
                StoreValueTypeObjectToStack(type, nop => nop.Nop());
            else
                _gen.Emit(OpCodes.Stind_Ref);

            return this;
        }

        #endregion

        #region Field Methods

        public ILCodeGenerator LoadStaticField(FieldInfo field)
        {
            if (field == null)
                throw new ArgumentNullException("field");
            if (!field.IsStatic)
                throw new ArgumentException(field.Name + "is not a static field");

            _gen.Emit(OpCodes.Ldsfld, field);

            return this;
        }

        public ILCodeGenerator LoadField(ILExpression thisObj, FieldInfo field)
        {
            if (thisObj == null)
                throw new ArgumentNullException("thisObj");
            if (field == null)
                throw new ArgumentNullException("field");
            if (field.IsStatic)
                throw new ArgumentException(field.Name + "is a static field");

            Load(thisObj);
            _gen.Emit(OpCodes.Ldfld, field);

            return this;
        }

        public ILCodeGenerator StoreStaticField(FieldInfo field, ILExpression val)
        {
            if (field == null)
                throw new ArgumentNullException("field");
            if (!field.IsStatic)
                throw new ArgumentException(field.Name + "is not a static field");

            Load(val);
            _gen.Emit(OpCodes.Stsfld, field);

            return this;
        }

        public ILCodeGenerator StoreField(ILExpression thisObj, FieldInfo field, ILExpression val)
        {
            if (thisObj == null)
                throw new ArgumentNullException("thisObj");
            if (field == null)
                throw new ArgumentNullException("field");
            if (field.IsStatic)
                throw new ArgumentException(field.Name + " is a static field");

            Load(thisObj);
            Load(val);
            _gen.Emit(OpCodes.Stfld, field);

            return this;
        }

        #endregion

        #region Property Methods

        public ILCodeGenerator LoadStaticProperty(PropertyInfo property)
        {
            if (property == null)
                throw new ArgumentNullException("property");
            if (!property.CanRead)
                throw new ArgumentException("property is write only");
            var getMethod = property.GetGetMethod() ?? property.GetGetMethod(true);
            if (!getMethod.IsStatic)
                throw new ArgumentException(property.Name + " is not a static property");

            CallStaticMethod(getMethod);

            return this;
        }

        public ILCodeGenerator LoadProperty(ILExpression thisObj, PropertyInfo property)
        {
            if (thisObj == null)
                throw new ArgumentNullException("thisObj");
            if (property == null)
                throw new ArgumentNullException("property");
            if (!property.CanRead)
                throw new ArgumentException("property is write only");
            var getMethod = property.GetGetMethod() ?? property.GetGetMethod(true);
            if (getMethod.IsStatic)
                throw new ArgumentException(property.Name + " is a static property");

            CallMethod(thisObj, getMethod);

            return this;
        }

        public ILCodeGenerator StoreStaticProperty(PropertyInfo property, ILExpression val)
        {
            if (property == null)
                throw new ArgumentNullException("property");
            if (!property.CanWrite)
                throw new ArgumentException("property is read only");
            var setMethod = property.GetSetMethod() ?? property.GetSetMethod(true);
            if (!setMethod.IsStatic)
                throw new ArgumentException(property.Name + " is not a static property");

            Load(val);
            CallStaticMethod(setMethod, val);

            return this;
        }

        public ILCodeGenerator StoreProperty(ILExpression thisObj, PropertyInfo property, ILExpression val)
        {
            if (thisObj == null)
                throw new ArgumentNullException("thisObj");
            if (property == null)
                throw new ArgumentNullException("property");
            if (!property.CanWrite)
                throw new ArgumentException("property is read only");
            var setMethod = property.GetSetMethod() ?? property.GetSetMethod(true);
            if (setMethod.IsStatic)
                throw new ArgumentException(property.Name + " is a static property");

            CallMethod(thisObj, setMethod, val);

            return this;
        }

        #endregion

        #region Value Type Methods

        public ILCodeGenerator Box(Type valueType, ILExpression val)
        {
            if (valueType == null)
                throw new ArgumentNullException("valueType");
            if (!valueType.IsValueType)
                throw new ArgumentException(valueType.FullName + " is not value type");
            if (val == null)
                throw new ArgumentNullException("val");

            Load(val);
            _gen.Emit(OpCodes.Box, valueType);

            return this;
        }

        public ILCodeGenerator Unbox(Type valueType, ILExpression val)
        {
            if (valueType == null)
                throw new ArgumentNullException("valueType");
            if (!valueType.IsValueType)
                throw new ArgumentException(valueType.FullName + " is not value type");
            if (val == null)
                throw new ArgumentNullException("val");

            Load(val);
            _gen.Emit(OpCodes.Unbox, valueType);

            return this;
        }

        /// <summary>
        /// equals Unbox + LoadValueTypeObjectFromStack
        /// </summary>
        public ILCodeGenerator UnboxAny(Type valueType, ILExpression val)
        {
            if (valueType == null)
                throw new ArgumentNullException("valueType");
            if (!valueType.IsValueType)
                throw new ArgumentException(valueType.FullName + " is not value type");
            if (val == null)
                throw new ArgumentNullException("val");

            Load(val);
            _gen.Emit(OpCodes.Unbox_Any, valueType);

            return this;

        }

        public ILCodeGenerator InitializeValueTypeObject(Type valueType, ILExpression valAddress)
        {
            if (valueType == null)
                throw new ArgumentNullException("valueType");
            if (valAddress == null)
                throw new ArgumentNullException("valAddress");

            Load(valAddress);
            _gen.Emit(OpCodes.Initobj, valueType);

            return this;
        }

        public ILCodeGenerator LoadValueTypeObjectFromStack(Type valueType)
        {
            if (valueType == null)
                throw new ArgumentNullException("valueType");
            if (!valueType.IsValueType)
                throw new ArgumentException(valueType.FullName + " is not a ValueType");

            var opcode = GetLdindOpCode(Type.GetTypeCode(valueType));
            if (!opcode.Equals(OpCodes.Nop))
                _gen.Emit(opcode);
            else
                _gen.Emit(OpCodes.Ldobj, valueType);

            return this;
        }

        public ILCodeGenerator StoreValueTypeObjectToStack(Type valueType, ILExpression val)
        {
            if (valueType == null)
                throw new ArgumentNullException("valueType");
            if (!valueType.IsValueType)
                throw new ArgumentException(valueType.FullName + " is not a ValueType");
            if (val == null)
                throw new ArgumentNullException("val");

            Load(val);

            _gen.Emit(OpCodes.Stobj, valueType);

            return this;
        }

        #endregion

        #region Array Methods

        public ILCodeGenerator NewArray(Type arrayElementType, ILExpression len)
        {
            if (arrayElementType == null)
                throw new ArgumentNullException("arrayElementType");

            Load(len);
            _gen.Emit(OpCodes.Newarr, arrayElementType);

            return this;
        }

        public ILCodeGenerator StoreArrayElement(ILExpression array, Type arrayElementType, 
            int arrayIndex, ILExpression val, Type valType)
        {
            if (array == null)
                throw new ArgumentNullException("array");
            if (arrayElementType == null)
                throw new ArgumentNullException("arrayElementType");

            if (arrayElementType.IsEnum)
                return StoreArrayElement(
                    array,
                    Enum.GetUnderlyingType(arrayElementType), arrayIndex, val, valType);

            if (IsStruct(arrayElementType))
            {
                StoreValueTypeObjectToStack(
                    arrayElementType,
                    gen =>
                    {
                        gen.Load(array);
                        gen.Load(arrayIndex);
                        gen.InternalILGenerator.Emit(OpCodes.Ldelema, arrayElementType);
                        gen.ConvertValue(gen2 => gen2.Load(val), valType, arrayElementType);
                    });
            }
            else
            {
                Load(array);
                Load(arrayIndex);
                ConvertValue(gen => gen.Load(val), valType, arrayElementType);

                var opcode = GetStelemOpCode(Type.GetTypeCode(arrayElementType));
                _gen.Emit(opcode);
            }

            return this;
        }

        public ILCodeGenerator LoadArrayElement(ILExpression array, Type arrayElementType, 
            int arrayIndex)
        {
            if (array == null)
                throw new ArgumentNullException("array");
            if (arrayElementType == null)
                throw new ArgumentNullException("arrayElementType");

            if (arrayElementType.IsEnum)
                return LoadArrayElement(array, Enum.GetUnderlyingType(arrayElementType), arrayIndex);

            Load(array);
            Load(arrayIndex);
            if (IsStruct(arrayElementType))
            {
                _gen.Emit(OpCodes.Ldelema, arrayElementType);
                return LoadValueTypeObjectFromStack(arrayElementType);
            }

            var opcode = GetLdelemOpCode(Type.GetTypeCode(arrayElementType));
            _gen.Emit(opcode);

            return this;
        }

        public ILCodeGenerator LoadArrayLength(ILExpression array)
        {
            if (array == null)
                throw new ArgumentNullException("array");

            Load(array);
            _gen.Emit(OpCodes.Ldlen);
            _gen.Emit(OpCodes.Conv_I4);

            return this;
        }

        #endregion

        #region Call Methods

        public ILCodeGenerator CallStaticMethod(MethodInfo method, params ILExpression[] vals)
        {
            if (method == null)
                throw new ArgumentNullException("method");
            if (!method.IsStatic)
                throw new ArgumentException(method.Name + " is not a static method");

            LoadExpressions(vals);
            _gen.Emit(OpCodes.Call, method);

            return this;
        }

        public ILCodeGenerator CallMethod(ILExpression thisObj, MethodInfo method, params ILExpression[] vals)
        {
            if (thisObj == null)
                throw new ArgumentNullException("thisObj");
            if (method == null)
                throw new ArgumentNullException("method");

            Load(thisObj);
            LoadExpressions(vals);
            if (method.IsVirtual)
                _gen.Emit(OpCodes.Callvirt, method);
            else
                _gen.Emit(OpCodes.Call, method);

            return this;
        }

        public ILCodeGenerator CallReferenceEquals(ILExpression left, ILExpression right)
        {
            return CallStaticMethod(_objectReferenceEqualsMethod, left, right);
        }

        public ILCodeGenerator CallEquals(ILExpression left, ILExpression right)
        {
            return CallStaticMethod(_objectEqualsMethod, left, right);
        }

        public ILCodeGenerator CallStringConcat(ILExpression left, ILExpression right)
        {
            return CallStaticMethod(_stringConcat2Method, left, right);
        }

        public ILCodeGenerator CallStringConcat(ILExpression str1, ILExpression str2, ILExpression str3)
        {
            return CallStaticMethod(_stringConcat3Method, str1, str2, str3);
        }

        public ILCodeGenerator CallStringFormat(ILExpression msg, params ILExpression[] vals)
        {
            if (msg == null)
                throw new ArgumentNullException("msg");

            var stringFormatArray = DeclareLocalVariable(typeof(object[]));
            StoreLocalVariable(stringFormatArray.LocalIndex,
                gen => 
                    gen.NewArray(typeof(object), 
                    gen2 => gen2.Load(vals.Length)));
            for (var i = 0; i < vals.Length; ++i)
            {
                var val = vals[i];
                StoreArrayElement(gen => gen.LoadLocalVariable(stringFormatArray.LocalIndex), 
                    typeof(object), i, gen => gen.Load(val), typeof(object));
            }
            return CallStaticMethod(_stringFormatMethod, 
                gen => gen.Load(msg),
                gen => gen.LoadLocalVariable(stringFormatArray.LocalIndex));
        }

        public ILCodeGenerator CallToString(Type type, ILExpression val)
        {
            if (type == null)
                throw new ArgumentNullException("type");
            if (val == null)
                throw new ArgumentNullException("val");

            if (type.IsValueType)
                return CallMethod(
                    gen => gen.Box(type, gen2 => gen2.Load(val)), 
                    _objectToStringMethod);

            return CallMethod(gen => gen.Load(val), _objectToStringMethod);
        }

        #endregion

        #region Arithmetic Methods

        public ILCodeGenerator Add(ILExpression left, ILExpression right)
        {
            if (left == null)
                throw new ArgumentNullException("left");
            if (right == null)
                throw new ArgumentNullException("right");

            LoadExpressions(left, right);
            _gen.Emit(OpCodes.Add);

            return this;
        }

        public ILCodeGenerator Subtract(ILExpression left, ILExpression right)
        {
            if (left == null)
                throw new ArgumentNullException("left");
            if (right == null)
                throw new ArgumentNullException("right");

            LoadExpressions(left, right);
            _gen.Emit(OpCodes.Sub);

            return this;
        }

        public ILCodeGenerator Negate(ILExpression val)
        {
            if (val == null)
                throw new ArgumentNullException("val");

            Load(val);
            _gen.Emit(OpCodes.Neg);

            return this;
        }

        public ILCodeGenerator Not(ILExpression val)
        {
            if (val == null)
                throw new ArgumentNullException("val");

            Load(val);
            _gen.Emit(OpCodes.Not);

            return this;
        }

        public ILCodeGenerator And(ILExpression left, ILExpression right)
        {
            if (left == null)
                throw new ArgumentNullException("left");
            if (right == null)
                throw new ArgumentNullException("right");

            LoadExpressions(left, right);
            _gen.Emit(OpCodes.And);

            return this;
        }

        public ILCodeGenerator Or(ILExpression left, ILExpression right)
        {
            if (left == null)
                throw new ArgumentNullException("left");
            if (right == null)
                throw new ArgumentNullException("right");

            LoadExpressions(left, right);
            _gen.Emit(OpCodes.Or);

            return this;
        }

        public ILCodeGenerator Xor(ILExpression left, ILExpression right)
        {
            if (left == null)
                throw new ArgumentNullException("left");
            if (right == null)
                throw new ArgumentNullException("right");

            LoadExpressions(left, right);
            _gen.Emit(OpCodes.Xor);

            return this;
        }

        public ILCodeGenerator Equals(ILExpression left, ILExpression right)
        {
            if (left == null)
                throw new ArgumentNullException("left");
            if (right == null)
                throw new ArgumentNullException("right");

            LoadExpressions(left, right);
            _gen.Emit(OpCodes.Ceq);

            return this;
        }

        public ILCodeGenerator GreaterThan(ILExpression left, ILExpression right)
        {
            if (left == null)
                throw new ArgumentNullException("left");
            if (right == null)
                throw new ArgumentNullException("right");

            LoadExpressions(left, right);
            _gen.Emit(OpCodes.Cgt);

            return this;
        }

        public ILCodeGenerator GreaterThanOrEquals(ILExpression left, ILExpression right)
        {
            if (left == null)
                throw new ArgumentNullException("left");
            if (right == null)
                throw new ArgumentNullException("right");

            LoadExpressions(left, right);
            _gen.Emit(OpCodes.Clt_Un);

            return this;
        }

        public ILCodeGenerator LessThan(ILExpression left, ILExpression right)
        {
            if (left == null)
                throw new ArgumentNullException("left");
            if (right == null)
                throw new ArgumentNullException("right");

            LoadExpressions(left, right);
            _gen.Emit(OpCodes.Clt);

            return this;
        }

        public ILCodeGenerator LessThanOrEquals(ILExpression left, ILExpression right)
        {
            if (left == null)
                throw new ArgumentNullException("left");
            if (right == null)
                throw new ArgumentNullException("right");

            LoadExpressions(left, right);
            _gen.Emit(OpCodes.Cgt_Un);

            return this;
        }

        public ILCodeGenerator Multiple(ILExpression left, ILExpression right)
        {
            if (left == null)
                throw new ArgumentNullException("left");
            if (right == null)
                throw new ArgumentNullException("right");

            LoadExpressions(left, right);
            _gen.Emit(OpCodes.Mul);

            return this;
        }

        public ILCodeGenerator Divide(ILExpression left, ILExpression right)
        {
            if (left == null)
                throw new ArgumentNullException("left");
            if (right == null)
                throw new ArgumentNullException("right");

            LoadExpressions(left, right);
            _gen.Emit(OpCodes.Div);

            return this;
        }

        public ILCodeGenerator Modulo(ILExpression left, ILExpression right)
        {
            if (left == null)
                throw new ArgumentNullException("left");
            if (right == null)
                throw new ArgumentNullException("right");

            LoadExpressions(left, right);
            _gen.Emit(OpCodes.Rem);

            return this;
        }

        #endregion

        #region Condition Methods

        public Label DefineLabel()
        {
            return _gen.DefineLabel();
        }

        public ILCodeGenerator MarkLabel(Label label)
        {
            _gen.MarkLabel(label);

            return this;
        }

        public ILCodeGenerator If(ILExpression boolVal)
        {
            Load(boolVal);
            InternalIf(Cmp.True);

            return this;
        }

        public ILCodeGenerator IfNot(ILExpression boolVal)
        {
            Load(boolVal);
            InternalIf(Cmp.False);

            return this;
        }

        public ILCodeGenerator IfLessThan(ILExpression left, ILExpression right)
        {
            LoadExpressions(left, right);
            InternalIf(Cmp.LessThan);

            return this;
        }

        public ILCodeGenerator IfGreaterThan(ILExpression left, ILExpression right)
        {
            LoadExpressions(left, right);
            InternalIf(Cmp.GreaterThan);

            return this;
        }

        public ILCodeGenerator IfLessThanOrEquals(ILExpression left, ILExpression right)
        {
            LoadExpressions(left, right);
            InternalIf(Cmp.LessThanOrEquals);

            return this;
        }

        public ILCodeGenerator IfGreaterThanOrEquals(ILExpression left, ILExpression right)
        {
            LoadExpressions(left, right);
            InternalIf(Cmp.GreaterThanOrEquals);

            return this;
        }

        public ILCodeGenerator IfEquals(ILExpression left, ILExpression right)
        {
            LoadExpressions(left, right);
            InternalIf(Cmp.Equals);

            return this;
        }

        public ILCodeGenerator IfNotEquals(ILExpression left, ILExpression right)
        {
            LoadExpressions(left, right);
            InternalIf(Cmp.NotEquals);

            return this;
        }

        public ILCodeGenerator Else()
        {
            var state = _ifStateStack.Pop();
            GoTo(state.EndIfLabel);
            MarkLabel(state.ElseBeginLabel);
            state.ElseBeginLabel = state.EndIfLabel;
            _ifStateStack.Push(state);

            return this;
        }

        public ILCodeGenerator ElseIf(ILExpression boolVal)
        {
            var state = _ifStateStack.Pop();
            GoTo(state.EndIfLabel);
            MarkLabel(state.ElseBeginLabel);
            Load(boolVal);
            state.ElseBeginLabel = DefineLabel();
            _gen.Emit(GetBranchCode(Cmp.True), state.ElseBeginLabel);
            _ifStateStack.Push(state);

            return this;
        }

        public ILCodeGenerator EndIf()
        {
            var state = _ifStateStack.Pop();
            if (!state.ElseBeginLabel.Equals(state.EndIfLabel))
            {
                MarkLabel(state.ElseBeginLabel);
            }
            return MarkLabel(state.EndIfLabel);
        }

        public ILCodeGenerator GoTo(Label label)
        {
            if (label == null)
                throw new ArgumentNullException("label");

            _gen.Emit(OpCodes.Br, label);

            return this;
        }

        #endregion

        #region  Try/Catch/Finally/Throw

        public ILCodeGenerator Try()
        {
            _gen.BeginExceptionBlock();

            return this;
        }

        public ILCodeGenerator Catch(Type exceptionType)
        {
            _gen.BeginCatchBlock(exceptionType);

            return this;
        }

        public ILCodeGenerator Finally()
        {
            _gen.BeginFinallyBlock();

            return this;
        }

        public ILCodeGenerator EndTry()
        {
            _gen.EndExceptionBlock();

            return this;
        }

        public ILCodeGenerator Throw(Type exType, ILExpression ex)
        {
            if (exType == null)
                throw new ArgumentNullException("exType");
            if (ex == null)
                throw new ArgumentNullException("ex");

            _gen.ThrowException(exType);

            return this;
        }

        public ILCodeGenerator Rethrow()
        {
            _gen.Emit(OpCodes.Rethrow);

            return this;
        }

        #endregion

        #region Misc Methods

        public ILCodeGenerator New(ConstructorInfo constructorInfo, params ILExpression[] vals)
        {
            if (constructorInfo == null)
                throw new ArgumentNullException("constructorInfo");

            if (vals != null)
            {
                foreach (var val in vals)
                {
                    Load(val);
                }
            }

            _gen.Emit(OpCodes.Newobj, constructorInfo);

            return this;
        }

        public ILCodeGenerator ConvertValue(ILExpression sourceVal, 
            Type sourceType, Type targetType)
        {
            if (sourceType == null)
                throw new ArgumentNullException("sourceType");
            if (targetType == null)
                throw new ArgumentNullException("targetType");

            Load(sourceVal);
            InternalConvert(sourceType, targetType, false);

            return this;
        }

        public ILCodeGenerator ConvertAddress(ILExpression sourceValAddress,
            Type sourceType, Type targetType)
        {
            if (sourceValAddress == null)
                throw new ArgumentNullException("sourceValAddress");
            if (sourceType == null)
                throw new ArgumentNullException("sourceType");
            if (targetType == null)
                throw new ArgumentNullException("targetType");

            Load(sourceValAddress);
            InternalConvert(sourceType, targetType, true);

            return this;
        }

        public ILCodeGenerator Dup()
        {
            _gen.Emit(OpCodes.Dup);

            return this;
        }

        public ILCodeGenerator Nop()
        {
            _gen.Emit(OpCodes.Nop);

            return this;
        }

        public ILCodeGenerator Pop()
        {
            _gen.Emit(OpCodes.Pop);

            return this;
        }

        public ILCodeGenerator IgnoreReturnValue()
        {
            _gen.Emit(OpCodes.Pop);

            return this;
        }

        public void Ret()
        {
            _gen.Emit(OpCodes.Ret);
        }

        #endregion

        #endregion

        #region Private Methods

        private static Type[] GetParameterTypes(ParameterInfo[] parameters)
        {
            var result = new Type[parameters.Length];
            for (var i = 0; i < parameters.Length; ++i)
            {
                result[i] = parameters[i].ParameterType;
            }
            return result;
        }

        private static OpCode GetLdindOpCode(TypeCode typeCode)
        {
            switch (typeCode)
            {
                case TypeCode.Boolean:
                    return OpCodes.Ldind_I1;

                case TypeCode.Char:
                    return OpCodes.Ldind_I2;

                case TypeCode.SByte:
                    return OpCodes.Ldind_I1;

                case TypeCode.Byte:
                    return OpCodes.Ldind_U1;

                case TypeCode.Int16:
                    return OpCodes.Ldind_I2;

                case TypeCode.UInt16:
                    return OpCodes.Ldind_U2;

                case TypeCode.Int32:
                    return OpCodes.Ldind_I4;

                case TypeCode.UInt32:
                    return OpCodes.Ldind_U4;

                case TypeCode.Int64:
                    return OpCodes.Ldind_I8;

                case TypeCode.UInt64:
                    return OpCodes.Ldind_I8;

                case TypeCode.Single:
                    return OpCodes.Ldind_R4;

                case TypeCode.Double:
                    return OpCodes.Ldind_R8;

                case TypeCode.String:
                    return OpCodes.Ldind_Ref;
            }
            return OpCodes.Nop;
        }

        private static OpCode GetLdelemOpCode(TypeCode typeCode)
        {
            switch (typeCode)
            {
                case TypeCode.Object:
                case TypeCode.DBNull:
                    return OpCodes.Ldelem_Ref;

                case TypeCode.Boolean:
                    return OpCodes.Ldelem_I1;

                case TypeCode.Char:
                    return OpCodes.Ldelem_I2;

                case TypeCode.SByte:
                    return OpCodes.Ldelem_I1;

                case TypeCode.Byte:
                    return OpCodes.Ldelem_U1;

                case TypeCode.Int16:
                    return OpCodes.Ldelem_I2;

                case TypeCode.UInt16:
                    return OpCodes.Ldelem_U2;

                case TypeCode.Int32:
                    return OpCodes.Ldelem_I4;

                case TypeCode.UInt32:
                    return OpCodes.Ldelem_U4;

                case TypeCode.Int64:
                    return OpCodes.Ldelem_I8;

                case TypeCode.UInt64:
                    return OpCodes.Ldelem_I8;

                case TypeCode.Single:
                    return OpCodes.Ldelem_R4;

                case TypeCode.Double:
                    return OpCodes.Ldelem_R8;

                case TypeCode.String:
                    return OpCodes.Ldelem_Ref;
            }
            return OpCodes.Nop;
        }

        private static bool IsStruct(Type type)
        {
            if (type.IsValueType)
                return !type.IsPrimitive;

            return false;
        }

        private static OpCode GetStelemOpCode(TypeCode typeCode)
        {
            switch (typeCode)
            {
                case TypeCode.Object:
                case TypeCode.DBNull:
                    return OpCodes.Stelem_Ref;

                case TypeCode.Boolean:
                    return OpCodes.Stelem_I1;

                case TypeCode.Char:
                    return OpCodes.Stelem_I2;

                case TypeCode.SByte:
                    return OpCodes.Stelem_I1;

                case TypeCode.Byte:
                    return OpCodes.Stelem_I1;

                case TypeCode.Int16:
                    return OpCodes.Stelem_I2;

                case TypeCode.UInt16:
                    return OpCodes.Stelem_I2;

                case TypeCode.Int32:
                    return OpCodes.Stelem_I4;

                case TypeCode.UInt32:
                    return OpCodes.Stelem_I4;

                case TypeCode.Int64:
                    return OpCodes.Stelem_I8;

                case TypeCode.UInt64:
                    return OpCodes.Stelem_I8;

                case TypeCode.Single:
                    return OpCodes.Stelem_R4;

                case TypeCode.Double:
                    return OpCodes.Stelem_R8;

                case TypeCode.String:
                    return OpCodes.Stelem_Ref;
            }
            return OpCodes.Nop;
        }

        private static OpCode GetConvOpCode(TypeCode typeCode)
        {
            switch (typeCode)
            {
                case TypeCode.Boolean:
                    return OpCodes.Conv_I1;

                case TypeCode.Char:
                    return OpCodes.Conv_I2;

                case TypeCode.SByte:
                    return OpCodes.Conv_I1;

                case TypeCode.Byte:
                    return OpCodes.Conv_U1;

                case TypeCode.Int16:
                    return OpCodes.Conv_I2;

                case TypeCode.UInt16:
                    return OpCodes.Conv_U2;

                case TypeCode.Int32:
                    return OpCodes.Conv_I4;

                case TypeCode.UInt32:
                    return OpCodes.Conv_U4;

                case TypeCode.Int64:
                    return OpCodes.Conv_I8;

                case TypeCode.UInt64:
                    return OpCodes.Conv_I8;

                case TypeCode.Single:
                    return OpCodes.Conv_R4;

                case TypeCode.Double:
                    return OpCodes.Conv_R8;
            }
            return OpCodes.Nop;
        }

        private static OpCode GetBranchCode(Cmp cmp)
        {
            switch (cmp)
            {
                case Cmp.True:
                    return OpCodes.Brfalse;

                case Cmp.False:
                    return OpCodes.Brtrue;

                case Cmp.LessThan:
                    return OpCodes.Bge;

                case Cmp.Equals:
                    return OpCodes.Bne_Un;

                case Cmp.LessThanOrEquals:
                    return OpCodes.Bgt;

                case Cmp.GreaterThanOrEquals:
                    return OpCodes.Blt;

                case Cmp.GreaterThan:
                    return OpCodes.Ble;

                case Cmp.NotEquals:
                    return OpCodes.Beq;
            }

            //never reach here
            return OpCodes.Blt;
        }

        private void LoadExpressions(params ILExpression[] expressions)
        {
            if (expressions != null)
            {
                foreach (var expr in expressions)
                {
                    if (expr != null)
                        expr(this);
                    else
                        LoadNull();
                }
            }
        }

        private void InternalLoadLocalVariableAddress(int slot)
        {
            if (slot <= 0xff)
                _gen.Emit(OpCodes.Ldloca_S, slot);
            else
                _gen.Emit(OpCodes.Ldloca, slot);
        }

        private void InternalConvert(Type sourceType, Type targetType, bool isAddress)
        {
            if (targetType != sourceType)
            {
                if (targetType.IsValueType)
                {
                    if (sourceType.IsValueType)
                    {
                        var opcode = GetConvOpCode(Type.GetTypeCode(targetType));
                        _gen.Emit(opcode);
                    }
                    else
                    {
                        if (!sourceType.IsAssignableFrom(targetType))
                            throw new InvalidOperationException(
                                string.Format(CultureInfo.InvariantCulture, "{0} is not assignable from {1}",
                                    sourceType.FullName, targetType.FullName));

                        _gen.Emit(OpCodes.Unbox, targetType);
                        if (!isAddress)
                            LoadValueTypeObjectFromStack(targetType);
                    }
                }
                else if (targetType.IsAssignableFrom(sourceType))
                {
                    if (sourceType.IsValueType)
                    {
                        if (isAddress)
                            LoadValueTypeObjectFromStack(sourceType);
                        _gen.Emit(OpCodes.Box, sourceType);
                    }
                }
                else
                {
                    _gen.Emit(OpCodes.Castclass, targetType);
                }
            }
        }

        private void InternalIf(Cmp cmp)
        {
            var state = new IfState {EndIfLabel = DefineLabel(), ElseBeginLabel = DefineLabel()};
            _gen.Emit(GetBranchCode(cmp), state.ElseBeginLabel);
            _ifStateStack.Push(state);
        }

        #endregion

        #region Nested Classes

        private enum Cmp
        {
            True,
            False,
            LessThan,
            Equals,
            LessThanOrEquals,
            GreaterThan,
            NotEquals,
            GreaterThanOrEquals
        }

        private sealed class IfState
        {
            public Label ElseBeginLabel;
            public Label EndIfLabel;
        }

        #endregion
    }
}
