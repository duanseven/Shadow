using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using NSun.Data.Configuration;


namespace NSun.Data
{
    [Serializable]
    [DataContract(Namespace = "http://nsun-shadow.com")]
    [KnownType("KnownTypes")]
    public class UpdateSqlSection : QueryCriteria, ICudSection, IConditionSection
    {
        #region Propertys & Fields

        [DataMember]
        private readonly List<Assignment> _assignments;

        [DataMember]
        private Condition _condition;

        [DataMember]
        private FromClip _from;

        [DataMember]
        private Condition _fromCondition;

        public ReadOnlyCollection<Assignment> Assignments
        {
            get { return new ReadOnlyCollection<Assignment>(_assignments); }
        }

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

        public UpdateSqlSection(string tablename)
            : base(tablename)
        {
            _assignments = new List<Assignment>();

            _queryType = QueryType.Update;
            _condition = new Condition();
            //after
            _fromCondition = new Condition();
            _from = new FromClip(tablename);
        }

        internal UpdateSqlSection(Database db, string tablename)
            : this(tablename)
        {
            Db = db;
        }

        public UpdateSqlSection(IQueryTable table)
            : this(null, table.GetTableName()) { }

        #endregion

        #region Public Methods

        public UpdateSqlSection AddColumn(QueryColumn column, object value)
        {
            AddAssignment(new Assignment(column, new ParameterExpression(value, column.DataType)));
            return this;
        }

        public override object Clone()
        {
            var clone = new UpdateSqlSection(Db, TableName);
            foreach (var assignment in Assignments)
            {
                clone._assignments.Add(assignment);
            }
            clone._condition = (Condition)_condition.Clone();
            clone.IdentyColumnName = IdentyColumnName;
            clone.IdentyColumnIsNumber = IdentyColumnIsNumber;
            return clone;
        }

        public UpdateSqlSection Where(Condition condition)
        {
            _condition = _condition.And(condition);
            OnChanged();
            return this;
        }

        #endregion

        #region No-Public Methods

        internal void AddAssignment(Assignment ag)
        {
            _assignments.Add(ag);
        }

        #endregion

        #region Join

        public UpdateSqlSection Join(IQueryTable joinTable, Condition joinOnWhere)
        {
            return Join(joinTable, joinTable.GetTableName(), joinOnWhere);
        }

        public UpdateSqlSection Join(IQueryTable joinTable, string joinTableAliasName, Condition joinOnWhere)
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
            StringBuilder retSql = new StringBuilder(" ");
            if (From != null)
            {
                retSql.Append(From.ToString());
            }
            return retSql.ToString();
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
