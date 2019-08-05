using System;
using System.Collections.Generic;
using System.Text;

namespace NSun.Data.Common
{
    /// <summary>
    /// A depending key is a key wrapper attached with dependencies
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    public sealed class DependingKey<TKey>
    {
        #region Private Fields

        private readonly TKey _keyValue;
        private readonly string[] _dependencies;

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the key value.
        /// </summary>
        /// <value>The key value.</value>
        public TKey KeyValue
        {
            get
            {
                return _keyValue;
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DependingKey&lt;TKey&gt;"/> class.
        /// </summary>
        /// <param name="keyValue">The key value.</param>
        /// <param name="dependencies">The dependencies.</param>
        public DependingKey(TKey keyValue, params string[] dependencies)
        {
            _keyValue = keyValue;
            _dependencies = dependencies;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(DependingKey<TKey> left, DependingKey<TKey> right)
        {
            return object.Equals(left, right);
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(DependingKey<TKey> left, DependingKey<TKey> right)
        {
            return !object.Equals(left, right);
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <param name="obj">The <see cref="T:System.Object"/> to compare with the current <see cref="T:System.Object"/>.</param>
        /// <returns>
        /// true if the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>; otherwise, false.
        /// </returns>
        /// <exception cref="T:System.NullReferenceException">
        /// The <paramref name="obj"/> parameter is null.
        /// </exception>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;

            DependingKey<TKey> dpendingKey = obj as DependingKey<TKey>;
            if (dpendingKey != null)
                return object.Equals(this.KeyValue, dpendingKey.KeyValue);

            return false;
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"/>.
        /// </returns>
        public override int GetHashCode()
        {
            return this.KeyValue.GetHashCode();
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return this.KeyValue.ToString();
        }

        /// <summary>
        /// Determine whether the key depends on specified dependency.
        /// </summary>
        /// <param name="dependency">The dependency.</param>
        /// <param name="ignoreCase">if set to <c>true</c> [ignore case].</param>
        /// <param name="partiallyMatch">if set to <c>true</c> [partially match].</param>
        /// <returns></returns>
        public bool Depends(string dependency, bool ignoreCase, bool partiallyMatch)
        {
            if (_dependencies != null)
            {
                for (int i = 0; i < _dependencies.Length; ++i)
                {
                    if (string.Compare(_dependencies[i], dependency, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal) == 0)
                        return true;

                    if (partiallyMatch && dependency != null && _dependencies[i] != null)
                    {
                        if (_dependencies[i].IndexOf(dependency, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal) >= 0)
                            return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Determine whether the key depends on specified dependency.
        /// </summary>
        /// <param name="dependency">The dependency.</param>
        /// <returns></returns>
        public bool Depends(string dependency)
        {
            return Depends(dependency, false, false);
        }

        #endregion
    }
}
