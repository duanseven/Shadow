using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;

namespace NSun.Data
{ 
    public abstract class QueryCommandBuilder
    {
        #region Private Members

        private static readonly Dictionary<string, DbCommand> _cachedCommands
            = new Dictionary<string, DbCommand>();
        private static readonly object _cachedCommandsLock = new object();

        #endregion

        #region Public Method

        public DbCommand BuildCommandSql(QueryCriteria criteria)
        {
            return BuildCommandSql(criteria, false);
        }

        public DbCommand BuildCommandSql(QueryCriteria criteria, bool isCountCommand)
        {
            if (criteria == null)
                throw new ArgumentNullException("criteria");
            DbCommand cmd = GetDbProviderFactory().CreateCommand();
            BuildCommandParameters(criteria, cmd.Parameters, false);
            cmd.CommandText = BuildCacheableSql(criteria, isCountCommand);
            return cmd;
        }


        public DbCommand BuildCommand(QueryCriteria criteria)
        {
            return BuildCommand(criteria, false);
        } 

        public DbCommand BuildCommand(QueryCriteria criteria, bool isCountCommand)
        {
            if (criteria == null)
                throw new ArgumentNullException("criteria");

            DbCommand cmd;

            var cacheableSql = BuildCacheableSql(criteria, isCountCommand);
            if (!_cachedCommands.ContainsKey(cacheableSql))//缓存中找不到
            {
                lock (_cachedCommandsLock)
                {
                    if (!_cachedCommands.ContainsKey(cacheableSql))
                    {
                        cmd = GetDbProviderFactory().CreateCommand();
                        cmd.CommandType = CommandType.Text;
                        //第一次加载参数
                        BuildCommandParameters(criteria, cmd.Parameters, false);
                        cmd.CommandText = BuildCommandText(cacheableSql, cmd.Parameters);
                        
                        var cachedCmd = CloneCommand(cmd);

                        //清除参数缓存command
                        foreach (DbParameter p in cachedCmd.Parameters)
                        {
                            p.Value = null;
                        }
                        _cachedCommands.Add(cacheableSql, cachedCmd);

                        return cmd;
                    }
                }
            }
            cmd = CloneCommand(_cachedCommands[cacheableSql]);
            //重新加载参数
            BuildCommandParameters(criteria, cmd.Parameters, true);

            return cmd;
        }

        public DbCommand BuildCommandSprocOrCustom(QueryCriteria criteria)
        {
            if (criteria == null)
                throw new ArgumentNullException("criteria");
            DbCommand cmd = GetDbProviderFactory().CreateCommand();
            cmd.CommandText = criteria.TableName;
            if (criteria.QueryType == QueryType.Sproc)
            {
                cmd.CommandType = CommandType.StoredProcedure;
            }
            if (criteria.QueryType == QueryType.Custom)
            {
                cmd.CommandType = CommandType.Text;
            }
            var cri = criteria as IParameterConditions;
            var sprocCmd = new SprocDbCommand(cmd, this);
            foreach (var parameterCondition in cri.ParameterConditions)
            {
                sprocCmd.AddParameter(parameterCondition);
            }
            return sprocCmd.Command;
        }

        #endregion

        #region Non-Public Methods  
 
        protected string BuildCommandText(string cacheableSql, DbParameterCollection parameterCollection)
        {
            foreach (DbParameter dbparameter in parameterCollection)
            {
                cacheableSql = cacheableSql.Replace(dbparameter.ParameterName, "?");
            }
            var splittedSql = cacheableSql.Split('?');
            var sb = new StringBuilder();
            if (splittedSql.Length > 1)
            {
                sb.Append(splittedSql[0]);
                for (var i = 1; i < splittedSql.Length; ++i)
                {
                    sb.Append(parameterCollection[i - 1].ParameterName);
                    sb.Append(splittedSql[i]);
                }
            }
            else
            {
                sb.Append(cacheableSql);
            }
            //replace table or column name quote charactors
            var quoteChars = GetDatabaseObjectNameQuoteCharacters();
            if (quoteChars.Length > 0)
            {
                var leftChar = quoteChars[0];
                var rightChar = (quoteChars.Length > 1 ? quoteChars[1] : leftChar);
                return sb.ToString().Replace('[', leftChar).Replace(']', rightChar);
            }

            return sb.ToString();
        }
 
        protected string BuildCacheableSql(QueryCriteria criteria, bool isCountCommand)
        {
            if (criteria == null)
                throw new ArgumentNullException("criteria");

            if (isCountCommand)
                return BuildNoPagingCacheableSql(criteria, isCountCommand);

            switch (criteria.QueryType)
            {
                case QueryType.Select:
                    var selectcriteria = (SelectSqlSection)criteria;
                    if (selectcriteria.SkipResults == 0)
                        return BuildNoPagingCacheableSql(criteria, isCountCommand);
                    return BuildPagingCacheableSql(criteria);
                case QueryType.Insert:
                    return BuildInsertCacheableSql(criteria);
                case QueryType.Update:
                    return BuildUpdateCacheableSql(criteria);
                case QueryType.Delete:
                    return BuildDeleteCacheableSql(criteria);
                case QueryType.Custom:
                    return criteria.TableName;
            }

            return null;
        }

        protected virtual string BuildInsertCacheableSql(QueryCriteria criteria)
        {
            InsertSqlSection insert = criteria as InsertSqlSection;
            var columnValues = GetColumnValues(insert.Assignments);

            var sb = new StringBuilder();
            sb.Append("INSERT INTO ");
            sb.Append(criteria.TableName.ToDatabaseObjectName());

            if (columnValues == null || columnValues.Count == 0)
            {
                sb.Append(" DEFAULT VALUES");
                return sb.ToString();
            }
            sb.Append(" (");
            var separate = "";
            foreach (var columnValue in columnValues)
            {
                sb.Append(separate);
                sb.Append(columnValue.Key);
                separate = ", ";
            }
            sb.Append(") VALUES (");
            separate = "";
            foreach (var columnValue in columnValues)
            {
                sb.Append(separate);
                sb.Append(ReferenceEquals(columnValue.Value, null)
                              ? "NULL"
                              : columnValue.Value.ToExpressionCacheableSql());
                separate = ", ";
            }
            sb.Append(")");

            return sb.ToString();
        }

        protected Dictionary<string, IExpression> GetColumnValues(ICollection<Assignment> assignments)
        {
            var result = new Dictionary<string, IExpression>();

            if (assignments != null)
            {
                foreach (var assignment in assignments)
                {
                    result[assignment.LeftColumn.ColumnName.ToDatabaseObjectName()] = assignment.RightExpression;
                }
            }

            return result;
        }
         
        protected virtual string BuildUpdateCacheableSql(QueryCriteria criteria1)
        {
            var criteria = criteria1 as UpdateSqlSection;

            var columnValues = GetColumnValues(criteria.Assignments);

            var sb = new StringBuilder();
            sb.Append("UPDATE ");
            sb.Append(criteria.TableName.ToDatabaseObjectName());
            sb.Append(" SET ");
            var separate = "";
            foreach (var columnValue in columnValues)
            {
                sb.Append(separate);
                sb.Append(columnValue.Key);
                sb.Append(" = ");
                sb.Append(ReferenceEquals(columnValue.Value, null)
                              ? "NULL"
                              : columnValue.Value.ToExpressionCacheableSql());
                separate = ", ";
            }

            if (criteria.FromCondition.LinkedConditions.Count > 0)
            {
                sb.Append(" FROM ");
                sb.Append(criteria.From.ToString());
            }

            if (criteria.ConditionWhere.LinkedConditions.Count > 0)
            {
                sb.Append(" ");
                sb.Append("WHERE ");
                AppendConditions(criteria, sb);
            }

            return sb.ToString();
        }
         
        private static string BuildDeleteCacheableSql(QueryCriteria criteria1)
        {
            var criteria = criteria1 as DeleteSqlSection;
            var sb = new StringBuilder();
            if (criteria.FromCondition.LinkedConditions.Count > 0)
            {
                sb.Append("DELETE ");
                sb.Append(criteria.TableName.ToDatabaseObjectName());
                sb.Append(" FROM ");
                sb.Append(criteria.From.ToString());           
            }
            else
            {
                sb.Append("DELETE FROM ");
                sb.Append(criteria.TableName.ToDatabaseObjectName());                
            }
            if (criteria.ConditionWhere.LinkedConditions.Count > 0)
            {
                sb.Append(" ");
                sb.Append("WHERE ");
                AppendConditions(criteria, sb);
            }
            return sb.ToString();
        }
 
        public static void AppendSortBys(QueryCriteria criteria, StringBuilder sb)
        {
            var select = criteria as SelectSqlSection;
            var separate = "";
            var en = select.SortBys.GetEnumerator();
            while (en.MoveNext())
            {
                sb.Append(separate);
                sb.Append(en.Current.Key.ToExpressionCacheableSql());
                if (en.Current.Value)
                {
                    sb.Append(" DESC");
                }
                separate = ", ";
            }
        }
         
        protected internal static void AppendConditions(QueryCriteria criteria, StringBuilder sb)
        {
            var select = criteria as IConditionSection;

            for (var i = 0; i < select.ConditionWhere.LinkedConditions.Count; ++i)
            {
                if (i > 0)
                {
                    switch (select.ConditionWhere.LinkedConditionAndOrs[i])
                    {
                        case ConditionAndOr.And:
                            sb.Append(" AND ");
                            break;
                        case ConditionAndOr.Or:
                            sb.Append(" OR ");
                            break;
                        case ConditionAndOr.Space:
                            sb.Append(" ");
                            break;
                    }
                    //sb.Append(select.ConditionWhere.LinkedConditionAndOrs[i] == ConditionAndOr.And ? " AND " : " OR ");
                }
                sb.Append(select.ConditionWhere.LinkedConditions[i].ToConditionCacheableSql());
            }
        }

        protected internal static void AppendConditions(Condition criteria, StringBuilder sb)
        {
            for (var i = 0; i < criteria.LinkedConditions.Count; ++i)
            {
                if (i > 0)
                {
                    //if (criteria.LinkedConditionAndOrs[i] == ConditionAndOr.And)
                    //{
                    //    sb.Append(" AND ");
                    //}
                    //else
                    //{
                    //    sb.Append(" OR ");
                    //}
                    switch (criteria.LinkedConditionAndOrs[i])
                    {
                        case ConditionAndOr.And:
                            sb.Append(" AND ");
                            break;
                        case ConditionAndOr.Or:
                            sb.Append(" OR ");
                            break;
                        case ConditionAndOr.Space:
                            sb.Append(" ");
                            break;
                    }
                }
                sb.Append(criteria.LinkedConditions[i].ToConditionCacheableSql());
            }
        }

        protected static void AppendFrom(string tableName, StringBuilder sb)
        {
            sb.Append("FROM ");
            sb.Append(tableName.ToDatabaseObjectName());
        }
 
        protected static void AppendResultColumns(QueryCriteria criteria, StringBuilder sb)
        {
            var select = criteria as SelectSqlSection; 
            if (select.ResultColumns.Count == 0)
            { 
                sb.Append("*"); 
            }
            else
            {
                AppendResultColumns(sb, select.ResultColumns);
            }
        }
 
        protected void BuildCommandParameters(QueryCriteria criteria, DbParameterCollection parameterCollection, bool setParameterValueOnly)
        {
            if (criteria == null) return;
            if (criteria.QueryType == QueryType.Select)
            {
                var select = criteria as SelectSqlSection;
                if (select.Table != null)
                {
                    if (select.Table.GetType() == typeof(CustomWithQueryTable))
                    {
                        var table = select.Table as CustomWithQueryTable;
                        if (!ReferenceEquals(table, null))
                        {
                            foreach (var childExpression in table.ChildExpressions)
                            {
                                AddExpressionParameters(childExpression, parameterCollection, setParameterValueOnly);
                            }
                        }
                    }
                }
                foreach (var column in select.ResultColumns)
                {
                    AddExpressionParameters(column, parameterCollection, setParameterValueOnly);
                }
            }
            if (criteria.QueryType == QueryType.Update || criteria.QueryType == QueryType.Insert)
            {
                var insert = criteria as ICudSection;
                foreach (var assignment in insert.Assignments)
                {
                    AddAssignmentParameters(assignment, parameterCollection, setParameterValueOnly);
                }
            }
            if (criteria.QueryType == QueryType.Select)
            {
                var select = criteria as SelectSqlSection;
                if (select.Table != null)
                {
                    if (select.Table.GetType() == typeof(CustomQueryTable))
                    {
                        var table = select.Table as CustomQueryTable;
                        if (!ReferenceEquals(table, null))
                        {
                            foreach (var childExpression in table.ChildExpressions)
                            {
                                AddExpressionParameters(childExpression, parameterCollection, setParameterValueOnly);
                            }
                        }
                    }
                }
                foreach (var condition in select.FromCondition.LinkedConditions)
                {
                    AddConditionParameters(condition, parameterCollection, setParameterValueOnly);
                }
            }
            if (criteria.QueryType == QueryType.Select || criteria.QueryType == QueryType.Update || criteria.QueryType == QueryType.Delete)
            {
                var select = criteria as IConditionSection;
                foreach (var condition in select.ConditionWhere.LinkedConditions)
                {
                    AddConditionParameters(condition, parameterCollection, setParameterValueOnly);
                }
            }
            if (criteria.QueryType == QueryType.Select)
            {
                var select = criteria as SelectSqlSection;
                foreach (var condition in select.HavingConditionWhere.LinkedConditions)
                {
                    AddConditionParameters(condition, parameterCollection, setParameterValueOnly);
                }
            }
        }
 
        protected void AddAssignmentParameters(Assignment assignment, DbParameterCollection parameterCollection, bool setParameterValueOnly)
        {
            AddExpressionParameters(assignment.LeftColumn, parameterCollection, setParameterValueOnly);
            if (!ReferenceEquals(assignment.RightExpression, null))
                AddExpressionParameters(assignment.RightExpression, parameterCollection, setParameterValueOnly);
        }
 
        protected void AddConditionParameters(Condition condition, DbParameterCollection parameterCollection, bool setParameterValueOnly)
        {
            AddExpressionParameters(condition.LeftExpression, parameterCollection, setParameterValueOnly);
            if (!ReferenceEquals(condition.RightExpression, null))
                AddExpressionParameters(condition.RightExpression, parameterCollection, setParameterValueOnly);

            foreach (var linkedCondition in condition.LinkedConditions)
            {
                AddConditionParameters(linkedCondition, parameterCollection, setParameterValueOnly);
            }
        }

        internal void AddParameterEqualsConditionParameter(ParameterEqualsCondition condition, DbParameterCollection parameterCollection)
        {
            var parameter = GetDbProviderFactory().CreateParameter();
            //parameter.ParameterName = ToParameterName(condition.LeftExpression.ID);
            parameter.ParameterName = condition.LeftExpression.ID;
            var direction = SprocParameterDirection.Input;
            if (condition.LeftExpression.Direction.HasValue)
                direction = condition.LeftExpression.Direction.Value;
            parameter.Direction =
                (ParameterDirection)
                Enum.Parse(typeof(ParameterDirection), direction.ToString());
            parameter.Value = (condition.RightExpression == null ?
                DBNull.Value : condition.RightExpression.Value);
            parameter.DbType = condition.LeftExpression.DataType;
            if (condition.LeftExpression.Size.HasValue)
                parameter.Size = condition.LeftExpression.Size.Value;
            AdjustParameterProperties(condition.LeftExpression, parameter);
            parameterCollection.Add(parameter);
        }
 
        protected void AddExpressionParameters(IExpression expr, DbParameterCollection parameterCollection, bool setParameterValueOnly)
        {
            if (expr == null) return;
            var parameterExpr = expr as IParameterExpression;
            if (parameterExpr != null)
            {
                AddParameter(parameterExpr, parameterCollection, setParameterValueOnly);
            }
            if (expr.ChildExpressions == null) return;
            foreach (var childExpr in expr.ChildExpressions)
            {
                AddExpressionParameters(childExpr, parameterCollection, setParameterValueOnly);
            }
        }
 
        protected void AddParameter(IParameterExpression parameterExpr, DbParameterCollection parameterCollection, bool setParameterValueOnly)
        {
            if (setParameterValueOnly)
            {
                foreach (DbParameter parameter in parameterCollection)
                {
                    if (parameter.Value != null) continue;
                    parameter.Value = parameterExpr.Value ?? DBNull.Value;
                    parameter.DbType = parameterExpr.DataType;
                    AdjustParameterProperties(parameterExpr, parameter);
                    break;
                }
            }
            else
            {
                var parameter = GetDbProviderFactory().CreateParameter();
                parameter.ParameterName = string.IsNullOrEmpty(parameterExpr.ID)
                                              ? ToParameterName("p" + (parameterCollection.Count + 1))
                                              : parameterExpr.ID;
                parameter.Value = parameterExpr.Value;
                parameter.DbType = parameterExpr.DataType;
                AdjustParameterProperties(parameterExpr, parameter);
                parameterCollection.Add(parameter);
            }
        }
 
        protected virtual DbCommand CloneCommand(DbCommand cmd)
        {
            var cloneable = cmd as ICloneable;
            if (cloneable != null)
                return (DbCommand)cloneable.Clone();

            return null;
        }

        protected static void AppendResultColumns(StringBuilder sb, IList<ExpressionClip> columns)
        {
            var separate = "";
            foreach (var column in columns)
            {
                sb.Append(separate);

                sb.Append(column.ToSelectColumnName());
                separate = ", ";
            }
        }

        protected internal static string AppendResultColumns(IList<ExpressionClip> columns)
        {
            StringBuilder sb = new StringBuilder();
            var separate = "";
            foreach (var column in columns)
            {
                sb.Append(separate);

                sb.Append(column.ToSelectColumnName());
                separate = ", ";
            }
            return sb.ToString();
        }

        protected static string AppendResultColumns(QueryCriteria criteria)
        {
            var select = criteria as SelectSqlSection;
            StringBuilder sb = new StringBuilder();
            if (select.ResultColumns.Count == 0)
            {
                sb.Append(criteria.TableName + ".*");
            }
            else
            {
                AppendResultColumns(sb, select.ResultColumns);
            }
            return sb.ToString();
        }

        protected internal static void AppendGroupByColumns(StringBuilder sb, IList<IColumn> _groups)
        {             
            var separate = "";
            foreach (var column in _groups)
            {
                sb.Append(separate);

                sb.Append(column.ToSelectColumnName());
                separate = ", ";
            }
        }


        protected static object GetDbValue(object dotNetValue)
        {
            if (dotNetValue == null)
            {
                return DBNull.Value;
            }
            if (dotNetValue.GetType().IsEnum)
            {
                return Convert.ToInt32(dotNetValue, CultureInfo.InvariantCulture);
            }
            return dotNetValue;
        }
      
        #endregion

        #region Override

        public abstract DbProviderFactory GetDbProviderFactory();         

        internal protected abstract string ToParameterName(string name);

        protected abstract string GetDatabaseObjectNameQuoteCharacters();

        protected abstract string BuildPagingCacheableSql(QueryCriteria criteria);

        protected virtual string BuildNoPagingCacheableSql(QueryCriteria criteria, bool isCountCommand)
        {
            var select = criteria as SelectSqlSection;
            if (isCountCommand)
                return BuildCountCacheableSql(select);
            StringBuilder sb = new StringBuilder("SELECT ");
            if (select.IsDistinct)
            {
                sb.Append("DISTINCT ");
            }
            AppendResultColumns(select, sb);
            sb.Append(" FROM ");
            sb.Append(select.ToString());
            return sb.ToString();
        }

        protected virtual string BuildCountCacheableSql(SelectSqlSection criteria)
        {
            SelectSqlSection select = (SelectSqlSection)criteria.Clone();
            select.ClearSortBys();
            var sb = new StringBuilder("SELECT COUNT(1) FROM ");
            if (select.Groups.Count > 0 || select.IsDistinct)
            {
                sb.Append("(SELECT ");
                if (select.IsDistinct)
                {
                    sb.Append(" DISTINCT ");
                }
                AppendResultColumns(select, sb);
                sb.Append(" FROM ");
                sb.Append(select.ToString());
            }
            else
            {
                sb.Append(select.ToString());
            }
            if (select.Groups.Count > 0 || select.IsDistinct)
                sb.Append(" ) AS GROUPQUERY_TABLE");

            return sb.ToString();
        }

        protected abstract void AdjustParameterProperties(IParameterExpression parameterExpr, DbParameter parameter);

        public virtual string GetSelectLastInsertAutoIncrementIDSql(string tableName, string columnName, System.Collections.Generic.Dictionary<string, string> options)
        {
            return string.Format("SELECT MAX([{0}]) FROM [{1}]", columnName, tableName);
        }

        public virtual bool SupportADO20Transaction { get; set; }

        public virtual bool SupportMultiSqlStatementInOneCommand { get; set; }

        #endregion
    }
}
