using System.Collections.Generic;
using System.Threading;

namespace NSun.Data.Common
{
    /// <summary>
    /// A abstract simple registry with readerwriter lock
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    public abstract class Registry<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>
    {
        private readonly Dictionary<TKey, TValue> _dict;
        private readonly ReaderWriterLock _dictLock;

        internal Registry()
        {
            _dict = new Dictionary<TKey, TValue>();
            _dictLock = new ReaderWriterLock();
        }

        /// <summary>
        /// Gets the TValue with the specified key.
        /// </summary>
        /// <value></value>
        public virtual TValue this[TKey key]
        {
            get
            {
                return RWLock.GetReadLock(
                    _dictLock,
                    RWLock.DEFAULT_RWLOCK_TIMEOUT,
                    () =>
                    {
                        TValue value;
                        if (_dict.TryGetValue(key, out value))
                            return value;
                        return default(TValue);
                    });
            }
        }

        /// <summary>
        /// Adds an item.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public virtual bool AddItem(TKey key, TValue value)
        {
            if (!_dict.ContainsKey(key))
            {
                return RWLock.GetWriteLock(
                    _dictLock,
                    RWLock.DEFAULT_RWLOCK_TIMEOUT,
                    () =>
                    {
                        if (!_dict.ContainsKey(key))
                        {
                            _dict.Add(key, value);
                            return true;
                        }
                        return false;
                    }
                    );
            }
            return false;
        }

        public virtual bool RemoveItem(TKey key)
        {
            if (!_dict.ContainsKey(key))
            {
                return RWLock.GetWriteLock(
                    _dictLock,
                    RWLock.DEFAULT_RWLOCK_TIMEOUT,
                    () =>
                    {
                        if (!_dict.ContainsKey(key))
                        {
                            _dict.Remove(key);
                            return true;
                        }
                        return false;
                    }
                    );
            }
            return false;
        }

        public virtual TValue GetItem(TKey key)
        {
            if (!_dict.ContainsKey(key))
            {
                return RWLock.GetWriteLock(
                    _dictLock,
                    RWLock.DEFAULT_RWLOCK_TIMEOUT,
                    () =>
                    {
                        if (!_dict.ContainsKey(key))
                        {
                            TValue tag;
                            _dict.TryGetValue(key, out tag);
                            return tag;
                        }
                        return default(TValue);
                    }
                    );
            }
            return default(TValue);
        }

        #region IEnumerable<KeyValuePair<TKey,TValue>> Members

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return _dict.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _dict.GetEnumerator();
        }

        #endregion
    }
}