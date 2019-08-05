using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading; 

namespace NSun.Data.Common
{
    /// <summary>
    /// A thread safe Lru generic IDictionary implementation
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Lru")]
    public class LruDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        #region Non-Public Fields

        private readonly int _capacity;
        
        private readonly Dictionary<TKey, Node> _data;
        private Node _head;
        private Node _tail;
        private readonly ReaderWriterLock _lock = new ReaderWriterLock();
        private readonly int _lockTimeout = 30000;

        #endregion

        #region Non-Public Properties

        /// <summary>
        /// Gets the lock.
        /// </summary>
        /// <value>The lock.</value>
        protected ReaderWriterLock Lock
        {
            get
            {
                return _lock;
            }
        }

        /// <summary>
        /// Gets the lock timeout.
        /// </summary>
        /// <value>The lock timeout.</value>
        protected int LockTimeout
        {
            get
            {
                return _lockTimeout;
            }
        }

        #endregion

        #region Private Nested Classes

        private class Node
        {
            #region Private Fields

            private readonly TKey _key;
            private Node _prev;
            private Node _next;
            private readonly TValue _value;

            #endregion

            #region Public Properties

            public TKey Key
            {
                get
                {
                    return _key;
                }
            }

            public Node Prev
            {
                get
                {
                    return _prev;
                }
                set
                {
                    _prev = value;
                }
            }

            public Node Next
            {
                get
                {
                    return _next;
                }
                set
                {
                    _next = value;
                }
            }

            public TValue Value
            {
                get
                {
                    return _value;
                }
            }

            #endregion

            #region Constructors

            public Node(TKey key, TValue value)
            {
                _key = key;
                _value = value;
            }

            #endregion
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LruDictionary&lt;TKey, TValue&gt;"/> class.
        /// </summary>
        /// <param name="capacity">The capacity.</param>
        public LruDictionary(int capacity)
        {
            if (capacity <= 0)
                throw new ArgumentException("capacity MUST > 0");

            this._capacity = capacity;
            this._data = new Dictionary<TKey, Node>();
        }

        #endregion

        #region Private Methods

        private void InsertNode(Node node)
        {
            node.Next = _head;
            node.Prev = null;
            if (_head != null)
                _head.Prev = node;
            _head = node;
            if (_tail == null)
                _tail = node;
        }

        private void DeleteNode(Node node)
        {
            if (node.Prev != null)
                node.Prev.Next = node.Next;
            else
                _head = node.Next;
            if (node.Next != null)
                node.Next.Prev = node.Prev;
            else
                _tail = node.Prev;
        }

        private bool RemoveLru()
        {
            return RWLock.GetWriteLock(_lock, _lockTimeout, delegate
            {
                if (_tail == null)
                    return false;
                _data.Remove(_tail.Key);
                DeleteNode(_tail);
                return true;
            });
        }

        /// <summary>
        /// Internals the remove.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        protected bool InternalRemove(TKey key)
        {
            Node node;
            if (_data.TryGetValue(key, out node))
            {
                _data.Remove(key);
                DeleteNode(node);
                return true;
            }
            return false;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Adds an element with the provided key and value to the <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </summary>
        /// <param name="key">The object to use as the key of the element to add.</param>
        /// <param name="value">The object to use as the value of the element to add.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// 	<paramref name="key"/> is null.
        /// </exception>
        /// <exception cref="T:System.ArgumentException">
        /// An element with the same key already exists in the <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </exception>
        /// <exception cref="T:System.NotSupportedException">
        /// The <see cref="T:System.Collections.Generic.IDictionary`2"/> is read-only.
        /// </exception>
        public void Add(TKey key, TValue value)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            RWLock.GetWriteLock(_lock, _lockTimeout, delegate
            {
                // Throw out old one if reusing a key.
                if (_data.ContainsKey(key))
                    Remove(key);
                // Keep throwing out old items to make space. Stop if empty.
                while (_data.Count >= _capacity)
                {
                    if (!RemoveLru())
                        break;
                }
                if (_data.Count >= _capacity)
                    // Just not enough room.
                    return false;
                Node node = new Node(key, value);
                _data.Add(key, node);
                InsertNode(node);
                return true;
            });
        }

        /// <summary>
        /// Removes the element with the specified key from the <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </summary>
        /// <param name="key">The key of the element to remove.</param>
        /// <returns>
        /// true if the element is successfully removed; otherwise, false.  This method also returns false if <paramref name="key"/> was not found in the original <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// 	<paramref name="key"/> is null.
        /// </exception>
        /// <exception cref="T:System.NotSupportedException">
        /// The <see cref="T:System.Collections.Generic.IDictionary`2"/> is read-only.
        /// </exception>
        public bool Remove(TKey key)
        {
            return RWLock.GetWriteLock(_lock, _lockTimeout, delegate
            {
                return InternalRemove(key);
            });
        }

        /// <summary>
        /// Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <exception cref="T:System.NotSupportedException">
        /// The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.
        /// </exception>
        public void Clear()
        {
            RWLock.GetWriteLock(_lock, _lockTimeout, delegate
            {
                _data.Clear();
                _head = null;
                _tail = null;
                return true;
            });
        }

        /// <summary>
        /// Determines whether the <see cref="T:System.Collections.Generic.IDictionary`2"/> contains an element with the specified key.
        /// </summary>
        /// <param name="key">The key to locate in the <see cref="T:System.Collections.Generic.IDictionary`2"/>.</param>
        /// <returns>
        /// true if the <see cref="T:System.Collections.Generic.IDictionary`2"/> contains an element with the key; otherwise, false.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// 	<paramref name="key"/> is null.
        /// </exception>
        public bool ContainsKey(TKey key)
        {
            return _data.ContainsKey(key);
        }

        /// <summary>
        /// Gets or sets the TValue with the specified key.
        /// </summary>
        /// <value></value>
        public TValue this[TKey key]
        {
            get
            {
                return RWLock.GetReadLock(_lock, _lockTimeout, delegate
                {
                    Node node;
                    if (_data.TryGetValue(key, out node))
                    {
                        DeleteNode(node);
                        InsertNode(node);
                        return node.Value;
                    }
                    return default(TValue);
                });
            }
            set
            {
                Add(key, value);
            }
        }

        /// <summary>
        /// Gets an <see cref="T:System.Collections.Generic.ICollection`1"/> containing the keys of the <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// An <see cref="T:System.Collections.Generic.ICollection`1"/> containing the keys of the object that implements <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </returns>
        public ICollection<TKey> Keys
        {
            get
            {
                return _data.Keys;
            }
        }

        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key whose value to get.</param>
        /// <param name="value">When this method returns, the value associated with the specified key, if the key is found; otherwise, the default value for the type of the <paramref name="value"/> parameter. This parameter is passed uninitialized.</param>
        /// <returns>
        /// true if the object that implements <see cref="T:System.Collections.Generic.IDictionary`2"/> contains an element with the specified key; otherwise, false.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// 	<paramref name="key"/> is null.
        /// </exception>
        public bool TryGetValue(TKey key, out TValue value)
        {
            value = this[key];
            return (value != null);
        }

        /// <summary>
        /// Gets an <see cref="T:System.Collections.Generic.ICollection`1"/> containing the values in the <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// An <see cref="T:System.Collections.Generic.ICollection`1"/> containing the values in the object that implements <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </returns>
        public ICollection<TValue> Values
        {
            get
            {
                List<TValue> retList = new List<TValue>();
                foreach (Node node in _data.Values)
                    retList.Add(node.Value);
                return retList;
            }
        }

        /// <summary>
        /// Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
        /// <exception cref="T:System.NotSupportedException">
        /// The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.
        /// </exception>
        public void Add(KeyValuePair<TKey, TValue> item)
        {
            Add(item.Key, item.Value);
        }

        /// <summary>
        /// Determines whether the <see cref="T:System.Collections.Generic.ICollection`1"/> contains a specific value.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
        /// <returns>
        /// true if <paramref name="item"/> is found in the <see cref="T:System.Collections.Generic.ICollection`1"/>; otherwise, false.
        /// </returns>
        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return _data.ContainsKey(item.Key);
        }

        /// <summary>
        /// Copies the elements of the <see cref="T:System.Collections.Generic.ICollection`1"/> to an <see cref="T:System.Array"/>, starting at a particular <see cref="T:System.Array"/> index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="T:System.Array"/> that is the destination of the elements copied from <see cref="T:System.Collections.Generic.ICollection`1"/>. The <see cref="T:System.Array"/> must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// 	<paramref name="array"/> is null.
        /// </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// 	<paramref name="arrayIndex"/> is less than 0.
        /// </exception>
        /// <exception cref="T:System.ArgumentException">
        /// 	<paramref name="array"/> is multidimensional.
        /// -or-
        /// <paramref name="arrayIndex"/> is equal to or greater than the length of <paramref name="array"/>.
        /// -or-
        /// The number of elements in the source <see cref="T:System.Collections.Generic.ICollection`1"/> is greater than the available space from <paramref name="arrayIndex"/> to the end of the destination <paramref name="array"/>.
        /// -or-
        /// Type <paramref name="T"/> cannot be cast automatically to the type of the destination <paramref name="array"/>.
        /// </exception>
        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            if (array == null || arrayIndex >= _data.Count - 1)
                return;

            if (arrayIndex < 0)
                arrayIndex = 0;

            IEnumerator<KeyValuePair<TKey, TValue>> en = this.GetEnumerator();
            int i = 0;
            while (i < arrayIndex)
                en.MoveNext();
            for (int j = 0; i < _data.Count && j < array.Length; ++i, ++j)
            {
                en.MoveNext();
                array[j] = en.Current;
            }
        }

        /// <summary>
        /// Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// The number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </returns>
        public int Count
        {
            get
            {
                return _data.Count;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.
        /// </summary>
        /// <value></value>
        /// <returns>true if the <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only; otherwise, false.
        /// </returns>
        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
        /// <returns>
        /// true if <paramref name="item"/> was successfully removed from the <see cref="T:System.Collections.Generic.ICollection`1"/>; otherwise, false. This method also returns false if <paramref name="item"/> is not found in the original <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </returns>
        /// <exception cref="T:System.NotSupportedException">
        /// The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.
        /// </exception>
        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return Remove(item.Key);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            Dictionary<TKey, Node>.Enumerator en = _data.GetEnumerator();
            while (en.MoveNext())
                yield return new KeyValuePair<TKey, TValue>(en.Current.Key, en.Current.Value.Value);
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            Dictionary<TKey, Node>.Enumerator en = _data.GetEnumerator();
            while (en.MoveNext())
                yield return new KeyValuePair<TKey, TValue>(en.Current.Key, en.Current.Value.Value);
        }

        #endregion
    }

    /// <summary>
    /// The LruDependingDictionary class is an enhanced version of LruDictionary, providing additional caching dependency ability.
    /// </summary>
    /// <typeparam name="TKey">The type of cached item key.</typeparam>
    /// <typeparam name="TValue">The type of cached item value.</typeparam>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Lru")]
    public class LruDependingDictionary<TKey, TValue> 
        : LruDictionary<DependingKey<TKey>, TValue>
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LruDependingDictionary&lt;TKey, TValue&gt;"/> class.
        /// </summary>
        /// <param name="capacity">The capacity.</param>
        public LruDependingDictionary(int capacity)
            : base(capacity)
        {
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the TValue with the specified key.
        /// </summary>
        /// <value></value>
        public TValue this[TKey key]
        {
            get
            {
                return this[new DependingKey<TKey>(key)];
            }
        }

        /// <summary>
        /// Removes the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        public void Remove(TKey key)
        {
            Remove(new DependingKey<TKey>(key));
        }

        /// <summary>
        /// Notifies the dependency changed.
        /// </summary>
        /// <param name="dependency">The dependency.</param>
        /// <param name="ignoreCase">if set to <c>true</c> [ignore case].</param>
        /// <param name="partiallyMatch">if set to <c>true</c> [partially match].</param>
        public void NotifyDependencyChanged(string dependency, bool ignoreCase, bool partiallyMatch)
        {
            List<DependingKey<TKey>> list = new List<DependingKey<TKey>>();
            IEnumerator<DependingKey<TKey>> en = this.Keys.GetEnumerator();
            RWLock.GetWriteLock(Lock, LockTimeout, delegate
            {
                while (en.MoveNext())
                    if (en.Current.Depends(dependency, ignoreCase, partiallyMatch))
                        list.Add(en.Current);
                if (list.Count > 0)
                    for (int i = 0; i < list.Count; ++i)
                        this.InternalRemove(list[i]);
                return true;
            });
        }

        /// <summary>
        /// Notifies the dependency changed.
        /// </summary>
        /// <param name="dependency">The dependency.</param>
        public void NotifyDependencyChanged(string dependency)
        {
            NotifyDependencyChanged(dependency, false, false);
        }

        #endregion
    }
}