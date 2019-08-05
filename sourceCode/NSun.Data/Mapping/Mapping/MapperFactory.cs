using System;
using System.Collections.Generic;

namespace NSun.Data.Mapper
{
    /// <summary>
    /// The MapperFactory class is a Emit &amp; delegate based object mapper delegate factory. It dynamically emit dynamic method, cache and return the delegate of the mapper.
    /// </summary>
    public class MapperFactory
    {
        private readonly Dictionary<MapperCacheKey, MapperBuilder> _mapperCache
            = new Dictionary<MapperCacheKey, MapperBuilder>();
        private readonly object _mapperCacheLock = new object();

        #region Non-Public Properties

        internal IDictionary<MapperCacheKey, MapperBuilder> MapperCache
        {
            get { return _mapperCache; }
        }

        #endregion

        #region Public Methods

        public Mapper<TFrom, TTo> GetMapper<TFrom, TTo>(
           bool ignoreCase, bool ignoreUnderscore, MappingType mappingtype, params string[] ignoreFields)
        {
            Initialize();

            var cacheKey = new MapperCacheKey(typeof(TFrom), typeof(TTo));
            MapperBuilder builder;
            if (!_mapperCache.TryGetValue(cacheKey, out builder))
            {
                lock (_mapperCacheLock)
                {
                    if (!_mapperCache.TryGetValue(cacheKey, out builder))
                    {
                        builder = ConfigureMapper<TFrom, TTo>(true, ignoreCase, ignoreUnderscore, mappingtype,
                                                              ignoreFields);
                    }
                }
            }

            var internalMapper = (InternalMapper<TFrom, TTo>)builder.BuildMapper();

            if (internalMapper == null)
                return null;

            return from =>
            {
                var to = default(TTo);
                var type = typeof(TTo);
                if (!MappingHelper.IsNullableType(type)
                    && !type.IsArray
                    && Type.GetTypeCode(type) == TypeCode.Object)
                {
                    to = Create<TTo>();
                }
                internalMapper(this, from, ref to);
                return to;
            };
        }


        public Mapper<TFrom, TTo> GetMapper<TFrom, TTo>(MappingType type, params string[] ignoreFields)
        {
            return GetMapper<TFrom, TTo>(true, true, type, ignoreFields);
        }

        /// <summary>
        /// Gets the mapper.
        /// </summary>
        /// <typeparam name="TFrom">The type of from.</typeparam>
        /// <typeparam name="TTo">The type of to.</typeparam>
        /// <returns></returns>
        public Mapper<TFrom, TTo> GetMapper<TFrom, TTo>()
        {
            return GetMapper<TFrom, TTo>(true, true, MappingType.All);
        }

        /// <summary>
        /// Configures the mapper.
        /// </summary>
        /// <typeparam name="TFrom">The type of from.</typeparam>
        /// <typeparam name="TTo">The type of to.</typeparam>
        /// <returns></returns>
        public MapperBuilder<TFrom, TTo> ConfigureMapper<TFrom, TTo>()
        {
            return ConfigureMapper<TFrom, TTo>(true, true, true, MappingType.All);
        }

        /// <summary>
        /// 除去不需要参数
        /// </summary>
        /// <typeparam name="TFrom"></typeparam>
        /// <typeparam name="TTo"></typeparam>
        /// <param name="ignoreFields"></param>
        /// <returns></returns>
        public MapperBuilder<TFrom, TTo> ConfigureMapper<TFrom, TTo>(MappingType type,params string[] ignoreFields)
        {
            return ConfigureMapper<TFrom, TTo>(true, true, true, type, ignoreFields);
        }

        /// <summary>
        /// Configures the mapper.
        /// </summary>
        /// <typeparam name="TFrom">The type of from.</typeparam>
        /// <typeparam name="TTo">The type of to.</typeparam>
        /// <param name="autoMap">if set to <c>true</c> [auto map].</param>
        /// <param name="ignoreCase">if set to <c>true</c> [ignore case].</param>
        /// <param name="ignoreUnderscore">if set to <c>true</c> [ignore underscore].</param>
        /// <param name="ignoreFields">The ignore fields.</param>
        /// <returns></returns>
        public MapperBuilder<TFrom, TTo> ConfigureMapper<TFrom, TTo>(
            bool autoMap, bool ignoreCase, bool ignoreUnderscore,MappingType type, params string[] ignoreFields)
        {
            var builder = new MapperBuilder<TFrom, TTo>(autoMap, ignoreCase, ignoreUnderscore, type, ignoreFields);
            lock (_mapperCacheLock)
            {
                _mapperCache[builder.CacheKey] = builder;
            }
            return builder;
        }

        #endregion

        #region Non-Public Methods

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        protected virtual void Initialize()
        {
        }

        /// <summary>
        /// Override this method in derived class to integrate with 3rd party object factory
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        protected virtual T Create<T>()
        {
            return Activator.CreateInstance<T>();
        }

        #endregion
    }
}
