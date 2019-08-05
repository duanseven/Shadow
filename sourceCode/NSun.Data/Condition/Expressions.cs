using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using System.Data;
using NSun.Data.Configuration;


namespace NSun.Data
{
    [Serializable]
    [DataContract(Namespace = "http://nsun-shadow.com")]
    [KnownType("KnownTypes")]
    public abstract class Expression : IExpression
    {
        #region KnownTypes

        static Type[] KnownTypes()
        {
            return KnownTypeRegistry.Instance.KnownTypes;
        }

        #endregion

        #region Member

        [DataMember]
        protected List<IExpression> _childExpressions;

        [DataMember]
        public string Sql
        {
            get { return _sql.ToString(); }
            internal set
            {
                _sql = new StringBuilder(value);
            }
        }

        public DbType DataType { get; set; }

        protected StringBuilder _sql;

        ICollection<IExpression> IExpression.ChildExpressions
        {
            get { return _childExpressions; }
        }

        public List<IExpression> ChildExpressions
        {
            get { return _childExpressions; }
        }

        #endregion

        #region Constructors

        protected Expression()
        {
            _childExpressions = new List<IExpression>();
            _sql = new StringBuilder();
        }

        protected Expression(string sql, IEnumerable<IExpression> childExpressions)
            : this()
        {
            if (string.IsNullOrEmpty(sql))
                throw new ArgumentNullException("sql");

            Sql = sql;
            if (childExpressions != null)
            {
                foreach (IExpression expr in childExpressions)
                    _childExpressions.Add(expr);
            }
        }

        protected Expression(string sql, DbType dbtype, IEnumerable<IExpression> childExpressions)
            : this(sql, childExpressions)
        {
            DataType = dbtype;
        }

        #endregion

        #region ICloneable Members

        public virtual object Clone()
        {
            var clone = (Expression)CreateInstance();
            clone.Sql = Sql;
            foreach (var expr in _childExpressions)
            {
                clone._childExpressions.Add((IExpression)expr.Clone());
            }
            clone.DataType = DataType;
            return clone;
        }

        protected abstract object CreateInstance();

        #endregion

        #region Public Methods

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            var objExpr = obj as Expression;
            if (objExpr == null)
                return false;

            if (Sql == objExpr.Sql && _childExpressions.Count == objExpr._childExpressions.Count)
            {
                return !_childExpressions.Where((t, i) => !t.Equals(objExpr._childExpressions[i])).Any();
            }
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return _sql.ToString();
        }

        #endregion
    }

    //[CollectionDataContract(Namespace = "http://nsun-shadow.com")]
    [Serializable]
    [DataContract(Namespace = "http://nsun-shadow.com")]
    [KnownType("KnownTypes")]
    public sealed class ExpressionCollection : IExpression, ICollection<IExpression>
    {
        #region KnownTypes

        static Type[] KnownTypes()
        {
            return KnownTypeRegistry.Instance.KnownTypes;
        }

        #endregion

        #region Member

        private readonly List<IExpression> _list = new List<IExpression>();

        #endregion

        #region IExpression Members

        public string Sql
        {
            get
            {
                if (ChildExpressions == null || ChildExpressions.Count == 0)
                    return "NULL";

                var sb = new StringBuilder();
                sb.Append("(");
                var separate = string.Empty;
                for (var i = 0; i < _list.Count; ++i)
                {
                    sb.Append(separate);
                    sb.Append("?");
                    separate = ", ";
                }
                sb.Append(")");
                return sb.ToString();
            }
        }

        public ICollection<IExpression> ChildExpressions
        {
            get { return _list; }
        }

        #endregion

        #region ICollection<IExpression> Members

        public void Add(IExpression item)
        {
            _list.Add(item);
        }

        public void Clear()
        {
            _list.Clear();
        }

        public bool Contains(IExpression item)
        {
            return _list.Contains(item);
        }

        public void CopyTo(IExpression[] array, int arrayIndex)
        {
            _list.CopyTo(array, arrayIndex);
        }

        int ICollection<IExpression>.Count
        {
            get { return _list.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(IExpression item)
        {
            return _list.Remove(item);
        }

        #endregion

        #region IEnumerable<IExpression> Members

        public IEnumerator<IExpression> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        #endregion

        #region ICloneable Members

        [ComVisible(false)]
        public object Clone()
        {
            var clone = new ExpressionCollection();
            foreach (var expr in _list)
            {
                clone._list.Add((IExpression)expr.Clone());
            }
            return clone;
        }

        #endregion
    }

    [Serializable]
    [DataContract(Namespace = "http://nsun-shadow.com")]
    [KnownType("KnownTypes")]
    public sealed class NullExpression : IExpression
    {
        #region KnownTypes

        static Type[] KnownTypes()
        {
            return KnownTypeRegistry.Instance.KnownTypes;
        }

        #endregion

        #region Static Member

        public static readonly NullExpression Value = new NullExpression();

        #endregion

        #region Constructors

        private NullExpression() { }

        #endregion

        #region IExpression Members

        public string Sql
        {
            get { return "NULL"; }
        }

        public ICollection<IExpression> ChildExpressions
        {
            get { return null; }
        }

        public string ToCacheableSql()
        {
            return Sql;
        }

        #endregion

        #region ICloneable Members

        public object Clone()
        {
            return this;
        }

        #endregion
    }
}
