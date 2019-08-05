using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using NSun.Data.Configuration;


namespace NSun.Data
{
    [Serializable]
    [DataContract(Namespace = "http://nsun-shadow.com")]
    [KnownType("KnownTypes")]
    public class DeleteSqlSection : QueryCriteria, IConditionSection
    {
        #region Property & Field

        [DataMember]
        private Condition _condition;

        [DataMember]
        private FromClip _from;

        [DataMember]
        private Condition _fromCondition;

        public Condition ConditionWhere
        {
            get { return _condition; }
        }
         
        public Condition FromCondition
        {
            get { return _fromCondition; }
            internal set { _fromCondition = value; }
        }

        protected internal FromClip From
        {
            get { return _from; }
            internal set { _from = value; }
        }

        #endregion 

        #region Construction

        public DeleteSqlSection(string tablename)
            : base(tablename)
        {
            _queryType = QueryType.Delete;
            _condition = new Condition();

            //after
            _fromCondition = new Condition();
            _from = new FromClip(tablename);
        }

        internal DeleteSqlSection(Database db, string tablename)
            : this(tablename)
        {
            Db = db;
        }

        public DeleteSqlSection(IQueryTable table)
            : this(null, table.GetTableName()) { }

        #endregion       

        #region Public Method

        public DeleteSqlSection Where(Condition condition)
        {
            if (ReferenceEquals(condition, null))
                throw new ArgumentNullException("condition");
            _condition = _condition.And(condition);
            OnChanged();
            return this;
        }

        #region Join

        public DeleteSqlSection Join(IQueryTable joinTable, Condition joinOnWhere)
        {
            return Join(joinTable, joinTable.GetTableName(), joinOnWhere);
        }

        public DeleteSqlSection Join(IQueryTable joinTable, string joinTableAliasName, Condition joinOnWhere)
        {
            From.Join(joinTable.GetTableName(), joinTableAliasName, joinOnWhere);
            JoinCondition(joinOnWhere);
            return this;
        }

        private void JoinCondition(Condition joinOnWhere)
        {
            _fromCondition.LinkedConditions.Add(joinOnWhere);
            _fromCondition.LinkedConditionAndOrs.Add(ConditionAndOr.And);
        }

        public override string ToString()
        {
            var retSql = new StringBuilder(" ");
            if (From != null)
            {
                retSql.Append(From.ToString());
            }
            return retSql.ToString();
        }

        public override object Clone()
        {
            var clone = new DeleteSqlSection(Db, TableName);
            clone._condition = (Condition)_condition.Clone();
            clone.IdentyColumnName = IdentyColumnName;
            clone.IdentyColumnIsNumber = IdentyColumnIsNumber;
            return clone;
        }

        #endregion

        #endregion 

        #region KnownTypes

        static Type[] KnownTypes()
        {
            return KnownTypeRegistry.Instance.KnownTypes;
        }

        #endregion

    }
}
