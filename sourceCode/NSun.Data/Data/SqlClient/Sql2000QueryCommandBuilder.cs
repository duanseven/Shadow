using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;

namespace NSun.Data.SqlClient
{
    /// <summary>
    /// The IQueryCommandBuilder implementation for SQL Server database.
    /// </summary>
    public class Sql2000QueryCommandBuilder : QueryCommandBuilder
    {
        /// <summary>
        /// The singleton.
        /// </summary>
        public static readonly Sql2000QueryCommandBuilder Instance;

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlQueryCommandBuilder"/> class.
        /// </summary>
        protected Sql2000QueryCommandBuilder()
        {
        }

        static Sql2000QueryCommandBuilder()
        {
            Instance = new Sql2000QueryCommandBuilder();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the db provider factory.
        /// </summary>
        /// <returns></returns>
        public override DbProviderFactory GetDbProviderFactory()
        {            
            return SqlClientFactory.Instance;
        }

        #endregion

        protected bool OrderyColumn(SelectSqlSection select)
        {
            foreach (var sortby in select.SortBys.Keys)
            {
                if (select.IdentyColumnName == sortby.Sql)
                {
                    return true;
                }
            }
            return false;
        }

        protected bool OrderyColumnMaxMin(SelectSqlSection select)
        {
            foreach (var sortby in select.SortBys.Keys)
            {
                if (select.IdentyColumnName == sortby.Sql)
                {
                    return select.SortBys[sortby];
                }
            }
            return false;
        }

        #region Non-Public Methods

        /// <summary>
        /// Builds the paging cacheable SQL.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns></returns>
        protected override string BuildPagingCacheableSql(QueryCriteria criteria)
        {
            var select = criteria as SelectSqlSection;
            StringBuilder sb = new StringBuilder(" SELECT ");
            if (select.Groups.Count == 0)
            {
                if (select.SortBys.Count == 0)
                {
                    return BuildPagingCacheableMaxMinSql(select);
                }
                else if ((select.SortBys.Count == 1 && OrderyColumn(select)))
                {
                    return BuildPagingCacheableNotInSql(select);
                }
                else
                {
                    //DECLARE @PAGETEMP TABLE
                    //(
                    //    __ROW_NUM INT IDENTITY(1,1),
                    //    __TID INT
                    //)
                    //INSERT INTO @PAGETEMP(__TID) SELECT TOP 30 OrderID FROM dbo.Orders  order by RequiredDate
                    //SELECT [@PAGETEMP].__ROW_NUM,Orders.* FROM Orders,@PAGETEMP WHERE dbo.Orders.OrderID= [@PAGETEMP].__TID
                    //AND [@PAGETEMP].__ROW_NUM>20 AND [@PAGETEMP].__ROW_NUM<=30 
                    sb = new StringBuilder("DECLARE @PAGETEMP TABLE ( __ROW_NUM INT IDENTITY(1,1),__TID INT )");
                    sb.Append("INSERT INTO @PAGETEMP(__TID) SELECT ");
                    if (select.IsDistinct)
                    {
                        sb.Append("DISTINCT ");
                    }
                    if (select.MaxResults + select.SkipResults > 0)
                    {
                        sb.Append("TOP ");
                        sb.Append(select.MaxResults + select.SkipResults);
                        sb.Append(' ');
                    }
                    sb.Append(select.IdentyColumnName);
                    sb.Append(" FROM ");
                    sb.Append(select.ToString());
                    sb.Append(";SELECT ");
                    sb.Append(AppendResultColumns(criteria));
                    sb.Append(",[@PAGETEMP].__ROW_NUM ");
                    sb.Append(" FROM @PAGETEMP,");
                    sb.Append(select.TableName);
                    sb.Append(" WHERE ");
                    sb.Append(select.IdentyColumnName + " =[@PAGETEMP].__TID AND");
                    sb.Append(" [@PAGETEMP].__ROW_NUM>");
                    sb.Append(select.SkipResults);
                    if (select.MaxResults + select.SkipResults > 0)
                    {
                        sb.Append(" AND [@PAGETEMP].__ROW_NUM<=");
                        sb.Append(select.MaxResults + select.SkipResults);
                        sb.Append(' ');
                    }
                    return sb.ToString();
                }
            }
            if (select.IsDistinct)
            {
                sb.Append("DISTINCT ");
            }
            if (select.MaxResults + select.SkipResults > 0)
            {
                sb.Append("TOP ");
                sb.Append(select.MaxResults + select.SkipResults);
                sb.Append(' ');
            }
            sb.Append(" IDENTITY(INT) __ROW_NUM ,");
            sb.Append(AppendResultColumns(criteria));
            sb.Append("  INTO #PAGETEMP");
            sb.Append(" FROM ");
            sb.Append(criteria.ToString());
            sb.Append(";SELECT *");
            sb.Append(" FROM #PAGETEMP");
            sb.Append(" WHERE ");
            sb.Append(" [#PAGETEMP].__ROW_NUM>");
            sb.Append(select.SkipResults);
            if (select.MaxResults + select.SkipResults > 0)
            {
                sb.Append(" AND [#PAGETEMP].__ROW_NUM<=");
                sb.Append(select.MaxResults + select.SkipResults);
                sb.Append(' ');
            }
            sb.Append(";DROP TABLE #PAGETEMP ");
            return sb.ToString();
        }

        protected string BuildPagingCacheableMaxMinSql(SelectSqlSection select)
        {

            //SELECT TOP 10 *
            //FROM TestTable
            //WHERE (ID >
            //          (SELECT MAX(id)/MIN(id)
            //         FROM (SELECT TOP 20 id
            //                 FROM TestTable
            //                 ORDER BY id) AS T))
            //ORDER BY ID
            bool isIdentyColumnDesc = false;

            if (select.SortBys.Count > 0)
            {
                isIdentyColumnDesc = OrderyColumnMaxMin(select);
            }

            StringBuilder sb = new StringBuilder("SELECT ");
            if (select.IsDistinct)
            {
                sb.Append("DISTINCT ");
            }
            if (select.MaxResults > 0)
            {
                sb.Append("TOP ");
                sb.Append(select.MaxResults);
                sb.Append(" ");
            }
            AppendResultColumns(select, sb);
            sb.Append(" FROM ");
            sb.Append(select.From.ToString());
            SelectSqlSection cloneWhere = (SelectSqlSection)select.Clone();

            #region Construct & extend CloneWhere

            StringBuilder sbInside = new StringBuilder();
            sbInside.Append(cloneWhere.IdentyColumnName);
            sbInside.Append(isIdentyColumnDesc ? '<' : '>');
            sbInside.Append('(');
            sbInside.Append("SELECT ");
            sbInside.Append(isIdentyColumnDesc ? "MIN(" : "MAX(");
            sbInside.Append("[__T].");
            string[] splittedIdentyColumn = cloneWhere.IdentyColumnName.Split('.');
            sbInside.Append(splittedIdentyColumn[splittedIdentyColumn.Length - 1]);
            sbInside.Append(") FROM (SELECT TOP ");
            sbInside.Append(cloneWhere.SkipResults);
            sbInside.Append(' ');
            sbInside.Append(cloneWhere.IdentyColumnName);
            sbInside.Append(" AS ");
            sbInside.Append(splittedIdentyColumn[splittedIdentyColumn.Length - 1]);
            sbInside.Append(" FROM ");
            sbInside.Append(cloneWhere.ToString());
            if (cloneWhere.SortBys.Count == 0)
            {
                sbInside.Append(" ORDER BY " + select.IdentyColumnName + (isIdentyColumnDesc ? " DESC " : " ASC "));
            }
            sbInside.Append(") [__T])");
 
            sb.Append(" WHERE ");            
            if (cloneWhere.ConditionWhere.LinkedConditions.Count == 0)
            {
                sb.Append(sbInside.ToString());
            }
            else
            {
                StringBuilder strsb = new StringBuilder();
                AppendConditions(select, strsb);
                sb.Append("(" + strsb.ToString() + ") AND " + sbInside.ToString());
                select.ConditionWhere.LinkedConditions.AddRange(cloneWhere.ConditionWhere.LinkedConditions);
            }

            #endregion

            if (select.SortBys.Count == 0)
            {
                sb.Append(" ORDER BY " + select.IdentyColumnName + (isIdentyColumnDesc ? " DESC " : " ASC "));
            }

            return sb.ToString();
        }

        protected string BuildPagingCacheableNotInSql(SelectSqlSection select)
        {

            //SELECT TOP 10 *
            //FROM TestTable
            //WHERE (ID NOT IN
            //          (SELECT TOP 20 id
            //         FROM TestTable
            //         ORDER BY id))
            //ORDER BY ID
            StringBuilder sb = new StringBuilder("SELECT ");
            if (select.IsDistinct)
            {
                sb.Append("DISTINCT ");
            }
            if (select.MaxResults > 0)
            {
                sb.Append("TOP ");
                sb.Append(select.MaxResults);
                sb.Append(' ');
            }
            AppendResultColumns(select, sb);
            sb.Append(" FROM ");
            sb.Append(select.From.ToString());

            SelectSqlSection cloneWhere = (SelectSqlSection)select.Clone();

            #region Construct & extend CloneWhere

            StringBuilder sbInside = new StringBuilder();
            sbInside.Append(cloneWhere.IdentyColumnName);
            sbInside.Append(" NOT IN (SELECT TOP ");
            sbInside.Append(cloneWhere.SkipResults);
            sbInside.Append(' ');
            sbInside.Append(cloneWhere.IdentyColumnName);
            sbInside.Append(" FROM ");
            sbInside.Append(cloneWhere.ToString());
            sbInside.Append(")");

            sb.Append(" WHERE ");
            if (cloneWhere.ConditionWhere.LinkedConditions.Count == 0)
            {
                sb.Append(sbInside.ToString());
            }
            else
            {
                var sbwhere = new StringBuilder();
                AppendConditions(select, sbwhere);
                sb.Append("(" + sbwhere.ToString() + ") AND " + sbInside.ToString());
                select.ConditionWhere.LinkedConditions.AddRange(cloneWhere.ConditionWhere.LinkedConditions);
            }

            #endregion
 
            sb.Append(" ORDER BY ");
            AppendSortBys(select, sb);

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
                return BuildCountCacheableSql(select);
            var sb = new StringBuilder();
            sb.Append("SELECT ");
            if (select.IsDistinct)
            {
                sb.Append("DISTINCT ");
            }
            if (select.MaxResults > 0)
            {
                sb.Append("TOP ");
                sb.Append(select.MaxResults);
                sb.Append(" ");
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

            return "@" + name.TrimStart('@');
        }


        /// <summary>
        /// Adjusts the parameter properties.
        /// </summary>
        /// <param name="parameterExpr">The parameter expr.</param>
        /// <param name="parameter">The parameter.</param>
        protected override void AdjustParameterProperties(IParameterExpression parameterExpr, DbParameter parameter)
        {
            var sqlParameter = parameter as SqlParameter;
            parameter.Value = GetDbValue(parameter.Value);
            if (parameter.Value == null || parameter.Value == DBNull.Value) return;
  
            if (parameter.DbType == DbType.Binary)
            {
                sqlParameter.SqlDbType = ((byte[])parameter.Value).Length > 8000
                                             ? SqlDbType.Image
                                             : SqlDbType.VarBinary;
                return; 
            }          
            if (parameter.DbType == DbType.Date || parameter.DbType == DbType.DateTime || parameter.DbType == DbType.Time)
            {   
                sqlParameter.SqlDbType = SqlDbType.DateTime;
                if (sqlParameter.SqlDbType == SqlDbType.DateTime && parameter.Value.GetType() == typeof(TimeSpan))
                {
                    sqlParameter.Value = new DateTime(1900, 1, 1).Add((TimeSpan)parameter.Value);
                    return;
                }
                return;
            } 
            if (parameter.DbType == DbType.Guid)
            {
                sqlParameter.SqlDbType = SqlDbType.UniqueIdentifier;

                if (parameter.Value.GetType() != typeof(Guid))
                {
                    parameter.Value = new Guid(parameter.Value.ToString());
                }
                return;
            }
            if (parameter.DbType == DbType.AnsiString)
            {
                sqlParameter.SqlDbType = parameter.Value.ToString().Length > 8000 ? SqlDbType.Text : SqlDbType.VarChar;
                return;
            }
            if (parameter.DbType == DbType.String)
            {
                sqlParameter.SqlDbType = parameter.Value.ToString().Length > 4000 ? SqlDbType.NText : SqlDbType.NVarChar;
                return;
            }           
            if (parameter.DbType == DbType.Object)
            {
                sqlParameter.SqlDbType = SqlDbType.NText;
                parameter.Value = ExpressionHelper.Serialize(parameter.Value);
                return;
            }
            if (parameter.Value.GetType().IsEnum && parameter.DbType == DbType.Int32)
            {
                parameter.Value = (int) parameter.Value;
                return;
            }
        }

        /// <summary>
        /// Gets the database object name quote characters.
        /// </summary>
        /// <returns></returns>
        protected override string GetDatabaseObjectNameQuoteCharacters()
        {
            return "[]";
        }

        public override string GetSelectLastInsertAutoIncrementIDSql(string tableName, string columnName, Dictionary<string, string> options)
        {
            return "SELECT SCOPE_IDENTITY()";
        }

        public override bool SupportADO20Transaction
        {
            get
            {
                return true;
            }
        }

        public override bool SupportMultiSqlStatementInOneCommand
        {
            get
            {
                return true;
            }
        } 

        #endregion
    }
}
