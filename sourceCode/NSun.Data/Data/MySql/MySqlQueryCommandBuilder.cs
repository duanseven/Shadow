using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Text;
using NSun.Data.SqlClient;

namespace NSun.Data.MySql
{
    public class MySqlQueryCommandBuilder : QueryCommandBuilder
    {
        private static Assembly asm;
       
        private static Dictionary<string, object> MySqlDbType = new Dictionary<string, object>();

        protected MySqlQueryCommandBuilder()
        {
            asm = Assembly.Load("MySql.Data");
            var mysqldbtype = asm.GetType("MySql.Data.MySqlClient.MySqlDbType");
            FieldInfo[] fields =
                mysqldbtype.GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
            MySqlDbType.Clear();             
            foreach (var item in fields)
            {
                var o = item.GetCustomAttributes(typeof(ObsoleteAttribute), false);
                if (o.Length > 0)
                {
                    continue;
                }
                if (!MySqlDbType.ContainsKey(item.Name.ToUpper()))
                {
                    MySqlDbType.Add(item.Name.ToUpper(), item.GetValue(mysqldbtype));
                }
            }            
        }

        static MySqlQueryCommandBuilder()
        {
            Instance = new MySqlQueryCommandBuilder(); 
        }

        public static readonly MySqlQueryCommandBuilder Instance;

        public override System.Data.Common.DbProviderFactory GetDbProviderFactory()
        {
            Type type = asm.GetType("MySql.Data.MySqlClient.MySqlClientFactory");
            FieldInfo field = type.GetField("Instance");
            return field.GetValue(null) as DbProviderFactory; 
        }

        protected internal override string ToParameterName(string name)
        { 
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");
            return "?" + name.TrimStart('?');            
        }

        protected override string GetDatabaseObjectNameQuoteCharacters()
        {
            return "``";
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
                sb.Append("," + select.MaxResults);
            }
            return sb.ToString();
        } 

        protected override void AdjustParameterProperties(IParameterExpression parameterExpr, System.Data.Common.DbParameter parameter)
        {
            var mysqlp = asm.GetType("MySql.Data.MySqlClient.MySqlParameter");
            object value = parameter.Value;
            var p = Convert.ChangeType(parameter, mysqlp);
            parameter.Value = GetDbValue(value);
            if (parameter.Value == null || parameter.Value == DBNull.Value) return;
            Type type = value.GetType();
            PropertyInfo pryinfo = mysqlp.GetProperty("MySqlDbType");
            PropertyInfo prysizeinfo = mysqlp.GetProperty("Size");
            PropertyInfo pryvalueinfo = mysqlp.GetProperty("Value");

            if (parameter.DbType == DbType.AnsiString || parameter.DbType == DbType.String)
            {
                if (value.ToString().Length > 65535)
                {
                    pryinfo.SetValue(p, MySqlDbType["LONGTEXT"], null); return;
                }
            }
            if (parameter.DbType == DbType.Object)
            { 
                pryinfo.SetValue(p, MySqlDbType["LONGTEXT"], null);
                pryvalueinfo.SetValue(p, ExpressionHelper.Serialize(value), null);
            }
            if (parameter.DbType == DbType.Binary)
            {
                if (((byte[])value).Length > 2000)
                {
                    pryinfo.SetValue(p, MySqlDbType["LONGBLOB"], null); return;
                }
            } 
            if ((parameter.DbType == DbType.Time || parameter.DbType == DbType.DateTime) && type == typeof(TimeSpan))
            {                
                pryinfo.SetValue(p, MySqlDbType["DOUBLE"], null);
                pryvalueinfo.SetValue(p, ((TimeSpan) value).TotalDays, null);
                return;
            }
            if (parameter.DbType==DbType.Time||parameter.DbType==DbType.DateTime)
            {
                pryinfo.SetValue(p, MySqlDbType["DATETIME"], null);
                return;
            }
            if (parameter.DbType != DbType.Guid && type == typeof(Guid))
            {
                pryinfo.SetValue(p, MySqlDbType["VARCHAR"], null);
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
            return "SELECT LAST_INSERT_ID()";
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
