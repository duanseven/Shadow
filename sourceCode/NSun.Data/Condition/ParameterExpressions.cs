using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using NSun.Data.Configuration;

namespace NSun.Data
{
    [Serializable]
    [DataContract(Namespace = "http://nsun-shadow.com")]
    [KnownType("KnownTypes")]
    public sealed class ParameterExpression : ExpressionClip, IParameterExpression
    {
        #region Constructors

        public ParameterExpression(object value, System.Data.DbType dbtype)
        {
            Value = value;
            Sql = "?";
            DataType = dbtype;
        }

        public ParameterExpression(string id, object value, System.Data.DbType dbtype)
            : this(value, dbtype)
        {
            if (string.IsNullOrEmpty(id))
                throw new ArgumentNullException("id");

            ID = id;
        }

        public ParameterExpression(string id, object value, System.Data.DbType dbtype, int size)
            : this(value, dbtype)
        {
            if (string.IsNullOrEmpty(id))
                throw new ArgumentNullException("id");

            ID = id;
            Size = size;
        }

        /// <summary>
        /// return
        /// </summary>
        /// <param name="sprocParamName"></param>
        /// <param name="direction"></param>
        public ParameterExpression(string sprocParamName, SprocParameterDirection direction)
        {
            if (string.IsNullOrEmpty(sprocParamName))
                throw new ArgumentNullException("sprocParamName");

            ID = sprocParamName;
            Sql = "?";
            Direction = direction;
        }

        /// <summary>
        /// input
        /// </summary>
        /// <param name="sprocParamName"></param>
        /// <param name="direction"></param>
        /// <param name="dbtype"></param>
        public ParameterExpression(string sprocParamName, SprocParameterDirection direction, System.Data.DbType dbtype)
        {
            ID = sprocParamName;
            Sql = "?";
            Direction = direction;
            DataType = dbtype;
        }

        /// <summary>
        /// intputoutput output
        /// </summary>
        /// <param name="sprocParamName"></param>
        /// <param name="direction"></param>
        /// <param name="dbtype"></param>
        /// <param name="size"></param>
        public ParameterExpression(string sprocParamName, SprocParameterDirection direction, System.Data.DbType dbtype, int size)
        {
            ID = sprocParamName;
            Sql = "?";
            Direction = direction;
            DataType = dbtype;
            Size = size;
        }


        #endregion

        #region Public Properties

        [DataMember]
        public string ID { get; internal set; }

        [DataMember]
        public object Value { get; set; }

        [DataMember]
        public int? Size { get; set; }

        [DataMember]
        public SprocParameterDirection? Direction { get; internal set; }

        [DataMember]
        public bool? IsUnicode { get; set; }

        object IParameterExpression.Value
        {
            get { return Value; }
            set { Value = value; }
        }

        #endregion

        #region Equals & NotEquals

        public new ParameterEqualsCondition EqualsObject(object value)
        {
            if (value == null)
                return new ParameterEqualsCondition(this, ExpressionOperator.Is, NullExpression.Value);
            return new ParameterEqualsCondition(this, ExpressionOperator.Equals, new ParameterExpression(value, DataType));
        }
        public new ParameterEqualsCondition NotEquals(object value)
        {
            if (value == null)
                return new ParameterEqualsCondition(this, ExpressionOperator.IsNot, NullExpression.Value);

            return new ParameterEqualsCondition(this, ExpressionOperator.NotEquals,
                                               new ParameterExpression(value, DataType));
        }

        public static ParameterEqualsCondition operator ==(ParameterExpression left, object right)
        {
            return left.EqualsObject(right);
        }

        public static ParameterEqualsCondition operator !=(ParameterExpression left, object right)
        {
            return left.NotEquals(right);
        }

        public static ParameterEqualsCondition operator ==(string left, ParameterExpression right)
        {
            return right.EqualsObject(left);
        }

        public static ParameterEqualsCondition operator !=(string left, ParameterExpression right)
        {
            return right.NotEquals(left);
        }

        public static ParameterEqualsCondition operator ==(int left, ParameterExpression right)
        {
            return right.EqualsObject(left);
        }

        public static ParameterEqualsCondition operator !=(int left, ParameterExpression right)
        {
            return right.NotEquals(left);
        }

        public static ParameterEqualsCondition operator ==(double left, ParameterExpression right)
        {
            return right.EqualsObject(left);
        }

        public static ParameterEqualsCondition operator !=(double left, ParameterExpression right)
        {
            return right.NotEquals(left);
        }

        [ComVisible(false)]
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        [ComVisible(false)]
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #endregion

        #region Clone

        public override object Clone()
        {
            var par = new ParameterExpression(Value, DataType) { Sql = Sql };
            if (string.IsNullOrEmpty(ID))
                par.ID = ID;
            if (_childExpressions.Count != 0)
            {
                foreach (var childExpression in _childExpressions)
                {
                    par._childExpressions.Add((IExpression)childExpression.Clone());
                }
            }
            if (Direction.HasValue)
            {
                par.Direction = Direction;
            }
            if (Size.HasValue)
            {
                par.Size = Size;
            }
            return par;
        }

        #endregion

        #region KnownTypes

        static Type[] KnownTypes()
        {
            return KnownTypeRegistry.Instance.KnownTypes;
        }

        #endregion
    }
}
