using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections;
using System.Data;
using System.Reflection;
using NSun.Data.Reflection;
using System.Reflection.Emit;

namespace NSun.Data.Mapper
{
    public enum MappingType
    {
        Property,
        Field,
        All
    }
    /// <summary>
    /// The abstract mapper builder
    /// </summary>
    public abstract class MapperBuilder
    {
        internal MapperBuilder()
        {
        }

        internal abstract MapperCacheKey CacheKey { get; }

        internal abstract Delegate BuildMapper();
    }

    /// <summary>
    /// The abstract mapper builder
    /// </summary>
    /// <typeparam name="TFrom">The type of from.</typeparam>
    /// <typeparam name="TTo">The type of to.</typeparam>
    public sealed class MapperBuilder<TFrom, TTo> : MapperBuilder
    {
        private Delegate _mapper;
        private readonly MappingType _mappingType;
        private readonly bool _autoMap;
        private readonly bool _ignoreCase;
        private readonly bool _ignoreUnderscore;
        private readonly string[] _ignoreFields;
        private readonly List<Delegate> _mappingChain = new List<Delegate>();
        private bool _expectsTo;
        private static readonly MapperCacheKey _cacheKey = new MapperCacheKey(typeof (TFrom), typeof (TTo));

        #region Constructors

        internal MapperBuilder(bool autoMap, bool ignoreCase, bool ignoreUnderscore,MappingType mapptype, string[] ignoreFields)
        {
            _mappingType = mapptype;
            _autoMap = autoMap;
            _ignoreCase = ignoreCase;
            _ignoreUnderscore = ignoreUnderscore;
            _ignoreFields = ignoreFields;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Get value from
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="from">From.</param>
        /// <returns></returns>
        public MapperBuilder<TFrom, TTo> From<TValue>(MappingFrom<TFrom, TValue> from)
        {
            if (from == null)
                throw new ArgumentNullException("from");
            if (_expectsTo)
                throw new InvalidOperationException("Expects To() but called From().");

            _mappingChain.Add(from);

            _expectsTo = true;

            return this;
        }

        /// <summary>
        /// Set value to
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="to">To.</param>
        /// <returns></returns>
        public MapperBuilder<TFrom, TTo> To(MappingTo<TTo, TFrom> to)
        {
            if (to == null)
                throw new ArgumentNullException("to");
            if (!_expectsTo)
                throw new InvalidOperationException("Expects From() but called To().");

            _mappingChain.Add(to);

            _expectsTo = false;

            return this;
        }

        public MapperBuilder<TFrom, TTo> To<TValue>(MappingTo<TTo, TValue> to)
        {
            if (to == null)
                throw new ArgumentNullException("to");
            if (!_expectsTo)
                throw new InvalidOperationException("Expects From() but called To().");

            _mappingChain.Add(to);

            _expectsTo = false;

            return this;
        }

        #endregion

        #region Non-Public Methods

        #region Overriden

        internal override MapperCacheKey CacheKey
        {
            get { return _cacheKey; }
        }

        internal override Delegate BuildMapper()
        {
            if (_mapper == null)
            {
                lock (this)
                {
                    if (_mapper == null)
                    {
                        if (PrimitiveTypeMapperBuilder.IsPrimitiveTypeMapping(typeof(TFrom), typeof(TTo)))
                        {
                            if (MappingHelper.IsEnumType(typeof(TFrom)) || MappingHelper.IsEnumType(typeof(TTo)))
                                _mapper = MapEnum();
                            else
                                _mapper = new PrimitiveTypeMapperBuilder(typeof(TFrom), typeof(TTo)).BuildMapper();
                        }
                        else if (MappingHelper.IsGuidType(typeof(TFrom)) && MappingHelper.IsGuidType(typeof(TTo)))
                        {
                            _mapper = MapFromGuidToGuid();
                        }
                        else if (typeof(IDataReader).IsAssignableFrom(typeof(TFrom)))
                        {
                            if (typeof(TTo).IsArray)
                                _mapper = MapIDataReaderToArray();
                            else if (typeof(ICollection<>).MakeGenericType(MappingHelper.GetElementType(typeof(TTo))).IsAssignableFrom(typeof(TTo)))
                                _mapper = MapIDataReaderToCollection();
                            else
                                _mapper = MapIDataReaderToObject();
                        }
                        else if (typeof(DataTable).IsAssignableFrom(typeof(TFrom)))
                        {
                            if (typeof(TTo).IsArray)
                                _mapper = MapDataTableToArray();
                            else if (typeof(ICollection<>).MakeGenericType(MappingHelper.GetElementType(typeof(TTo))).IsAssignableFrom(typeof(TTo)))
                                _mapper = MapDataTableToCollection();
                        }
                        else if (typeof(DataRow).IsAssignableFrom(typeof(TFrom)))
                        {
                            _mapper = MapDataRowToObject();
                        }
                        else if (typeof(IEnumerable).IsAssignableFrom(typeof(TFrom)))
                        {
                            if (typeof(TTo).IsArray)
                                _mapper = MapEnumerableToArray();
                            else if (typeof(ICollection<>).MakeGenericType(MappingHelper.GetElementType(typeof(TTo))).IsAssignableFrom(typeof(TTo)))
                                _mapper = MapEnumerableToCollection();
                        }
                        
                        if (_mapper == null)
                        {
                            _mapper = MapObjecToObject();
                        }
                    }
                }
            }

            return _mapper;
        }

        #endregion

        #region Shared

        private static void ExecuteFromTo<TFromValue, TToValue>(
            MapperFactory fac, TFrom fromObj, ref TTo toObj, int fromToIndex)
        {
            if (fac == null)
                throw new ArgumentNullException("fac");

            if (Equals(fromObj, default(TFrom)))
                toObj = default(TTo);

            MapperBuilder builder;
            fac.MapperCache.TryGetValue(new MapperCacheKey(typeof(TFrom), typeof(TTo)), out builder);
            var genericBuilder = builder as MapperBuilder<TFrom, TTo>;
            if (genericBuilder != null && fromToIndex < genericBuilder._mappingChain.Count / 2)
            {
                var from = (MappingFrom<TFrom, TFromValue>)genericBuilder._mappingChain[fromToIndex * 2];
                var to = (MappingTo<TTo, TToValue>)genericBuilder._mappingChain[fromToIndex * 2 + 1];
                var fromValue = from(fromObj);
                if (typeof(TFromValue) != typeof(TToValue))
                {
                    var valueMapper = fac.GetMapper<TFromValue, TToValue>();
                    toObj = to(toObj, valueMapper(fromValue));
                }
                else
                {
                    toObj = to(toObj, (TToValue)(object)fromValue);
                }
            }
        }

        private void EmitMappingChain(ILCodeGenerator gen)
        {
            if (_mappingChain.Count == 0)
                return;

            for (var i = 0; i < _mappingChain.Count / 2; i += 2)
            {
                var fromToIndex = i / 2;
                var fromDelegate = _mappingChain[i];
                var fromValueType = fromDelegate.GetType().GetGenericArguments()[1];
                var toDelegate = _mappingChain[i + 1];
                var toValueType = toDelegate.GetType().GetGenericArguments()[1];

                gen.CallStaticMethod(
                    typeof(MapperBuilder<TFrom, TTo>).GetMethod("ExecuteFromTo", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static).MakeGenericMethod(fromValueType, toValueType),
                    val1 => val1.LoadArgument(0),
                    val2 => val2.LoadArgument(1),
                    val3 => val3.LoadArgument(2),
                    val4 => val4.Load(fromToIndex)
                );
            }
        }

        private bool IsIgnoreField(string fieldName)
        {
            if (string.IsNullOrEmpty(fieldName))
                return false;

            if (_ignoreFields != null)
            {
                foreach (string ignoreField in _ignoreFields)
                {
                    bool matched;
                    if (_ignoreUnderscore)
                    {
                        matched = (string.Compare(
                                fieldName.Replace("_", string.Empty),
                                ignoreField.Replace("_", string.Empty),
                                _ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal
                            ) == 0
                        );
                    }
                    else
                    {
                        matched = (string.Compare(
                                fieldName,
                                ignoreField,
                                _ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal
                            ) == 0
                        );
                    }

                    if (matched)
                        return true;
                }
            }

            return false;
        }

        #endregion

        #region MapEnumerable

        private Delegate MapEnumerableToArray()
        {
            var resultDelegate = typeof (InternalMapper<TFrom, TTo>);
            DynamicMethod toArrayMethod;
            var gen = ILCodeGenerator.CreateDynamicMethodCodeGenerator(
                "m" + Guid.NewGuid().ToString("N"), 
                resultDelegate,
                out toArrayMethod);

            var fromElementType = MappingHelper.GetElementType(typeof(TFrom));
            var toElementType = MappingHelper.GetElementType(typeof(TTo));
            var collectionType = typeof(List<>).MakeGenericType(toElementType);
            var collection = gen.DeclareLocalVariable(collectionType);
            var en = gen.DeclareLocalVariable(typeof (IEnumerator));
            var foreachBegin = gen.DefineLabel();
            var foreachEnd = gen.DefineLabel();
            gen.StoreLocalVariable(
                collection,
                val => val.New(collectionType.GetConstructor(Type.EmptyTypes))
            );
            EmitGetFromEnumerator(gen, en);
            gen.MarkLabel(foreachBegin);
            EmitIfEnumeratorMoveNext(gen, en);
            if (fromElementType.IsValueType)
                EmitMapValueTypeItem(toElementType, gen, collectionType, collection, en, fromElementType);
            else
                EmitMapReferenceTypeItem(toElementType, gen, collectionType, collection, en, fromElementType);
            gen.GoTo(foreachBegin);
            gen.Else();
            gen.GoTo(foreachEnd);
            gen.EndIf();
            gen.MarkLabel(foreachEnd);
            gen.StoreArgumentIndirectly(
                2,
                typeof (TTo),
                val => val.CallMethod(
                    thisObj => thisObj.LoadLocalVariable(collection),
                    collectionType.GetMethod("ToArray")
                )
            );
            gen.Ret();

            var result = toArrayMethod.CreateDelegate(resultDelegate);
            return result;
        }

        private Delegate MapEnumerableToCollection()
        {
            var resultDelegate = typeof(InternalMapper<TFrom, TTo>);
            DynamicMethod toCollectionMethod;
            var gen = ILCodeGenerator.CreateDynamicMethodCodeGenerator(
                "m" + Guid.NewGuid().ToString("N"),
                resultDelegate,
                out toCollectionMethod
            );

            EmitMappingChain(gen);
            var fromElementType = MappingHelper.GetElementType(typeof(TFrom));
            var toElementType = MappingHelper.GetElementType(typeof(TTo));
            var collectionType = typeof(TTo);
            var collection = gen.DeclareLocalVariable(collectionType);
            var en = gen.DeclareLocalVariable(typeof(IEnumerator));
            var foreachBegin = gen.DefineLabel();
            var foreachEnd = gen.DefineLabel();
            gen.StoreLocalVariable(
                collection,
                val => val.LoadArgumentIndirectly(2, typeof(TTo))
            );
            EmitGetFromEnumerator(gen, en);
            gen.MarkLabel(foreachBegin);
            EmitIfEnumeratorMoveNext(gen, en);
            if (fromElementType.IsValueType)
                EmitMapValueTypeItem(toElementType, gen, collectionType, collection, en, fromElementType);
            else
                EmitMapReferenceTypeItem(toElementType, gen, collectionType, collection, en, fromElementType);
            gen.GoTo(foreachBegin);
            gen.Else();
            gen.GoTo(foreachEnd);
            gen.EndIf();
            gen.MarkLabel(foreachEnd);
            if (_autoMap)
                EmitAutoMapFromObject(gen, collection);
            gen.StoreArgumentIndirectly(
                2,
                typeof(TTo),
                val => val.LoadLocalVariable(collection)
            );
            gen.Ret();

            var result = toCollectionMethod.CreateDelegate(resultDelegate);
            return result;
        }

        private void EmitGetFromEnumerator(ILCodeGenerator gen, LocalBuilder en)
        {
            gen.StoreLocalVariable(
                en,
                val => val.CallMethod(
                    thisObj => thisObj.ConvertValue(
                        sourceVal => sourceVal.LoadArgument(1),
                        typeof(TFrom),
                        typeof(IEnumerable)
                    ),
                    typeof(IEnumerable).GetMethod("GetEnumerator")
                )
            );
        }

        private void EmitIfEnumeratorMoveNext(ILCodeGenerator gen, LocalBuilder en)
        {
            gen.If(
                boolVal => boolVal.CallMethod(
                               thisObj => thisObj.LoadLocalVariable(en),
                               typeof (IEnumerator).GetMethod("MoveNext")
                               )
                );
        }

        private void EmitMapValueTypeItem(Type toElementType, ILCodeGenerator gen, Type collectionType, LocalBuilder collection, LocalBuilder en, Type fromElementType)
        {
            gen.CallMethod(
                thisObj => thisObj.ConvertValue(
                    sourceVal => sourceVal.LoadLocalVariable(collection),
                    collectionType,
                    typeof(ICollection<>).MakeGenericType(toElementType)
                ),
                typeof(ICollection<>).MakeGenericType(toElementType).GetMethod("Add"),
                val1 => val1.CallMethod(
                    thisObj2 => thisObj2.CallMethod(
                        thisObj3 => thisObj3.LoadArgument(0),
                        typeof(MapperFactory).GetMethod("GetMapper").MakeGenericMethod(
                            fromElementType, toElementType)
                    ),
                    typeof(Mapper<,>).MakeGenericType(fromElementType, toElementType).GetMethod("Invoke"),
                    valFrom => valFrom.UnboxAny(
                        fromElementType,
                        val => val.LoadProperty(
                            thisObj2 =>
                            thisObj2.LoadLocalVariable(en),
                            typeof(IEnumerator).GetProperty("Current")
                        )
                    )
                )
            );
        }

        private void EmitMapReferenceTypeItem(Type toElementType, ILCodeGenerator gen, Type collectionType, LocalBuilder collection, LocalBuilder en, Type fromElementType)
        {
            gen.CallMethod(
                thisObj => thisObj.ConvertValue(
                    sourceVal => sourceVal.LoadLocalVariable(collection),
                    collectionType,
                    typeof(ICollection<>).MakeGenericType(toElementType)
                ),
                typeof(ICollection<>).MakeGenericType(toElementType).GetMethod("Add"),
                val1 => val1.CallMethod(
                    thisObj2 => thisObj2.CallMethod(
                        thisObj3 => thisObj3.LoadArgument(0),
                        typeof(MapperFactory).GetMethod("GetMapper").MakeGenericMethod(fromElementType, toElementType)
                    ),
                    typeof(Mapper<,>).MakeGenericType(fromElementType, toElementType).GetMethod("Invoke"),
                    valFrom => valFrom.ConvertValue(
                        sourceVal2 => sourceVal2.LoadProperty(
                            thisObj2 => thisObj2.LoadLocalVariable(en),
                            typeof(IEnumerator).GetProperty("Current")
                        ),
                       typeof(object),
                       fromElementType
                    )
                )
            );
        }

        #endregion

        #region MapIDataReader

        private Delegate MapIDataReaderToArray()
        {
            var resultDelegate = typeof(InternalMapper<TFrom, TTo>);
            DynamicMethod toArrayMethod;
            var gen = ILCodeGenerator.CreateDynamicMethodCodeGenerator(
                "m" + Guid.NewGuid().ToString("N"),
                resultDelegate,
                out toArrayMethod);

            var toElementType = MappingHelper.GetElementType(typeof(TTo));
            var collectionType = typeof(List<>).MakeGenericType(toElementType);
            var collection = gen.DeclareLocalVariable(collectionType);
            var foreachBegin = gen.DefineLabel();
            var foreachEnd = gen.DefineLabel();
            gen.StoreLocalVariable(
                collection,
                val => val.New(collectionType.GetConstructor(Type.EmptyTypes))
            );
            gen.MarkLabel(foreachBegin);
            EmitIfIDataReaderRead(gen);
            EmitMapIDataReaderItem(toElementType, gen, collectionType, collection);
            gen.GoTo(foreachBegin);
            gen.Else();
            gen.GoTo(foreachEnd);
            gen.EndIf();
            gen.MarkLabel(foreachEnd);
            gen.StoreArgumentIndirectly(
                2,
                typeof(TTo),
                val => val.CallMethod(
                    thisObj => thisObj.LoadLocalVariable(collection),
                    collectionType.GetMethod("ToArray")
                )
            );
            gen.Ret();

            var result = toArrayMethod.CreateDelegate(resultDelegate);
            return result;
        }

        private Delegate MapIDataReaderToCollection()
        {
            var resultDelegate = typeof(InternalMapper<TFrom, TTo>);
            DynamicMethod toCollectionMethod;
            var gen = ILCodeGenerator.CreateDynamicMethodCodeGenerator(
                "m" + Guid.NewGuid().ToString("N"),
                resultDelegate,
                out toCollectionMethod
            );

            EmitMappingChain(gen);
            var toElementType = MappingHelper.GetElementType(typeof(TTo));
            var collectionType = typeof(TTo);
            var collection = gen.DeclareLocalVariable(collectionType);
            var foreachBegin = gen.DefineLabel();
            var foreachEnd = gen.DefineLabel();
            gen.StoreLocalVariable(
                collection,
                val => val.LoadArgumentIndirectly(2, typeof(TTo))
            );
            gen.MarkLabel(foreachBegin);
            EmitIfIDataReaderRead(gen);
            EmitMapIDataReaderItem(toElementType, gen, collectionType, collection);
            gen.GoTo(foreachBegin);
            gen.Else();
            gen.GoTo(foreachEnd);
            gen.EndIf();
            gen.MarkLabel(foreachEnd);
            gen.StoreArgumentIndirectly(
                2,
                typeof(TTo),
                val => val.LoadLocalVariable(collection)
            );
            gen.Ret();

            var result = toCollectionMethod.CreateDelegate(resultDelegate);
            return result;
        }

        private Delegate MapIDataReaderToObject()
        {
            var resultDelegate = typeof(InternalMapper<TFrom, TTo>);
            DynamicMethod toObjectMethod;
            var gen = ILCodeGenerator.CreateDynamicMethodCodeGenerator(
                "m" + Guid.NewGuid().ToString("N"),
                resultDelegate,
                out toObjectMethod);

            EmitMappingChain(gen);
            var obj = gen.DeclareLocalVariable(typeof(TTo));
            gen.StoreLocalVariable(
                obj,
                val => val.LoadArgumentIndirectly(2, typeof(TTo))
            );
            if (_autoMap)
                EmitAutoMapFromIDataReader(gen, obj);
            gen.StoreArgumentIndirectly(
                2,
                typeof(TTo),
                val => val.LoadLocalVariable(obj)
            );
            gen.Ret();

            var result = toObjectMethod.CreateDelegate(resultDelegate);
            return result;
        }

        internal static DataColumn GetIDataReaderMappingColumn(IDataReader reader, string columnName, bool ignoreUnderscore)
        {
            for (var i = 0; i < reader.FieldCount; ++i)
            {
                var readerColumnName = reader.GetName(i);
                bool matched;
                if (ignoreUnderscore)
                {
                    matched = (string.Compare(
                            readerColumnName.Replace("_", string.Empty),
                            columnName.Replace("_", string.Empty),
                            StringComparison.OrdinalIgnoreCase
                        ) == 0
                    );
                }
                else
                {
                    matched = (string.Compare(
                            readerColumnName,
                            columnName,
                            StringComparison.OrdinalIgnoreCase
                        ) == 0
                    );
                }
                if (matched)
                    return new DataColumn(readerColumnName, reader.GetFieldType(i));
            }

            return null;
        }

        private void EmitAutoMapFromIDataReader(ILCodeGenerator gen, LocalBuilder obj)
        {
            //map from IDataReader fields to to object properties/fields
            var baseType = typeof(TTo);
            while (baseType != typeof(object) && baseType != typeof(ValueType))
            {
                switch (_mappingType)
                {
                    case MappingType.Property:
                        foreach (var property in baseType.GetProperties())
                        {
                            if (!property.CanWrite || IsIgnoreField(property.Name))
                                continue;

                            var targetProperty = property;
                            var dataColumn = gen.DeclareLocalVariable(typeof(DataColumn));
                            EmitStoreIDataReaderMappingColumnToLocalVariable(gen, targetProperty.Name, dataColumn);
                            EmitIfNotMappingColumnIsNull(gen, dataColumn);
                            var columnValue = gen.DeclareLocalVariable(typeof(object));
                            EmitStoreIDataReaderColumnValueToLocalVariable(gen, dataColumn, columnValue);
                            EmitIfColumnValueIsDbNull(gen, columnValue);
                            gen.StoreProperty(
                                typeof(TTo).IsValueType ?
                                new ILExpression(thisObj => thisObj.LoadLocalVariableAddress(obj))
                                :
                                new ILExpression(thisObj => thisObj.LoadLocalVariable(obj)),
                                targetProperty,
                                val => val.LoadDefaultValue(targetProperty.PropertyType)
                            );
                            gen.Else();
                            gen.StoreProperty(
                                typeof(TTo).IsValueType ?
                                new ILExpression(thisObj => thisObj.LoadLocalVariableAddress(obj))
                                :
                                new ILExpression(thisObj => thisObj.LoadLocalVariable(obj)),
                                targetProperty,
                                val => val.ConvertValue(
                                    sourceVal => sourceVal.LoadLocalVariable(columnValue),
                                    typeof(object),
                                    targetProperty.PropertyType
                                )
                            );
                            gen.EndIf();
                            gen.EndIf();
                        }
                        break;
                    case MappingType.Field:
                        foreach (var field in baseType.GetFields())
                        {
                            if (IsIgnoreField(field.Name))
                                continue;

                            var targetField = field;
                            var dataColumn = gen.DeclareLocalVariable(typeof(DataColumn));
                            EmitStoreIDataReaderMappingColumnToLocalVariable(gen, targetField.Name, dataColumn);
                            EmitIfNotMappingColumnIsNull(gen, dataColumn);
                            var columnValue = gen.DeclareLocalVariable(typeof(object));
                            EmitStoreIDataReaderColumnValueToLocalVariable(gen, dataColumn, columnValue);
                            EmitIfColumnValueIsDbNull(gen, columnValue);
                            gen.StoreField(
                                typeof(TTo).IsValueType ?
                                new ILExpression(thisObj => thisObj.LoadLocalVariableAddress(obj))
                                :
                                new ILExpression(thisObj => thisObj.LoadLocalVariable(obj)),
                                targetField,
                                val => val.LoadDefaultValue(targetField.FieldType)
                            );
                            gen.Else();
                            gen.StoreField(
                                typeof(TTo).IsValueType ?
                                new ILExpression(thisObj => thisObj.LoadLocalVariableAddress(obj))
                                :
                                new ILExpression(thisObj => thisObj.LoadLocalVariable(obj)),
                                targetField,
                                val => val.ConvertValue(
                                    sourceVal => sourceVal.LoadLocalVariable(columnValue),
                                    typeof(object),
                                    targetField.FieldType
                                )
                            );
                            gen.EndIf();
                            gen.EndIf();
                        }
                        break;
                    case MappingType.All:
                        foreach (var property in baseType.GetProperties())
                        {
                            if (!property.CanWrite || IsIgnoreField(property.Name))
                                continue;

                            var targetProperty = property;
                            var dataColumn = gen.DeclareLocalVariable(typeof(DataColumn));
                            EmitStoreIDataReaderMappingColumnToLocalVariable(gen, targetProperty.Name, dataColumn);
                            EmitIfNotMappingColumnIsNull(gen, dataColumn);
                            var columnValue = gen.DeclareLocalVariable(typeof(object));
                            EmitStoreIDataReaderColumnValueToLocalVariable(gen, dataColumn, columnValue);
                            EmitIfColumnValueIsDbNull(gen, columnValue);
                            gen.StoreProperty(
                                typeof(TTo).IsValueType ?
                                new ILExpression(thisObj => thisObj.LoadLocalVariableAddress(obj))
                                :
                                new ILExpression(thisObj => thisObj.LoadLocalVariable(obj)),
                                targetProperty,
                                val => val.LoadDefaultValue(targetProperty.PropertyType)
                            );
                            gen.Else();
                            gen.StoreProperty(
                                typeof(TTo).IsValueType ?
                                new ILExpression(thisObj => thisObj.LoadLocalVariableAddress(obj))
                                :
                                new ILExpression(thisObj => thisObj.LoadLocalVariable(obj)),
                                targetProperty,
                                val => val.ConvertValue(
                                    sourceVal => sourceVal.LoadLocalVariable(columnValue),
                                    typeof(object),
                                    targetProperty.PropertyType
                                )
                            );
                            gen.EndIf();
                            gen.EndIf();
                        }

                        foreach (var field in baseType.GetFields())
                        {
                            if (IsIgnoreField(field.Name))
                                continue;

                            var targetField = field;
                            var dataColumn = gen.DeclareLocalVariable(typeof(DataColumn));
                            EmitStoreIDataReaderMappingColumnToLocalVariable(gen, targetField.Name, dataColumn);
                            EmitIfNotMappingColumnIsNull(gen, dataColumn);
                            var columnValue = gen.DeclareLocalVariable(typeof(object));
                            EmitStoreIDataReaderColumnValueToLocalVariable(gen, dataColumn, columnValue);
                            EmitIfColumnValueIsDbNull(gen, columnValue);
                            gen.StoreField(
                                typeof(TTo).IsValueType ?
                                new ILExpression(thisObj => thisObj.LoadLocalVariableAddress(obj))
                                :
                                new ILExpression(thisObj => thisObj.LoadLocalVariable(obj)),
                                targetField,
                                val => val.LoadDefaultValue(targetField.FieldType)
                            );
                            gen.Else();
                            gen.StoreField(
                                typeof(TTo).IsValueType ?
                                new ILExpression(thisObj => thisObj.LoadLocalVariableAddress(obj))
                                :
                                new ILExpression(thisObj => thisObj.LoadLocalVariable(obj)),
                                targetField,
                                val => val.ConvertValue(
                                    sourceVal => sourceVal.LoadLocalVariable(columnValue),
                                    typeof(object),
                                    targetField.FieldType
                                )
                            );
                            gen.EndIf();
                            gen.EndIf();
                        }
                        break;
                    default:
                        break;
                }
               

                baseType = baseType.BaseType;
            }
        }

        private void EmitStoreIDataReaderColumnValueToLocalVariable(ILCodeGenerator gen, LocalBuilder dataColumn, LocalBuilder columnValue)
        {
            gen.StoreLocalVariable(
                columnValue,
                val => val.CallMethod(
                    thisObj => thisObj.ConvertValue(
                        sourceVal => sourceVal.LoadArgument(1),
                        typeof(TFrom),
                        typeof(IDataRecord)
                    ),
                    typeof(IDataRecord).GetMethod("GetValue", new[] { typeof(int) }),
                    columnIndex => columnIndex.CallMethod(
                        thisObj => thisObj.ConvertValue(
                            sourceVal => sourceVal.LoadArgument(1),
                            typeof(TFrom),
                            typeof(IDataRecord)
                        ),
                        typeof(IDataRecord).GetMethod("GetOrdinal", new[] { typeof(string) }),
                        columnName => columnName.LoadProperty(
                            thisObj => thisObj.LoadLocalVariable(dataColumn),
                            typeof(DataColumn).GetProperty("ColumnName")
                        )
                    )
                )
            );
        }

        private void EmitStoreIDataReaderMappingColumnToLocalVariable(ILCodeGenerator gen, string columnName, LocalBuilder dataColumn)
        {
            gen.StoreLocalVariable(
                dataColumn,
                val => val.CallStaticMethod(
                    typeof(MapperBuilder<TFrom, TTo>).GetMethod("GetIDataReaderMappingColumn", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static),
                    reader => reader.LoadArgument(1),
                    columnName2 => columnName2.Load(columnName),
                    ignoreUnderscore => ignoreUnderscore.Load(_ignoreUnderscore)
                )
            );
        }

        private void EmitMapIDataReaderItem(Type toElementType, ILCodeGenerator gen, Type collectionType, LocalBuilder collection)
        {
            gen.CallMethod(
                thisObj => thisObj.ConvertValue(
                    sourceVal => sourceVal.LoadLocalVariable(collection),
                    collectionType,
                    typeof(ICollection<>).MakeGenericType(toElementType)
                ),
                typeof(ICollection<>).MakeGenericType(toElementType).GetMethod("Add"),
                val1 => val1.CallMethod(
                    thisObj2 => thisObj2.CallMethod(
                        thisObj3 => thisObj3.LoadArgument(0),
                        typeof(MapperFactory).GetMethod("GetMapper").MakeGenericMethod(typeof(TFrom), toElementType)
                    ),
                    typeof(Mapper<,>).MakeGenericType(typeof(TFrom), toElementType).GetMethod("Invoke"),
                    valFrom => valFrom.LoadArgument(1)
                )
            );
        }

        private void EmitIfIDataReaderRead(ILCodeGenerator gen)
        {
            gen.If(
                boolVal => boolVal.CallMethod(
                    thisObj => thisObj.ConvertValue(
                        sourceVal => sourceVal.LoadArgument(1),
                        typeof(TFrom),
                        typeof(IDataReader)
                    ),
                    typeof(IDataReader).GetMethod("Read")
                )
            );
        }

        #endregion

        #region MapDataTable

        private Delegate MapDataTableToArray()
        {
            var resultDelegate = typeof(InternalMapper<TFrom, TTo>);
            DynamicMethod toArrayMethod;
            var gen = ILCodeGenerator.CreateDynamicMethodCodeGenerator(
                "m" + Guid.NewGuid().ToString("N"),
                resultDelegate,
                out toArrayMethod);

            var toElementType = MappingHelper.GetElementType(typeof(TTo));
            var collectionType = typeof(List<>).MakeGenericType(toElementType);
            var collection = gen.DeclareLocalVariable(collectionType);
            var en = gen.DeclareLocalVariable(typeof(IEnumerator));
            var foreachBegin = gen.DefineLabel();
            var foreachEnd = gen.DefineLabel();
            gen.StoreLocalVariable(
                collection,
                val => val.New(collectionType.GetConstructor(Type.EmptyTypes))
            );
            EmitGetFromDataTableEnumerator(gen, en);
            gen.MarkLabel(foreachBegin);
            EmitIfEnumeratorMoveNext(gen, en);
            EmitMapReferenceTypeItem(toElementType, gen, collectionType, collection, en, typeof(DataRow));
            gen.GoTo(foreachBegin);
            gen.Else();
            gen.GoTo(foreachEnd);
            gen.EndIf();
            gen.MarkLabel(foreachEnd);
            gen.StoreArgumentIndirectly(
                2,
                typeof(TTo),
                val => val.CallMethod(
                    thisObj => thisObj.LoadLocalVariable(collection),
                    collectionType.GetMethod("ToArray")
                )
            );
            gen.Ret();

            var result = toArrayMethod.CreateDelegate(resultDelegate);
            return result;
        }

        private Delegate MapDataTableToCollection()
        {
            var resultDelegate = typeof(InternalMapper<TFrom, TTo>);
            DynamicMethod toCollectionMethod;
            var gen = ILCodeGenerator.CreateDynamicMethodCodeGenerator(
                "m" + Guid.NewGuid().ToString("N"),
                resultDelegate,
                out toCollectionMethod
            );

            EmitMappingChain(gen);
            var toElementType = MappingHelper.GetElementType(typeof(TTo));
            var collectionType = typeof(TTo);
            var collection = gen.DeclareLocalVariable(collectionType);
            var en = gen.DeclareLocalVariable(typeof(IEnumerator));
            var foreachBegin = gen.DefineLabel();
            var foreachEnd = gen.DefineLabel();
            gen.StoreLocalVariable(
                collection,
                val => val.LoadArgumentIndirectly(2, typeof(TTo))
            );
            EmitGetFromDataTableEnumerator(gen, en);
            gen.MarkLabel(foreachBegin);
            EmitIfEnumeratorMoveNext(gen, en);
            EmitMapReferenceTypeItem(toElementType, gen, collectionType, collection, en, typeof(DataRow));
            gen.GoTo(foreachBegin);
            gen.Else();
            gen.GoTo(foreachEnd);
            gen.EndIf();
            gen.MarkLabel(foreachEnd);
            gen.StoreArgumentIndirectly(
                2,
                typeof(TTo),
                val => val.LoadLocalVariable(collection)
            );
            gen.Ret();

            var result = toCollectionMethod.CreateDelegate(resultDelegate);
            return result;
        }

        private Delegate MapDataRowToObject()
        {
            var resultDelegate = typeof(InternalMapper<TFrom, TTo>);
            DynamicMethod toObjectMethod;
            var gen = ILCodeGenerator.CreateDynamicMethodCodeGenerator(
                "m" + Guid.NewGuid().ToString("N"),
                resultDelegate,
                out toObjectMethod);

            EmitMappingChain(gen);
            var obj = gen.DeclareLocalVariable(typeof(TTo));
            gen.StoreLocalVariable(
                obj,
                val => val.LoadArgumentIndirectly(2, typeof(TTo))
            );
            if (_autoMap)
                EmitAutoMapFromDataRow(gen, obj);
            gen.StoreArgumentIndirectly(
                2,
                typeof(TTo),
                val => val.LoadLocalVariable(obj)
            );
            gen.Ret();

            var result = toObjectMethod.CreateDelegate(resultDelegate);
            return result;
        }

        internal static DataColumn GetDataRowMappingColumn(DataRow row, string columnName, bool ignoreUnderscore)
        {
            foreach (DataColumn column in row.Table.Columns)
            {
                bool matched;
                if (ignoreUnderscore)
                {
                    matched = (string.Compare(
                            column.ColumnName.Replace("_", string.Empty),
                            columnName.Replace("_", string.Empty),
                            StringComparison.OrdinalIgnoreCase
                        ) == 0
                    );
                }
                else
                {
                    matched = (string.Compare(
                            column.ColumnName,
                            columnName,
                            StringComparison.OrdinalIgnoreCase
                        ) == 0
                    );
                }
                if (matched)
                    return column;
            }

            return null;
        }

        private void EmitGetFromDataTableEnumerator(ILCodeGenerator gen, LocalBuilder en)
        {
            gen.StoreLocalVariable(
                en,
                val => val.CallMethod(
                    thisObj => thisObj.ConvertValue(
                        sourceVal => sourceVal.LoadProperty(
                            thisObj2 => thisObj2.LoadArgument(1),
                            typeof(DataTable).GetProperty("Rows")
                        ),
                        typeof(TFrom),
                        typeof(IEnumerable)
                    ),
                    typeof(IEnumerable).GetMethod("GetEnumerator")
                )
            );
        }

        private void EmitAutoMapFromDataRow(ILCodeGenerator gen, LocalBuilder local)
        {
            //map from DataRow fields to to object properties/fields
            var baseType = typeof(TTo);
            while (baseType != typeof(object) && baseType != typeof(ValueType))
            {
                switch (_mappingType)
                {
                    case MappingType.Property:
                        foreach (var property in baseType.GetProperties())
                        {
                            if (!property.CanWrite || IsIgnoreField(property.Name))
                                continue;

                            var targetProperty = property;
                            var dataColumn = gen.DeclareLocalVariable(typeof(DataColumn));
                            EmitStoreDataRowMappingColumnToLocalVariable(gen, targetProperty.Name, dataColumn);
                            EmitIfNotMappingColumnIsNull(gen, dataColumn);
                            var columnValue = gen.DeclareLocalVariable(typeof(object));
                            EmitStoreDataRowColumnValueToLocalVariable(gen, dataColumn, columnValue);
                            EmitIfColumnValueIsDbNull(gen, columnValue);
                            gen.StoreProperty(
                                typeof(TTo).IsValueType ?
                                new ILExpression(thisObj => thisObj.LoadLocalVariableAddress(local))
                                :
                                new ILExpression(thisObj => thisObj.LoadLocalVariable(local)),
                                targetProperty,
                                val => val.LoadDefaultValue(targetProperty.PropertyType)
                            );
                            gen.Else();
                            gen.StoreProperty(
                                typeof(TTo).IsValueType ?
                                new ILExpression(thisObj => thisObj.LoadLocalVariableAddress(local))
                                :
                                new ILExpression(thisObj => thisObj.LoadLocalVariable(local)),
                                targetProperty,
                                val => val.ConvertValue(
                                    sourceVal => sourceVal.LoadLocalVariable(columnValue),
                                    typeof(object),
                                    targetProperty.PropertyType
                                )
                            );
                            gen.EndIf();
                            gen.EndIf();
                        }
                        break;
                    case MappingType.Field:
                        foreach (var field in baseType.GetFields())
                        {
                            if (IsIgnoreField(field.Name))
                                continue;

                            var targetField = field;
                            var dataColumn = gen.DeclareLocalVariable(typeof(DataColumn));
                            EmitStoreDataRowMappingColumnToLocalVariable(gen, targetField.Name, dataColumn);
                            EmitIfNotMappingColumnIsNull(gen, dataColumn);
                            var columnValue = gen.DeclareLocalVariable(typeof(object));
                            EmitStoreDataRowColumnValueToLocalVariable(gen, dataColumn, columnValue);
                            EmitIfColumnValueIsDbNull(gen, columnValue);
                            gen.StoreField(
                                typeof(TTo).IsValueType ?
                                new ILExpression(thisObj => thisObj.LoadLocalVariableAddress(local))
                                :
                                new ILExpression(thisObj => thisObj.LoadLocalVariable(local)),
                                targetField,
                                val => val.LoadDefaultValue(targetField.FieldType)
                            );
                            gen.Else();
                            gen.StoreField(
                                typeof(TTo).IsValueType ?
                                new ILExpression(thisObj => thisObj.LoadLocalVariableAddress(local))
                                :
                                new ILExpression(thisObj => thisObj.LoadLocalVariable(local)),
                                targetField,
                                val => val.ConvertValue(
                                    sourceVal => sourceVal.LoadLocalVariable(columnValue),
                                    typeof(object),
                                    targetField.FieldType
                                )
                            );
                            gen.EndIf();
                            gen.EndIf();
                        }
                        break;
                    case MappingType.All:
                        foreach (var property in baseType.GetProperties())
                        {
                            if (!property.CanWrite || IsIgnoreField(property.Name))
                                continue;

                            var targetProperty = property;
                            var dataColumn = gen.DeclareLocalVariable(typeof(DataColumn));
                            EmitStoreDataRowMappingColumnToLocalVariable(gen, targetProperty.Name, dataColumn);
                            EmitIfNotMappingColumnIsNull(gen, dataColumn);
                            var columnValue = gen.DeclareLocalVariable(typeof(object));
                            EmitStoreDataRowColumnValueToLocalVariable(gen, dataColumn, columnValue);
                            EmitIfColumnValueIsDbNull(gen, columnValue);
                            gen.StoreProperty(
                                typeof(TTo).IsValueType ?
                                new ILExpression(thisObj => thisObj.LoadLocalVariableAddress(local))
                                :
                                new ILExpression(thisObj => thisObj.LoadLocalVariable(local)),
                                targetProperty,
                                val => val.LoadDefaultValue(targetProperty.PropertyType)
                            );
                            gen.Else();
                            gen.StoreProperty(
                                typeof(TTo).IsValueType ?
                                new ILExpression(thisObj => thisObj.LoadLocalVariableAddress(local))
                                :
                                new ILExpression(thisObj => thisObj.LoadLocalVariable(local)),
                                targetProperty,
                                val => val.ConvertValue(
                                    sourceVal => sourceVal.LoadLocalVariable(columnValue),
                                    typeof(object),
                                    targetProperty.PropertyType
                                )
                            );
                            gen.EndIf();
                            gen.EndIf();
                        }
                        foreach (var field in baseType.GetFields())
                        {
                            if (IsIgnoreField(field.Name))
                                continue;

                            var targetField = field;
                            var dataColumn = gen.DeclareLocalVariable(typeof(DataColumn));
                            EmitStoreDataRowMappingColumnToLocalVariable(gen, targetField.Name, dataColumn);
                            EmitIfNotMappingColumnIsNull(gen, dataColumn);
                            var columnValue = gen.DeclareLocalVariable(typeof(object));
                            EmitStoreDataRowColumnValueToLocalVariable(gen, dataColumn, columnValue);
                            EmitIfColumnValueIsDbNull(gen, columnValue);
                            gen.StoreField(
                                typeof(TTo).IsValueType ?
                                new ILExpression(thisObj => thisObj.LoadLocalVariableAddress(local))
                                :
                                new ILExpression(thisObj => thisObj.LoadLocalVariable(local)),
                                targetField,
                                val => val.LoadDefaultValue(targetField.FieldType)
                            );
                            gen.Else();
                            gen.StoreField(
                                typeof(TTo).IsValueType ?
                                new ILExpression(thisObj => thisObj.LoadLocalVariableAddress(local))
                                :
                                new ILExpression(thisObj => thisObj.LoadLocalVariable(local)),
                                targetField,
                                val => val.ConvertValue(
                                    sourceVal => sourceVal.LoadLocalVariable(columnValue),
                                    typeof(object),
                                    targetField.FieldType
                                )
                            );
                            gen.EndIf();
                            gen.EndIf();
                        }
                        break;
                    default:
                        break;
                }
                

                baseType = baseType.BaseType;
            }
        }

        private void EmitIfColumnValueIsDbNull(ILCodeGenerator gen, LocalBuilder columnValue)
        {
            gen.If(
                boolVal => boolVal.CallReferenceEquals(
                    left => left.LoadLocalVariable(columnValue),
                    right => right.LoadStaticField(
                        typeof(DBNull).GetField("Value", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public)
                    )
                )
            );
        }

        private void EmitStoreDataRowColumnValueToLocalVariable(ILCodeGenerator gen, LocalBuilder dataColumn, LocalBuilder columnValue)
        {
            gen.StoreLocalVariable(
                columnValue,
                val => val.CallMethod(
                    thisObj => thisObj.LoadArgument(1),
                    typeof(DataRow).GetMethod("get_Item", new[] { typeof(DataColumn) }),
                    column => column.LoadLocalVariable(dataColumn)
                )
            );
        }

        private void EmitIfNotMappingColumnIsNull(ILCodeGenerator gen, LocalBuilder dataColumn)
        {
            gen.IfNot(
                boolVal => boolVal.CallReferenceEquals(
                    left => left.LoadLocalVariable(dataColumn),
                    right => right.LoadNull()
                )
            );
        }

        private void EmitStoreDataRowMappingColumnToLocalVariable(ILCodeGenerator gen, string columnName, LocalBuilder dataColumn)
        {
            gen.StoreLocalVariable(
                dataColumn,
                val => val.CallStaticMethod(
                    typeof(MapperBuilder<TFrom, TTo>).GetMethod("GetDataRowMappingColumn", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static),
                    row => row.LoadArgument(1),
                    columnName2 => columnName2.Load(columnName),
                    ignoreUnderscore => ignoreUnderscore.Load(_ignoreUnderscore)
                )
            );
        }

        #endregion

        #region MapObject

        private Delegate MapEnum()
        {
            var resultDelegate = typeof(InternalMapper<TFrom, TTo>);
            DynamicMethod toObjectMethod;
            var gen = ILCodeGenerator.CreateDynamicMethodCodeGenerator(
                "m" + Guid.NewGuid().ToString("N"),
                resultDelegate,
                out toObjectMethod);

            var underlyingFromType = MappingHelper.GetUnderlyingType(typeof(TFrom));
            var underlyingToType = MappingHelper.GetUnderlyingType(typeof(TTo));
            gen.StoreArgumentIndirectly(
                2,
                underlyingToType,
                val => val.CallMethod(
                    thisObj2 => thisObj2.CallMethod(
                        thisObj3 => thisObj3.LoadArgument(0),
                        typeof(MapperFactory).GetMethod("GetMapper").MakeGenericMethod(
                            underlyingFromType, underlyingToType)
                    ),
                    typeof(Mapper<,>).MakeGenericType(underlyingFromType, underlyingToType).GetMethod("Invoke"),
                    val2 => val2.LoadArgument(1)
                )
            );
            gen.Ret();

            var result = toObjectMethod.CreateDelegate(resultDelegate);
            return result;
        }

        private Delegate MapFromGuidToGuid()
        {
            if (MappingHelper.IsNullableType(typeof(TFrom))
                && MappingHelper.IsNullableType(typeof(TTo)))
            {
                return new InternalMapper<Guid?, Guid?>(delegate(MapperFactory fac, Guid? from, ref Guid? to)
                {
                    to = from;
                });
            }
            else if (MappingHelper.IsNullableType(typeof(TFrom))
                && !MappingHelper.IsNullableType(typeof(TTo)))
            {
                return new InternalMapper<Guid?, Guid>(delegate(MapperFactory fac, Guid? from, ref Guid to)
                {
                    to = from.HasValue ? from.Value : default(Guid);
                });
            }
            else if (!MappingHelper.IsNullableType(typeof(TFrom))
                && MappingHelper.IsNullableType(typeof(TTo)))
            {
                return new InternalMapper<Guid, Guid?>(delegate(MapperFactory fac, Guid from, ref Guid? to)
                {
                    to = from;
                });
            }
            else
            {
                return new InternalMapper<Guid, Guid>(delegate(MapperFactory fac, Guid from, ref Guid to)
                {
                    to = from;
                });
            }
        }

        private Delegate MapObjecToObject()
        {
            var resultDelegate = typeof(InternalMapper<TFrom, TTo>);
            DynamicMethod toObjectMethod;
            var gen = ILCodeGenerator.CreateDynamicMethodCodeGenerator(
                "m" + Guid.NewGuid().ToString("N"),
                resultDelegate,
                out toObjectMethod);

            EmitMappingChain(gen);
            var obj = gen.DeclareLocalVariable(typeof(TTo));
            gen.StoreLocalVariable(
                obj,
                val => val.LoadArgumentIndirectly(2, typeof(TTo))
            );
            if (_autoMap)
                EmitAutoMapFromObject(gen, obj);
            gen.StoreArgumentIndirectly(
                2,
                typeof(TTo),
                val => val.LoadLocalVariable(obj)
            );
            gen.Ret();

            var result = toObjectMethod.CreateDelegate(resultDelegate);
            return result;
        }
         
        private void EmitAutoMapFromObject(ILCodeGenerator gen, LocalBuilder local)
        {
            //map from object properties/fields to to object properties/fields
            var baseType = typeof(TFrom);
            while (baseType != typeof(object) && baseType != typeof(ValueType))
            {
                switch (_mappingType)
                {
                    case MappingType.Property:
                        foreach (var property in baseType.GetProperties())
                        {
                            if (!property.CanRead || IsIgnoreField(property.Name))
                                continue;

                            var targetProperty = MappingHelper.GetPropertyInfo(typeof(TTo), property.Name, _ignoreCase, _ignoreUnderscore);
                            if (targetProperty != null && targetProperty.CanWrite)
                            {
                                var sourceProperty = property;
                                gen.StoreProperty(
                                    typeof(TTo).IsValueType ?
                                    new ILExpression(thisObj => thisObj.LoadLocalVariableAddress(local))
                                    :
                                    new ILExpression(thisObj => thisObj.LoadLocalVariable(local)),
                                    targetProperty,
                                    sourceProperty.PropertyType == targetProperty.PropertyType ?
                                    typeof(TFrom).IsValueType ?
                                    new ILExpression(valFrom => valFrom.LoadProperty(
                                        thisObj2 => thisObj2.LoadArgumentAddress(1),
                                        sourceProperty
                                    ))
                                    :
                                    new ILExpression(valFrom => valFrom.LoadProperty(
                                        thisObj2 => thisObj2.LoadArgument(1),
                                        sourceProperty
                                    ))
                                    :
                                    new ILExpression(val => val.CallMethod(
                                        thisObj2 => thisObj2.CallMethod(
                                            thisObj3 => thisObj3.LoadArgument(0),
                                            typeof(MapperFactory).GetMethod("GetMapper").MakeGenericMethod(
                                                sourceProperty.PropertyType, targetProperty.PropertyType)
                                        ),
                                        typeof(Mapper<,>).MakeGenericType(
                                            sourceProperty.PropertyType,
                                            targetProperty.PropertyType).GetMethod("Invoke"),
                                        typeof(TFrom).IsValueType ?
                                        new ILExpression(valFrom => valFrom.LoadProperty(
                                            thisObj2 => thisObj2.LoadArgumentAddress(1),
                                            sourceProperty
                                        ))
                                        :
                                        new ILExpression(valFrom => valFrom.LoadProperty(
                                            thisObj2 => thisObj2.LoadArgument(1),
                                            sourceProperty
                                        ))
                                    ))
                                );
                            }
                            else
                            {
                                var targetField = MappingHelper.GetFieldInfo(typeof(TTo), property.Name, _ignoreCase, _ignoreUnderscore);
                                if (targetField != null)
                                {
                                    var sourceProperty = property;
                                    gen.StoreField(
                                        typeof(TTo).IsValueType ?
                                        new ILExpression(thisObj => thisObj.LoadLocalVariableAddress(local))
                                        :
                                        new ILExpression(thisObj => thisObj.LoadLocalVariable(local)),
                                        targetField,
                                        sourceProperty.PropertyType == targetField.FieldType ?
                                        typeof(TFrom).IsValueType ?
                                        new ILExpression(valFrom => valFrom.LoadProperty(
                                            thisObj2 => thisObj2.LoadArgumentAddress(1),
                                            sourceProperty
                                        ))
                                        :
                                        new ILExpression(valFrom => valFrom.LoadProperty(
                                            thisObj2 => thisObj2.LoadArgument(1),
                                            sourceProperty
                                        ))
                                        :
                                        new ILExpression(val => val.CallMethod(
                                            thisObj2 => thisObj2.CallMethod(
                                                thisObj3 => thisObj3.LoadArgument(0),
                                                typeof(MapperFactory).GetMethod("GetMapper").MakeGenericMethod(
                                                    sourceProperty.PropertyType, targetField.FieldType)
                                            ),
                                            typeof(Mapper<,>).MakeGenericType(
                                                sourceProperty.PropertyType,
                                                targetField.FieldType).GetMethod("Invoke"),
                                            typeof(TFrom).IsValueType ?
                                            new ILExpression(valFrom => valFrom.LoadProperty(
                                                thisObj2 => thisObj2.LoadArgumentAddress(1),
                                                sourceProperty
                                            ))
                                            :
                                            new ILExpression(valFrom => valFrom.LoadProperty(
                                                thisObj2 => thisObj2.LoadArgument(1),
                                                sourceProperty
                                            ))
                                        ))
                                    );
                                }
                            }
                        }
                        break;
                    case MappingType.Field:
                        foreach (var field in baseType.GetFields())
                        {
                            if (IsIgnoreField(field.Name))
                                continue;

                            var targetProperty = MappingHelper.GetPropertyInfo(typeof(TTo), field.Name, _ignoreCase, _ignoreUnderscore);
                            if (targetProperty != null && targetProperty.CanWrite)
                            {
                                var sourceField = field;
                                gen.StoreProperty(
                                    typeof(TTo).IsValueType ?
                                    new ILExpression(thisObj => thisObj.LoadLocalVariableAddress(local))
                                    :
                                    new ILExpression(thisObj => thisObj.LoadLocalVariable(local)),
                                    targetProperty,
                                    sourceField.FieldType == targetProperty.PropertyType ?
                                    typeof(TFrom).IsValueType ?
                                    new ILExpression(valFrom => valFrom.LoadField(
                                        thisObj2 => thisObj2.LoadArgumentAddress(1),
                                        sourceField
                                    ))
                                    :
                                    new ILExpression(valFrom => valFrom.LoadField(
                                        thisObj2 => thisObj2.LoadArgument(1),
                                        sourceField
                                    ))
                                    :
                                    new ILExpression(val => val.CallMethod(
                                        thisObj2 => thisObj2.CallMethod(
                                            thisObj3 => thisObj3.LoadArgument(0),
                                            typeof(MapperFactory).GetMethod("GetMapper").MakeGenericMethod(
                                                sourceField.FieldType, targetProperty.PropertyType)
                                        ),
                                        typeof(Mapper<,>).MakeGenericType(
                                            sourceField.FieldType,
                                            targetProperty.PropertyType).GetMethod("Invoke"),
                                        typeof(TFrom).IsValueType ?
                                        new ILExpression(valFrom => valFrom.LoadField(
                                            thisObj2 => thisObj2.LoadArgumentAddress(1),
                                            sourceField
                                        ))
                                        :
                                        new ILExpression(valFrom => valFrom.LoadField(
                                            thisObj2 => thisObj2.LoadArgument(1),
                                            sourceField
                                        ))
                                    ))
                                );
                            }
                            else
                            {
                                var targetField = MappingHelper.GetFieldInfo(typeof(TTo), field.Name, _ignoreCase, _ignoreUnderscore);
                                if (targetField != null)
                                {
                                    var sourceField = field;
                                    gen.StoreField(
                                        typeof(TTo).IsValueType ?
                                        new ILExpression(thisObj => thisObj.LoadLocalVariableAddress(local))
                                        :
                                        new ILExpression(thisObj => thisObj.LoadLocalVariable(local)),
                                        targetField,
                                        sourceField.FieldType == targetField.FieldType ?
                                        typeof(TFrom).IsValueType ?
                                        new ILExpression(valFrom => valFrom.LoadField(
                                            thisObj2 => thisObj2.LoadArgumentAddress(1),
                                            sourceField
                                        ))
                                        :
                                        new ILExpression(valFrom => valFrom.LoadField(
                                            thisObj2 => thisObj2.LoadArgument(1),
                                            sourceField
                                        ))
                                        :
                                        new ILExpression(val => val.CallMethod(
                                            thisObj2 => thisObj2.CallMethod(
                                                thisObj3 => thisObj3.LoadArgument(0),
                                                typeof(MapperFactory).GetMethod("GetMapper").MakeGenericMethod(
                                                    sourceField.FieldType, targetField.FieldType)
                                            ),
                                            typeof(Mapper<,>).MakeGenericType(
                                                sourceField.FieldType,
                                                targetField.FieldType).GetMethod("Invoke"),
                                            typeof(TFrom).IsValueType ?
                                            new ILExpression(valFrom => valFrom.LoadField(
                                                thisObj2 => thisObj2.LoadArgumentAddress(1),
                                                sourceField
                                            ))
                                            :
                                            new ILExpression(valFrom => valFrom.LoadField(
                                                thisObj2 => thisObj2.LoadArgument(1),
                                                sourceField
                                            ))
                                        ))
                                    );
                                }
                            }
                        }
                        break;
                    case MappingType.All:
                        foreach (var property in baseType.GetProperties())
                        {
                            if (!property.CanRead || IsIgnoreField(property.Name))
                                continue;

                            var targetProperty = MappingHelper.GetPropertyInfo(typeof(TTo), property.Name, _ignoreCase, _ignoreUnderscore);
                            if (targetProperty != null && targetProperty.CanWrite)
                            {
                                var sourceProperty = property;
                                gen.StoreProperty(
                                    typeof(TTo).IsValueType ?
                                    new ILExpression(thisObj => thisObj.LoadLocalVariableAddress(local))
                                    :
                                    new ILExpression(thisObj => thisObj.LoadLocalVariable(local)),
                                    targetProperty,
                                    sourceProperty.PropertyType == targetProperty.PropertyType ?
                                    typeof(TFrom).IsValueType ?
                                    new ILExpression(valFrom => valFrom.LoadProperty(
                                        thisObj2 => thisObj2.LoadArgumentAddress(1),
                                        sourceProperty
                                    ))
                                    :
                                    new ILExpression(valFrom => valFrom.LoadProperty(
                                        thisObj2 => thisObj2.LoadArgument(1),
                                        sourceProperty
                                    ))
                                    :
                                    new ILExpression(val => val.CallMethod(
                                        thisObj2 => thisObj2.CallMethod(
                                            thisObj3 => thisObj3.LoadArgument(0),
                                            typeof(MapperFactory).GetMethod("GetMapper").MakeGenericMethod(
                                                sourceProperty.PropertyType, targetProperty.PropertyType)
                                        ),
                                        typeof(Mapper<,>).MakeGenericType(
                                            sourceProperty.PropertyType,
                                            targetProperty.PropertyType).GetMethod("Invoke"),
                                        typeof(TFrom).IsValueType ?
                                        new ILExpression(valFrom => valFrom.LoadProperty(
                                            thisObj2 => thisObj2.LoadArgumentAddress(1),
                                            sourceProperty
                                        ))
                                        :
                                        new ILExpression(valFrom => valFrom.LoadProperty(
                                            thisObj2 => thisObj2.LoadArgument(1),
                                            sourceProperty
                                        ))
                                    ))
                                );
                            }
                            else
                            {
                                var targetField = MappingHelper.GetFieldInfo(typeof(TTo), property.Name, _ignoreCase, _ignoreUnderscore);
                                if (targetField != null)
                                {
                                    var sourceProperty = property;
                                    gen.StoreField(
                                        typeof(TTo).IsValueType ?
                                        new ILExpression(thisObj => thisObj.LoadLocalVariableAddress(local))
                                        :
                                        new ILExpression(thisObj => thisObj.LoadLocalVariable(local)),
                                        targetField,
                                        sourceProperty.PropertyType == targetField.FieldType ?
                                        typeof(TFrom).IsValueType ?
                                        new ILExpression(valFrom => valFrom.LoadProperty(
                                            thisObj2 => thisObj2.LoadArgumentAddress(1),
                                            sourceProperty
                                        ))
                                        :
                                        new ILExpression(valFrom => valFrom.LoadProperty(
                                            thisObj2 => thisObj2.LoadArgument(1),
                                            sourceProperty
                                        ))
                                        :
                                        new ILExpression(val => val.CallMethod(
                                            thisObj2 => thisObj2.CallMethod(
                                                thisObj3 => thisObj3.LoadArgument(0),
                                                typeof(MapperFactory).GetMethod("GetMapper").MakeGenericMethod(
                                                    sourceProperty.PropertyType, targetField.FieldType)
                                            ),
                                            typeof(Mapper<,>).MakeGenericType(
                                                sourceProperty.PropertyType,
                                                targetField.FieldType).GetMethod("Invoke"),
                                            typeof(TFrom).IsValueType ?
                                            new ILExpression(valFrom => valFrom.LoadProperty(
                                                thisObj2 => thisObj2.LoadArgumentAddress(1),
                                                sourceProperty
                                            ))
                                            :
                                            new ILExpression(valFrom => valFrom.LoadProperty(
                                                thisObj2 => thisObj2.LoadArgument(1),
                                                sourceProperty
                                            ))
                                        ))
                                    );
                                }
                            }
                        }
                        foreach (var field in baseType.GetFields())
                        {
                            if (IsIgnoreField(field.Name))
                                continue;

                            var targetProperty = MappingHelper.GetPropertyInfo(typeof(TTo), field.Name, _ignoreCase, _ignoreUnderscore);
                            if (targetProperty != null && targetProperty.CanWrite)
                            {
                                var sourceField = field;
                                gen.StoreProperty(
                                    typeof(TTo).IsValueType ?
                                    new ILExpression(thisObj => thisObj.LoadLocalVariableAddress(local))
                                    :
                                    new ILExpression(thisObj => thisObj.LoadLocalVariable(local)),
                                    targetProperty,
                                    sourceField.FieldType == targetProperty.PropertyType ?
                                    typeof(TFrom).IsValueType ?
                                    new ILExpression(valFrom => valFrom.LoadField(
                                        thisObj2 => thisObj2.LoadArgumentAddress(1),
                                        sourceField
                                    ))
                                    :
                                    new ILExpression(valFrom => valFrom.LoadField(
                                        thisObj2 => thisObj2.LoadArgument(1),
                                        sourceField
                                    ))
                                    :
                                    new ILExpression(val => val.CallMethod(
                                        thisObj2 => thisObj2.CallMethod(
                                            thisObj3 => thisObj3.LoadArgument(0),
                                            typeof(MapperFactory).GetMethod("GetMapper").MakeGenericMethod(
                                                sourceField.FieldType, targetProperty.PropertyType)
                                        ),
                                        typeof(Mapper<,>).MakeGenericType(
                                            sourceField.FieldType,
                                            targetProperty.PropertyType).GetMethod("Invoke"),
                                        typeof(TFrom).IsValueType ?
                                        new ILExpression(valFrom => valFrom.LoadField(
                                            thisObj2 => thisObj2.LoadArgumentAddress(1),
                                            sourceField
                                        ))
                                        :
                                        new ILExpression(valFrom => valFrom.LoadField(
                                            thisObj2 => thisObj2.LoadArgument(1),
                                            sourceField
                                        ))
                                    ))
                                );
                            }
                            else
                            {
                                var targetField = MappingHelper.GetFieldInfo(typeof(TTo), field.Name, _ignoreCase, _ignoreUnderscore);
                                if (targetField != null)
                                {
                                    var sourceField = field;
                                    gen.StoreField(
                                        typeof(TTo).IsValueType ?
                                        new ILExpression(thisObj => thisObj.LoadLocalVariableAddress(local))
                                        :
                                        new ILExpression(thisObj => thisObj.LoadLocalVariable(local)),
                                        targetField,
                                        sourceField.FieldType == targetField.FieldType ?
                                        typeof(TFrom).IsValueType ?
                                        new ILExpression(valFrom => valFrom.LoadField(
                                            thisObj2 => thisObj2.LoadArgumentAddress(1),
                                            sourceField
                                        ))
                                        :
                                        new ILExpression(valFrom => valFrom.LoadField(
                                            thisObj2 => thisObj2.LoadArgument(1),
                                            sourceField
                                        ))
                                        :
                                        new ILExpression(val => val.CallMethod(
                                            thisObj2 => thisObj2.CallMethod(
                                                thisObj3 => thisObj3.LoadArgument(0),
                                                typeof(MapperFactory).GetMethod("GetMapper").MakeGenericMethod(
                                                    sourceField.FieldType, targetField.FieldType)
                                            ),
                                            typeof(Mapper<,>).MakeGenericType(
                                                sourceField.FieldType,
                                                targetField.FieldType).GetMethod("Invoke"),
                                            typeof(TFrom).IsValueType ?
                                            new ILExpression(valFrom => valFrom.LoadField(
                                                thisObj2 => thisObj2.LoadArgumentAddress(1),
                                                sourceField
                                            ))
                                            :
                                            new ILExpression(valFrom => valFrom.LoadField(
                                                thisObj2 => thisObj2.LoadArgument(1),
                                                sourceField
                                            ))
                                        ))
                                    );
                                }
                            }
                        }
                        break;
                    default:
                        break;
                }
                

                baseType = baseType.BaseType;
            }
        }

        #endregion

        #endregion
    }
}
