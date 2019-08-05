using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common; 
using System.Reflection;
using System.Text; 

namespace NSun.Data.DB2
{
    public class DB2QueryCommandBuilder : QueryCommandBuilder
    {
        private static Assembly asm;

        private static Dictionary<string, object> DB2DbType = new Dictionary<string, object>();

        protected DB2QueryCommandBuilder()
        {
            asm = Assembly.Load("IBM.Data.DB2");
            var mysqldbtype = asm.GetType("IBM.Data.DB2.DB2Type");
            FieldInfo[] fields =
                mysqldbtype.GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
            DB2DbType.Clear();
            foreach (var item in fields)
            {
                if (!DB2DbType.ContainsKey(item.Name.ToUpper()))
                {
                    DB2DbType.Add(item.Name.ToUpper(), item.GetValue(mysqldbtype));
                }
            }
        }

        static DB2QueryCommandBuilder()
        {
            Instance = new DB2QueryCommandBuilder();
        }

        public static readonly DB2QueryCommandBuilder Instance;

        public override System.Data.Common.DbProviderFactory GetDbProviderFactory()
        {
            Type type = asm.GetType("IBM.Data.DB2.DB2Factory");
            FieldInfo field = type.GetField("Instance");
            return field.GetValue(null) as DbProviderFactory;
        }

        protected internal override string ToParameterName(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");
            return "@" + name.TrimStart('@');
        }

        protected override string GetDatabaseObjectNameQuoteCharacters()
        {
            return "\"\"";
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
            var select = criteria as SelectSqlSection;

            //page split algorithm using ROWNUMBER() 

            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT *");
            sb.Append(" FROM (");
            sb.Append("SELECT ");
            if (select.IsDistinct)
            {
                sb.Append("DISTINCT ");
            }
            sb.Append(AppendResultColumns(criteria));
            sb.Append(",ROWNUMBER() OVER (ORDER BY ");
            if (select.SortBys.Count > 0)
            {
                AppendSortBys(criteria, sb);
            }
            else
            {
                sb.Append(criteria.IdentyColumnName);
            }
            sb.Append(") AS [__Pos]");
            sb.Append(" FROM ");
            sb.Append(criteria.ToString());
            sb.Append(") [__T] WHERE [__T].[__Pos]>");

            sb.Append(select.SkipResults);

            if (select.MaxResults > 0)
            {
                sb.Append(" ");
                sb.Append("AND [__T].[__Pos] <= ");
                sb.Append(select.MaxResults + select.SkipResults);
            }

            return sb.ToString();
        }

        protected override void AdjustParameterProperties(IParameterExpression parameterExpr, System.Data.Common.DbParameter parameter)
        {
            var mysqlp = asm.GetType("IBM.Data.DB2.DB2Parameter");
            object value = parameter.Value;
            var p = Convert.ChangeType(parameter, mysqlp);
            parameter.Value = GetDbValue(value);
            if (parameter.Value == null || parameter.Value == DBNull.Value) return;
            Type type = value.GetType();
            PropertyInfo pryinfo = mysqlp.GetProperty("DB2Type");
            PropertyInfo prysizeinfo = mysqlp.GetProperty("Size");
            PropertyInfo pryvalueinfo = mysqlp.GetProperty("Value");

            if (parameter.DbType == DbType.AnsiString)
            {
                if (value.ToString().Length > 2000)
                {
                    pryinfo.SetValue(p, DB2DbType["CLOB"], null); return;
                }
            }
            if (parameter.DbType == DbType.String)
            {
                if (value.ToString().Length > 1000)
                {
                    pryinfo.SetValue(p, DB2DbType["DBCLOB"], null); return;
                }
            }
            if (parameter.DbType == DbType.Object)
            {
                pryinfo.SetValue(p, DB2DbType["DBCLOB"], null);
                pryvalueinfo.SetValue(p, ExpressionHelper.Serialize(value), null);
            }
            if (parameter.DbType == DbType.Binary)
            {
                if (((byte[])value).Length > 2000)
                {
                    pryinfo.SetValue(p, DB2DbType["BLOB"], null); return;
                }
            }

            if (parameter.DbType == DbType.Time)
            {
                pryinfo.SetValue(p, DB2DbType["TIMESTAMP"], null);
                return;
            }
            if (parameter.DbType == DbType.DateTime)
            {
                pryinfo.SetValue(p, DB2DbType["TIMESTAMP"], null);
                return;
            }
            if (parameter.DbType != DbType.Guid && type == typeof(Guid))
            {
                pryinfo.SetValue(p, DB2DbType["CHAR"], null);
                prysizeinfo.SetValue(p, 36, null);
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
            return "VALUES (IDENTITY_VAL_LOCAL())";
        }

        public override bool SupportMultiSqlStatementInOneCommand
        {
            get
            {
                return true;
            }
        }

        public override bool SupportADO20Transaction
        {
            get
            {
                return true;
            }
        }

    }
}
