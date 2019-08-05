using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.OracleClient; 
using System.Text;
using System.Globalization;

namespace NSun.Data.OracleClient
{
    /// <summary>
    /// The IQueryCommandBuilder implementation for Oracle database.
    /// </summary> 
    public class OracleQueryCommandBuilder : QueryCommandBuilder
    {
        /// <summary>
        /// The singleton.
        /// </summary>
        public static readonly OracleQueryCommandBuilder Instance;

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="OracleQueryCommandBuilder"/> class.
        /// </summary>
        protected OracleQueryCommandBuilder()
        {
        }

        /// <summary>
        /// Initializes the <see cref="OracleQueryCommandBuilder"/> class.
        /// </summary>
        static OracleQueryCommandBuilder()
        {
            Instance = new OracleQueryCommandBuilder();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the db provider factory.
        /// </summary>
        /// <returns></returns>
        public override DbProviderFactory GetDbProviderFactory()
        {
            return OracleClientFactory.Instance;
        }

        #endregion

        #region Non-Public Methods

        /// <summary>
        /// Builds the paging cacheable SQL.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns></returns>
        protected override string BuildPagingCacheableSql(QueryCriteria criteria)
        {
            //select * from (select * from users order by id  ) where rownum < 11; //top 10
            //select * from  users where rownum < 11; //只能是小于 因为 rownum要从1开始叠加
            var select = criteria as SelectSqlSection;
            var sb = new StringBuilder();
            sb.Append("SELECT ");
            //AppendResultColumns(criteria, sb);
            sb.Append(" * ");
            sb.Append(" ");
            sb.Append("FROM ");
            sb.Append("(SELECT ");
            if (select.IsDistinct)
            {
                sb.Append("DISTINCT ");
            }
            sb.Append(AppendResultColumns(criteria));
            sb.Append(", ROW_NUMBER() OVER (ORDER BY ");
            if (select.SortBys.Count > 0)
            {
                AppendSortBys(criteria, sb);
            }
            else
            {
                sb.Append(criteria.IdentyColumnName);
            }
            sb.Append(") [__Pos] ");

            sb.Append(" FROM ");
            sb.Append(criteria.ToString());
            sb.Append(") [__T] WHERE [__T].[__Pos] > ");

            sb.Append(select.SkipResults);
            if (select.MaxResults > 0)
            {
                sb.Append(" ");
                sb.Append("AND [__T].[__Pos] <= ");
                sb.Append(select.MaxResults + select.SkipResults);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Builds the no paging cacheable SQL.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <param name="isCountCommand">if set to <c>true</c> [is count command].</param>
        /// <returns></returns>
        protected override string BuildNoPagingCacheableSql(QueryCriteria criteria, bool isCountCommand)
        {
            var select = criteria as SelectSqlSection;
            if (isCountCommand)
                return BuildCountCacheableSql(criteria);

            if (select.MaxResults > 0)
                return BuildPagingCacheableSql(criteria); 
            var sb = new StringBuilder();
            sb.Append("SELECT ");
            if (select.IsDistinct)
            {
                sb.Append("DISTINCT ");
            } 
            AppendResultColumns(select, sb);
            sb.Append(" ");
            sb.Append(" FROM ");
            sb.Append(select.ToString());
            return sb.ToString(); 
        }

        /// <summary>
        /// Format a name string to a parameter name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        internal protected override string ToParameterName(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");
            return ":" + name.TrimStart(':');
            //return name;
        }

        /// <summary>
        /// Adjusts the parameter properties.
        /// </summary>
        /// <param name="parameterExpr">The parameter expr.</param>
        /// <param name="parameter">The parameter.</param>
        protected override void AdjustParameterProperties(IParameterExpression parameterExpr, DbParameter parameter)
        {
            //RegisterColumnType(DbType.Boolean, "NUMBER(1,0)");
            //RegisterColumnType(DbType.Byte, "NUMBER(3,0)");
            //RegisterColumnType(DbType.Int16, "NUMBER(5,0)");
            //RegisterColumnType(DbType.Int32, "NUMBER(10,0)");
            //RegisterColumnType(DbType.Int64, "NUMBER(20,0)");
            var oracleParameter = parameter as OracleParameter;
            parameter.Value = GetDbValue(parameter.Value);

            if (parameter.Value == null || parameter.Value == DBNull.Value) return;

            if (oracleParameter.DbType != DbType.Guid && parameter.Value.GetType() == typeof(Guid))
            {
                oracleParameter.OracleType = OracleType.Char;
                oracleParameter.Size = 36;                
                return;
            }
            if (oracleParameter.DbType==DbType.Guid)
            {
                oracleParameter.OracleType = OracleType.Raw;
                oracleParameter.Size = 16;
                return;
            }

            if ((parameter.DbType == DbType.Time || parameter.DbType == DbType.DateTime) && parameter.Value is TimeSpan)
            {
                oracleParameter.OracleType = OracleType.Double;
                oracleParameter.Value = ((TimeSpan)parameter.Value).TotalDays;
                return;
            }

            if (parameter.DbType == DbType.AnsiStringFixedLength)
            {
                oracleParameter.OracleType = OracleType.Char; return;
            }
            if (parameter.DbType == DbType.AnsiString)
            {
                if (parameter.Value.ToString().Length> 2147483647)
                {
                    oracleParameter.OracleType = OracleType.Clob; return;
                }  
                oracleParameter.OracleType = OracleType.VarChar; return;
            }
            if (parameter.DbType == DbType.StringFixedLength)
            {
                oracleParameter.OracleType = OracleType.NChar; return;
            } 
            if (parameter.DbType == DbType.String)
            {
                if (parameter.Value.ToString().Length > 1073741823)
                {
                    oracleParameter.OracleType = OracleType.NClob; return;
                }  
                oracleParameter.OracleType = OracleType.NVarChar; return;
            }
            if (parameter.DbType == DbType.Boolean)
            {
                parameter.Value = (((bool)parameter.Value) ? 1 : 0);
                oracleParameter.OracleType = OracleType.Number; return;
            }
            if (parameter.DbType == DbType.Byte
                || parameter.DbType == DbType.Int16
                || parameter.DbType == DbType.Int32
                || parameter.DbType == DbType.Int64
                || parameter.DbType == DbType.UInt16
                || parameter.DbType == DbType.UInt32
                || parameter.DbType == DbType.UInt64
                || parameter.DbType ==DbType.Currency
                || parameter.DbType == DbType.Decimal
                )
            {                 
                oracleParameter.OracleType = OracleType.Number; return;
            }
            if (parameter.DbType == DbType.Single)
            {
                oracleParameter.OracleType = OracleType.Float; return;
            }
            if (parameter.DbType == DbType.Double)
            {                 
                oracleParameter.OracleType = OracleType.Double; return;
            } 
            if (parameter.DbType == DbType.Date || parameter.DbType == DbType.DateTime || parameter.DbType == DbType.Time)
            {
                oracleParameter.OracleType = OracleType.DateTime; return;
            }
            if (parameter.DbType == DbType.Binary)
            {
                if (((byte[])parameter.Value).Length > 2000)
                    {
                        oracleParameter.OracleType = OracleType.Blob;
                        return;
                    }  
            }
            if (parameter.DbType == DbType.Object && parameter.Direction == ParameterDirection.Output)
            {
                oracleParameter.OracleType = OracleType.Cursor;
                return;
            }
            if (parameter.DbType == DbType.Object)
            {
                oracleParameter.OracleType = OracleType.NClob;
                parameter.Value = ExpressionHelper.Serialize(parameter.Value);
                return;
            }

            if (parameter.Value.GetType().IsEnum && parameter.DbType == DbType.Int32)
            {
                parameter.Value = (int)parameter.Value;
                return;
            }
            return;
        }

        /// <summary>
        /// Gets the database object name quote characters.
        /// </summary>
        /// <returns></returns>
        protected override string GetDatabaseObjectNameQuoteCharacters()
        {
            return "\"\"";
        }

        private static string BuildCountCacheableSql(QueryCriteria criteria)
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
                sb.Append(" ) [__T]");

            return sb.ToString(); 
        }

        public override string GetSelectLastInsertAutoIncrementIDSql(string tableName, string columnName, Dictionary<string, string> options)
        {
            if (options != null && options.Count > 0)
            {
                Dictionary<string, string>.Enumerator en = options.GetEnumerator();
                en.MoveNext();
                if (!string.IsNullOrEmpty(en.Current.Value))
                {
                    return string.Format("SELECT {0}.nextval FROM DUAL", en.Current.Value);
                }
            }
 
            return string.Format("SELECT SEQ_{0}_{1}.nextval FROM DUAL", tableName, columnName);
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
        #endregion
    }
}
