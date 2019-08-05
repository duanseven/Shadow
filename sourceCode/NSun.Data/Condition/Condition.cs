using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using NSun.Data.Configuration;


namespace NSun.Data
{
    [Serializable]
    [DataContract(Namespace = "http://nsun-shadow.com")]
    [KnownType("KnownTypes")]
    public class Condition : ExpressionClip
    {
        #region Member

        [DataMember]
        private IExpression _left;

        [DataMember]
        private ExpressionOperator _operator;

        [DataMember]
        private IExpression _right;

        [DataMember]
        private List<ConditionAndOr> _linkedConditionAndOrs;

        [DataMember]
        private List<Condition> _linkedConditions;

        [DataMember]
        private bool _isNot;

        #endregion
         
        #region Constructors

        internal Condition(IExpression left, ExpressionOperator op, IExpression right)
            : this()
        {
            _left = left;
            _operator = op;
            _right = right;
        }

        public Condition()
        {
            _linkedConditions = new List<Condition>();
            _linkedConditionAndOrs = new List<ConditionAndOr>();
            DataType = DbType.Boolean;
        }

        #endregion

        #region Properties
        
        public IExpression LeftExpression
        {
            get { return _left; }
        }
         
        public ExpressionOperator Operator
        {
            get { return _operator; }
        }
         
        public IExpression RightExpression
        {
            get { return _right; }
        }
         
        public bool IsNot
        {
            get { return _isNot; }
        }
         
        public List<ConditionAndOr> LinkedConditionAndOrs
        {
            get { return _linkedConditionAndOrs; }
        }
         
        public List<Condition> LinkedConditions
        {
            get { return _linkedConditions; }
        }

        #endregion

        #region Public Methods

        public static bool IsNullOrEmpty(Condition where)
        {            
            return ((object)where) == null || where.ToString().Length == 0;
        }

        public Condition And(Condition condition)
        {
            if (ReferenceEquals(condition, null))
                throw new ArgumentNullException("condition");

            var retCondition = (Condition)Clone();
            retCondition._linkedConditionAndOrs.Add(ConditionAndOr.And);
            retCondition._linkedConditions.Add(condition);

            return retCondition;
        }

        public Condition Space(Condition condition)
        {
            if (ReferenceEquals(condition, null))
                throw new ArgumentNullException("condition");

            var retCondition = (Condition)Clone();
            retCondition._linkedConditionAndOrs.Add(ConditionAndOr.Space);
            retCondition._linkedConditions.Add(condition);

            return retCondition;
        }

        public Condition Or(Condition condition)
        {
            if (ReferenceEquals(condition, null))
                throw new ArgumentNullException("condition");

            var retCondition = (Condition)Clone();
            retCondition._linkedConditionAndOrs.Add(ConditionAndOr.Or);
            retCondition._linkedConditions.Add(condition);

            return retCondition;
        }

        public Condition Not()
        {
            var retCondition = (Condition)Clone();
            retCondition._isNot = !retCondition._isNot;
            return retCondition;
        }

        public override object Clone()
        {
            var rightExpr = (RightExpression == null ? null : (IExpression)RightExpression.Clone());
            var leftExpr = (LeftExpression == null ? null : (IExpression)LeftExpression.Clone());
            var clone = new Condition(leftExpr, Operator, rightExpr) { _isNot = _isNot };
            for (var i = 0; i < _linkedConditions.Count; ++i)
            {
                clone._linkedConditionAndOrs.Add(_linkedConditionAndOrs[i]);
                clone._linkedConditions.Add((Condition)_linkedConditions[i].Clone());
            }
            return clone;
        }

        #endregion

        #region Operators

        public static bool operator true(Condition right)
        {
            return false;
        }

        public static bool operator false(Condition right)
        {
            return false;
        }

        public static Condition operator &(Condition left, Condition right)
        {
            if (ReferenceEquals(left, null))
                return right;

            if (ReferenceEquals(right, null))
                return left;

            return left.And(right);
        }

        public static Condition operator |(Condition left, Condition right)
        {
            if (ReferenceEquals(left, null))
                return right;

            if (ReferenceEquals(right, null))
                return left;

            return left.Or(right);
        }

        public static Condition operator !(Condition right)
        {
            return right.Not();
        }

        public new bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
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
