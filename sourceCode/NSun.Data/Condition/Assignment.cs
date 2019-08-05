using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using NSun.Data.Configuration;


namespace NSun.Data
{
    [Serializable]
    [DataContract(Namespace = "http://nsun-shadow.com")]
    [KnownType("KnownTypes")]
    public sealed class Assignment
    {
        #region Member

        [DataMember]
        private IColumn _left;

        [DataMember]
        private IExpression _right;

        #endregion

        #region Constructors

        internal Assignment(IColumn left, IExpression right)
        {
            if (ReferenceEquals(left, null))
                throw new ArgumentNullException("left");

            if (ReferenceEquals(right, null))
                right = NullExpression.Value;

            _left = left;
            _right = right;
        }

        #endregion

        #region Properties

        public IColumn LeftColumn
        {
            get { return _left; }
        }

        public IExpression RightExpression
        {
            get { return _right; }
        }

        #endregion

        #region KnownTypes

        /// <summary>
        /// Knowns the types.
        /// </summary>
        /// <returns></returns>
        static Type[] KnownTypes()
        {
            return KnownTypeRegistry.Instance.KnownTypes;
        }

        #endregion
    }
}
