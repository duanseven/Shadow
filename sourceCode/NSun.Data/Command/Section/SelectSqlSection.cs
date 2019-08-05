using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using NSun.Data.Configuration;


namespace NSun.Data
{
    [Serializable]
    [DataContract(Namespace = "http://nsun-shadow.com")]
    [KnownType("KnownTypes")]
    public class SelectSqlSection : QueryCriteria, IConditionSection, ISelectSection
    {
        #region Member

        /// <summary>
        /// return columns
        /// </summary>
        [DataMember]
        private readonly List<ExpressionClip> _resultColumns;

        /// <summary>
        /// distinct
        /// </summary>
        [DataMember]
        private bool _isDistinct;

        /// <summary>
        /// top num
        /// </summary>
        [DataMember]
        private int _maxResults;

        /// <summary>
        /// skip num
        /// </summary>
        [DataMember]
        private int _skipResults; 

        /// <summary>
        /// orderby
        /// </summary>
        [DataMember]
        private Dictionary<ExpressionClip, bool> _sortBys;

        /// <summary>
        /// where
        /// </summary> 
        [DataMember]
        private Condition _condition;

        /// <summary>
        /// havingWhere
        /// </summary> 
        [DataMember]
        private Condition _havingcondition;

        /// <summary>
        /// Group By
        /// </summary>
        [DataMember]
        private readonly List<IColumn> _groups;

        /// <summary>
        /// join from
        /// </summary>
        [DataMember]
        private FromClip _from;

        /// <summary>
        /// join where
        /// </summary> 
        [DataMember]
        private Condition _fromCondition;

        internal IQueryTable Table { get; set; }

        #endregion

        #region Construction

        protected internal SelectSqlSection()
        {            
            _resultColumns = new List<ExpressionClip>();
            _sortBys = new Dictionary<ExpressionClip, bool>();
            _queryType = QueryType.Select;
            _groups = new List<IColumn>();
            _queryType = QueryType.Select;
            _condition = new Condition();
            _havingcondition = new Condition();
            _fromCondition = new Condition();
        }

        internal SelectSqlSection(Database db, string tableName)
            : this()
        {
            Db = db;
            TableName = tableName;
            _from = new FromClip(TableName);
        }
       
        internal SelectSqlSection(Database db, IQueryTable table)
            : this(db, table.GetTableName())
        {
            if (table is CustomQueryTable)
            {
                var tab = table as CustomQueryTable;
                if (table is CustomWithQueryTable)
                {
                    TableName = tab.AsName;
                    _from = new FromClip(tab.AsName);
                }
                Table = table;
                _from.isCostomTable = true;
            }
        }

        public SelectSqlSection(string tablename)
            : this(null, tablename)
        {
        }

        public SelectSqlSection(IQueryTable table)
            : this(null, table) { }

        #endregion

        #region Properties

        public Condition ConditionWhere
        {
            get { return _condition; }
        }

        public Condition HavingConditionWhere
        {
            get { return _havingcondition; }
        }

        public Condition FromCondition
        {
            get { return _fromCondition; }
            internal set { _fromCondition = value; }
        }
         
        /// <summary>
        /// Join From Table
        /// </summary>       
        protected internal FromClip From
        {
            get { return _from; }
            internal set { _from = value; }
        }

        public ReadOnlyCollection<ExpressionClip> ResultColumns
        {
            get { return new ReadOnlyCollection<ExpressionClip>(_resultColumns); }
        }

        public bool IsDistinct
        {
            get { return _isDistinct; }
            internal set { _isDistinct = value; }
        }

        public int MaxResults
        {
            get { return _maxResults; }
            internal set { _maxResults = value; }
        }

        public int SkipResults
        {
            get { return _skipResults; }
            internal set { _skipResults = value; }
        }

        public IDictionary<ExpressionClip, bool> SortBys
        {
            get { return _sortBys; }
            internal set
            {
                _sortBys = (Dictionary<ExpressionClip, bool>)value;
            }
        }

        public List<IColumn> Groups { get { return _groups; } }

        #endregion

        #region Public Methods

        /// <summary>
        /// Sets the is distinct.
        /// </summary>
        /// <param name="isDistinct">if set to <c>true</c> [is distinct].</param>
        /// <returns></returns>
        public SelectSqlSection SetIsDistinct(bool isDistinct)
        {
            _isDistinct = isDistinct;
            OnChanged();
            return this;
        }

        public SelectSqlSection SetIsOnLock(bool isonlock)
        {
            _from.IsOnlock = isonlock;
            OnChanged();
            return this;
        }         

        public SelectSqlSection SetMaxResults(int n)
        {
            _maxResults = n;
            OnChanged();
            return this;
        }

        public SelectSqlSection SetSkipResults(int n)
        {
            _skipResults = n;
            OnChanged();
            return this;
        }

        public SelectSqlSection SortBy(ExpressionClip column, bool isDescendent)
        {
            if (ReferenceEquals(column, null))
                throw new ArgumentNullException("column");

            if (!_sortBys.ContainsKey(column))
                _sortBys.Add(column, isDescendent);
            OnChanged();
            return this;
        }

        public SelectSqlSection SortBy(OrderByClip orderby)
        {
            if (ReferenceEquals(orderby, null))
                throw new ArgumentNullException("orderby");

            if (!_sortBys.ContainsKey(orderby.Orderby.Key))
                _sortBys.Add(orderby.Orderby.Key, orderby.Orderby.Value);
            OnChanged();
            return this;
        }

        public SelectSqlSection ThenSortBy(ExpressionClip column, bool isDescendent)
        {
            return SortBy(column, isDescendent);
        }

        public SelectSqlSection ThenSortBy(OrderByClip orderby)
        {
            return SortBy(orderby.Orderby.Key, orderby.Orderby.Value);
        }

        public SelectSqlSection And(Condition condition)
        {
            if (ReferenceEquals(condition, null))
                throw new ArgumentNullException("condition");


            _condition.LinkedConditionAndOrs.Add(ConditionAndOr.And);
            _condition.LinkedConditions.Add(condition);

            OnChanged();
            return this;
        }

        public SelectSqlSection AndHaving(Condition condition)
        {
            if (ReferenceEquals(condition, null))
                throw new ArgumentNullException("condition");

            _havingcondition.LinkedConditionAndOrs.Add(ConditionAndOr.And);
            _havingcondition.LinkedConditions.Add(condition);

            OnChanged();
            return this;
        }

        public SelectSqlSection Or(Condition condition)
        {
            if (ReferenceEquals(condition, null))
                throw new ArgumentNullException("condition");

            _condition.LinkedConditionAndOrs.Add(ConditionAndOr.Or);
            _condition.LinkedConditions.Add(condition);

            OnChanged();
            return this;
        }

        public SelectSqlSection OrHaving(Condition condition)
        {
            if (ReferenceEquals(condition, null))
                throw new ArgumentNullException("condition");

            _havingcondition.LinkedConditionAndOrs.Add(ConditionAndOr.Or);
            _havingcondition.LinkedConditions.Add(condition);

            OnChanged();
            return this;
        }

        public SelectSqlSection Select(params ExpressionClip[] columns)
        {
            ClearResultColumns();
            foreach (var column in columns)
            {
                if (!_resultColumns.Contains(column))
                    _resultColumns.Add(column);
            }
            return this;
        }

        public SelectSqlSection BindColumn(params ExpressionClip[] columns)
        {
            foreach (var column in columns)
            {
                if (!_resultColumns.Contains(column))
                    _resultColumns.Add(column);
            }
            return this;
        }

        public SelectSqlSection Where(Condition condition)
        {
            return And(condition);
        }

        public SelectSqlSection Having(Condition condition)
        {
            return AndHaving(condition);
        }

        public SelectSqlSection GroupBy(params IColumn[] columns)
        {
            if (ReferenceEquals(columns, null))
                throw new ArgumentNullException("columns");
            _groups.AddRange(columns);
            return this;
        }

        public SelectSqlSection Join(IQueryTable joinTable, string joinTableAliasName, Condition joinOnWhere)
        {
            From.Join(joinTable.GetTableName(), joinTableAliasName, joinOnWhere);
            JoinCondition(joinOnWhere);
            return this;
        }

        public SelectSqlSection Join(IQueryTable joinTable, Condition joinOnWhere)
        {
            return Join(joinTable, joinTable.GetTableName(), joinOnWhere);
        }

        public SelectSqlSection LeftJoin(IQueryTable joinTable, string joinTableAliasName, Condition joinOnWhere)
        {
            From.LeftJoin(joinTable.GetTableName(), joinTableAliasName, joinOnWhere);
            JoinCondition(joinOnWhere);
            return this;
        }

        public SelectSqlSection LeftJoin(IQueryTable joinTable, Condition joinOnWhere)
        {
            return LeftJoin(joinTable, joinTable.GetTableName(), joinOnWhere);
        }

        public SelectSqlSection RightJoin(IQueryTable joinTable, string joinTableAliasName, Condition joinOnWhere)
        {
            From.RightJoin(joinTable.GetTableName(), joinTableAliasName, joinOnWhere);
            JoinCondition(joinOnWhere);
            return this;
        }

        public SelectSqlSection RightJoin(IQueryTable joinTable, Condition joinOnWhere)
        {
            return RightJoin(joinTable, joinTable.GetTableName(), joinOnWhere);
        }

        public SelectSqlSection FullJoin(IQueryTable joinTable, string joinTableAliasName, Condition joinOnWhere)
        {
            From.FullJoin(joinTable.GetTableName(), joinTableAliasName, joinOnWhere);
            JoinCondition(joinOnWhere);
            return this;
        }

        public SelectSqlSection FullJoin(IQueryTable joinTable, Condition joinOnWhere)
        {
            return FullJoin(joinTable, joinTable.GetTableName(), joinOnWhere);
        }

        public override object Clone()
        {
            var clone = new SelectSqlSection(Db, TableName);
            clone.IdentyColumnName = IdentyColumnName;
            clone.IdentyColumnIsNumber = IdentyColumnIsNumber;
            clone._condition = (Condition)_condition.Clone();
            clone._havingcondition = (Condition)_havingcondition.Clone();
            clone._isDistinct = _isDistinct;
            clone._maxResults = _maxResults;
            //clone._predefinedColumns.AddRange(_predefinedColumns);
            clone._resultColumns.AddRange(_resultColumns);
            clone._skipResults = _skipResults;
            foreach (var item in _sortBys)
            {
                clone._sortBys.Add(item.Key, item.Value);
            }
            clone._tableName = _tableName;

            //add groupby
            foreach (var column in _groups)
            {
                clone._groups.Add(column);
            }

            //from
            clone._from = new FromClip(_from.TableOrViewName);
            clone._fromCondition = (Condition)_fromCondition.Clone();
            return clone;
        }

        public SelectSqlSection SetSelectRange(int topItemCount, int skipItemCount, QueryColumn identyColumn)
        {
            SetMaxResults(topItemCount);
            SetSkipResults(skipItemCount);
            IdentyColumnName = identyColumn.ColumnName.ToDatabaseObjectName();
            IdentyColumnIsNumber =
                (identyColumn.DataType == DbType.Int32) ||
                (identyColumn.DataType == DbType.Int16) ||
                (identyColumn.DataType == DbType.Int64) ||
                (identyColumn.DataType == DbType.Byte) ||
                (identyColumn.DataType == DbType.Double) ||
                (identyColumn.DataType == DbType.Currency) ||
                (identyColumn.DataType == DbType.Decimal) ||
                (identyColumn.DataType == DbType.SByte) ||
                (identyColumn.DataType == DbType.Single) ||
                (identyColumn.DataType == DbType.UInt16) ||
                (identyColumn.DataType == DbType.UInt32) ||
                (identyColumn.DataType == DbType.UInt64);
            return this;
        }

        public SubQuery ToSubQuery()
        {
            DbCommand cmd = Db.CommandBuilder.BuildCommandSql(this, false);
            var expr = new SubQuery { Sql = "(" + cmd.CommandText + ")" };
            foreach (DbParameter parameter in cmd.Parameters)
            {
                expr.ChildExpressions.Add(new ParameterExpression(parameter.Value, parameter.DbType));
            }
            return expr;
        }

        public SubQuery ToSubQuery(Database db)
        {
            DbCommand cmd = db.CommandBuilder.BuildCommandSql(this, false);
            var expr = new SubQuery { Sql = "(" + cmd.CommandText + ")" };
            foreach (DbParameter parameter in cmd.Parameters)
            {
                expr.ChildExpressions.Add(new ParameterExpression(parameter.Value, parameter.DbType));
            }
            return expr;
        }

        public override string ToString()
        {
            var retSql = new StringBuilder();
            if (From != null)
            {
                retSql.Append(From.ToString());

                if (ConditionWhere.LinkedConditions.Count > 0)
                {
                    retSql.Append(" WHERE ");
                    if (ConditionWhere.IsNot)
                    {
                        retSql.Append("(NOT (");
                    }
                    QueryCommandBuilder.AppendConditions(ConditionWhere, retSql);
                    if (ConditionWhere.IsNot)
                    {
                        retSql.Append("))");
                    }
                }
                if (_groups.Count != 0)
                {
                    retSql.Append(" GROUP BY ");
                    QueryCommandBuilder.AppendGroupByColumns(retSql, _groups);
                }
                if (HavingConditionWhere.LinkedConditions.Count > 0 && _groups.Count != 0)
                {
                    if (_groups.Count != 0)
                    {
                        retSql.Append(" HAVING ");
                    }
                    if (HavingConditionWhere.IsNot)
                    {
                        retSql.Append("(NOT (");
                    }
                    QueryCommandBuilder.AppendConditions(HavingConditionWhere, retSql);
                    if (HavingConditionWhere.IsNot)
                    {
                        retSql.Append("))");
                    }
                }
                if (_sortBys.Count != 0)
                {
                    retSql.Append(" ORDER BY ");
                    QueryCommandBuilder.AppendSortBys(this, retSql);
                }
            }
            else
            {
                if (ConditionWhere.LinkedConditions.Count > 0)
                {
                    if (ConditionWhere.IsNot)
                    {
                        retSql.Append("(NOT (");
                    }
                    QueryCommandBuilder.AppendConditions(ConditionWhere, retSql);
                    if (ConditionWhere.IsNot)
                    {
                        retSql.Append("))");
                    }
                }
            }
            return retSql.ToString();
        }

        #region Clear

        public void ClearResultColumns()
        {
            _resultColumns.Clear();
            OnChanged();
        }

        public void ClearConditions()
        {
            _condition.LinkedConditionAndOrs.Clear();
            _condition.LinkedConditions.Clear();
            OnChanged();
        }

        public void ClearSortBys()
        {
            _sortBys.Clear();
            OnChanged();
        }

        public void ClearHaving()
        {
            _havingcondition.LinkedConditionAndOrs.Clear();
            _havingcondition.LinkedConditions.Clear();
            OnChanged();
        }

        public void ClearGroupBy()
        {
            _groups.Clear();
            OnChanged();
        }

        public void Reset()
        {
            ClearConditions();
            ClearGroupBy();
            ClearHaving();
            ClearResultColumns();
            ClearSortBys();
            Tran = null;
            _maxResults = 0;
            _skipResults = 0;
            _isDistinct = false;
            _from = new FromClip(TableName);
            _fromCondition = new Condition();
            IdentyColumnName = null;
            IdentyColumnIsNumber = false;
        }
        #endregion

        #endregion

        #region No-Public Method

        private void JoinCondition(Condition joinOnWhere)
        {
            _fromCondition.LinkedConditions.Add(joinOnWhere);
            _fromCondition.LinkedConditionAndOrs.Add(ConditionAndOr.And);
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
