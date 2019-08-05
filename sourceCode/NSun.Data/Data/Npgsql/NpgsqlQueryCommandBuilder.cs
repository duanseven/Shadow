using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Reflection;
using System.Text;
namespace NSun.Data.Npgsql
{
    public class NpgsqlQueryCommandBuilder : QueryCommandBuilder
    {
        private static Assembly asm; 
        private static Dictionary<string, object> PostgreSqlDbType = new Dictionary<string, object>();

        protected NpgsqlQueryCommandBuilder()
        {
            asm = Assembly.Load("Npgsql"); 
            var mysqldbtype = asm.GetType("NpgsqlTypes.NpgsqlDbType");
            FieldInfo[] fields =
                mysqldbtype.GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
            PostgreSqlDbType.Clear();
            foreach (var item in fields)
            {
                if (!PostgreSqlDbType.ContainsKey(item.Name.ToUpper()))
                {
                    PostgreSqlDbType.Add(item.Name.ToUpper(), item.GetValue(mysqldbtype));
                }
            }
           
        }

        static NpgsqlQueryCommandBuilder()
        {
            Instance = new NpgsqlQueryCommandBuilder();
        }

        public static readonly NpgsqlQueryCommandBuilder Instance;

        public override System.Data.Common.DbProviderFactory GetDbProviderFactory()
        {
            Type type = asm.GetType("Npgsql.NpgsqlFactory");
            FieldInfo field = type.GetField("Instance");
            return field.GetValue(null) as DbProviderFactory;
        }

        protected internal override string ToParameterName(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");
            return ":" + name.TrimStart(':');
        }

        protected override string GetDatabaseObjectNameQuoteCharacters()
        {
            return "\"\"";
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

        protected override string BuildNoPagingCacheableSql(QueryCriteria criteria, bool isCountCommand)
        {
            var select = criteria as SelectSqlSection;
            if (select.MaxResults > 0)
            {
                return BuildPagingCacheableSql(criteria);
            }
            else
            {
                return base.BuildNoPagingCacheableSql(criteria, isCountCommand);
            }
        }

        protected override string BuildPagingCacheableSql(QueryCriteria criteria)
        {
            StringBuilder sb = new StringBuilder(base.BuildNoPagingCacheableSql(criteria, false));
            var select = criteria as SelectSqlSection;
            if (select.SkipResults == 0)
            {
                sb.Append(" LIMIT " + select.MaxResults);
            }
            else
            {
                sb.Append(" LIMIT " + select.SkipResults);
                sb.Append(" OFFSET " + select.MaxResults);
            }
            return sb.ToString();
        }

        protected override void AdjustParameterProperties(IParameterExpression parameterExpr, System.Data.Common.DbParameter parameter)
        {
            var mysqlp = asm.GetType("Npgsql.NpgsqlParameter");
            object value = parameter.Value;
            var p = Convert.ChangeType(parameter, mysqlp);
            parameter.Value = GetDbValue(value);
            if (parameter.Value == null || parameter.Value == DBNull.Value) return;
            Type type = value.GetType();
            PropertyInfo pryinfo = mysqlp.GetProperty("NpgsqlDbType");

            if (parameter.DbType != DbType.Guid && type == typeof(Guid))
            {
                parameter.DbType = DbType.String;
                parameter.Size = 36;
                return;
            }
            if (parameter.DbType == DbType.String)
            {
                pryinfo.SetValue(p, PostgreSqlDbType["TEXT"], null);
                parameter.Value = value.ToString();

                return;
            }
            if (parameter.DbType == DbType.Object)
            {
                parameter.DbType = DbType.String;
                parameter.Value = ExpressionHelper.Serialize(value);
                return;
            }
            if (parameter.Value.GetType().IsEnum && parameter.DbType == DbType.Int32)
            {
                parameter.Value = (int)parameter.Value;
                return;
            }
        }

        public override string GetSelectLastInsertAutoIncrementIDSql(string tableName, string columnName, Dictionary<string, string> options)
        {
            return string.Format("SELECT currval('\"{0}_{1}_seq\"')", tableName, columnName);
        }

        public override bool SupportMultiSqlStatementInOneCommand
        {
            get
            {
                return false;
            }
        }

        public override bool SupportADO20Transaction
        {
            get
            {
                return false;
            }
        }

    }
}
