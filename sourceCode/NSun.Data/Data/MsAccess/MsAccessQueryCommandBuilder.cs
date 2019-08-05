using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Text;
using NSun.Data.SqlClient;

namespace NSun.Data.MsAccess
{
    public class MsAccessQueryCommandBuilder : Sql2000QueryCommandBuilder
    {
        /// <summary>
        /// The singleton.
        /// </summary>
        public static readonly MsAccessQueryCommandBuilder Instance;

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MsAccessQueryCommandBuilder"/> class.
        /// </summary>
        protected MsAccessQueryCommandBuilder()
        {

        }

        /// <summary>
        /// Initializes the <see cref="MsAccessQueryCommandBuilder"/> class.
        /// </summary>
        static MsAccessQueryCommandBuilder()
        {
            Instance = new MsAccessQueryCommandBuilder();
        }

        #endregion

        public override System.Data.Common.DbProviderFactory GetDbProviderFactory()
        {
            return OleDbFactory.Instance;
        }

        protected override string BuildPagingCacheableSql(QueryCriteria criteria)
        {
            var select = criteria as SelectSqlSection;
            return select.SortBys.Count == 0 ?
                BuildPagingCacheableMaxMinSql(select) :
                BuildPagingCacheableNotInSql(select);
        }

        protected override void AdjustParameterProperties(IParameterExpression parameterExpr, System.Data.Common.DbParameter parameter)
        {
            OleDbParameter oleDbParam = (OleDbParameter)parameter;
            object value = parameter.Value;
            parameter.Value = GetDbValue(value);
            if (parameter.Value == null || parameter.Value == DBNull.Value) return;
            Type type = parameter.Value.GetType();
            if (oleDbParam.DbType != DbType.Guid && type == typeof(Guid))
            {
                oleDbParam.OleDbType = OleDbType.Char;
                oleDbParam.Size = 36;
                return;
            }

            if ((parameter.DbType == DbType.Time || parameter.DbType == DbType.DateTime) && type == typeof(TimeSpan))
            {
                oleDbParam.OleDbType = OleDbType.Double;
                oleDbParam.Value = ((TimeSpan)value).TotalDays;
                return;
            } 
            if (parameter.DbType == DbType.Binary)
            {
                oleDbParam.OleDbType = ((byte[])value).Length > 2000 ? OleDbType.LongVarBinary : OleDbType.VarBinary;
                return;
            }
            if (parameter.DbType == DbType.Date || parameter.DbType == DbType.DateTime || parameter.DbType == DbType.Time || parameter.DbType == DbType.DateTime2)
            { 
                oleDbParam.OleDbType = OleDbType.LongVarWChar;
                oleDbParam.Value = value.ToString();
            } 
            if (parameter.DbType == DbType.String)
            {
                oleDbParam.OleDbType = value.ToString().Length > 2000 ? OleDbType.LongVarWChar : OleDbType.VarWChar;
                return;
            }
            if (parameter.DbType == DbType.AnsiString)
            {
                oleDbParam.OleDbType = value.ToString().Length >4000 ? OleDbType.LongVarChar : OleDbType.VarChar;
                return;
            }
            if (parameter.DbType == DbType.Object)
            {
                oleDbParam.OleDbType = OleDbType.LongVarWChar;
                parameter.Value = ExpressionHelper.Serialize(parameter.Value);
                return;
            }
            if (parameter.Value.GetType().IsEnum && parameter.DbType == DbType.Int32)
            {
                parameter.Value = (int)parameter.Value;
                return;
            }
        }

        protected override string BuildInsertCacheableSql(QueryCriteria criteria)
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
                string column = columnValue.Key;
                if (columnValue.Key.Split('.').Length > 1)
                {
                    column = columnValue.Key.Split('.')[1];
                }
                // sb.Append(columnValue.Key);
                sb.Append(column);
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

        protected override string BuildUpdateCacheableSql(QueryCriteria criteria1)
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
                string column = columnValue.Key;
                if (columnValue.Key.Split('.').Length > 1)
                {
                    column = columnValue.Key.Split('.')[1];
                }
                //sb.Append(columnValue.Key);
                sb.Append(column);
                sb.Append(" = ");
                sb.Append(ReferenceEquals(columnValue.Value, null)
                              ? "NULL"
                              : columnValue.Value.ToExpressionCacheableSql());
                separate = ", ";
            }
            if (criteria.ConditionWhere.LinkedConditions.Count > 0)
            {
                sb.Append(" ");
                sb.Append("WHERE ");
                AppendConditions(criteria, sb);
            }

            return sb.ToString();
        }

        #region Override

        public override string GetSelectLastInsertAutoIncrementIDSql(string tableName, string columnName, Dictionary<string, string> options)
        {
            return "SELECT @@IDENTITY";
        }

        public override bool SupportADO20Transaction
        {
            get
            {
                return false;
            }
        }

        public override bool SupportMultiSqlStatementInOneCommand
        {
            get
            {
                return false;
            }
        }
        #endregion
    }
}
