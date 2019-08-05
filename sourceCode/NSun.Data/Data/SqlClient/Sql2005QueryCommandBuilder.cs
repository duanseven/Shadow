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
    public class SqlQueryCommandBuilder : Sql2000QueryCommandBuilder
    {
        /// <summary>
        /// The singleton.
        /// </summary>
        public static readonly SqlQueryCommandBuilder Instance;

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlQueryCommandBuilder"/> class.
        /// </summary>
        protected SqlQueryCommandBuilder()
        {
        }

        static SqlQueryCommandBuilder()
        {
            Instance = new SqlQueryCommandBuilder();
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
            var select = criteria as SelectSqlSection;
            var sb = new StringBuilder();

            sb.Append("WITH [__T] AS (SELECT ");
            if (select.IsDistinct)
            {
                sb.Append("DISTINCT ");
            }
            if (select.MaxResults + select.SkipResults > 0)
            {
                sb.Append("TOP ");
                sb.Append(select.MaxResults + select.SkipResults);
                sb.Append(" ");
            }
            AppendResultColumns(criteria, sb);
            sb.Append(", ROW_NUMBER() OVER (ORDER BY ");
            if (select.SortBys.Count > 0)
            {
                AppendSortBys(criteria, sb);
            }
            else
            {
                sb.Append(criteria.IdentyColumnName);
            }
            sb.Append(") AS [__Pos] ");

            sb.Append(" FROM ");
            sb.Append(criteria.ToString());
            sb.Append(") SELECT ");
            sb.Append(" * ");
            sb.Append(" ");
            sb.Append("FROM [__T] WHERE [__T].[__Pos] > ");
            sb.Append(select.SkipResults);
            if (select.MaxResults > 0)
            {
                sb.Append(" ");
                sb.Append("AND [__T].[__Pos] <= ");
                sb.Append(select.MaxResults + select.SkipResults);
            }
            return sb.ToString();
        }

        protected override string BuildNoPagingCacheableSql(QueryCriteria criteria, bool isCountCommand)
        {
            var select = criteria as SelectSqlSection;
            if (object.ReferenceEquals(select.Table, null) || select.Table.GetType() == typeof(CustomQueryTable))
            {
                return base.BuildNoPagingCacheableSql(criteria, isCountCommand);
            }
            var cqt = select.Table as CustomWithQueryTable;
            var sb = new StringBuilder();
            sb.Append(cqt.Sql);
            sb.Append(base.BuildNoPagingCacheableSql(criteria, isCountCommand));           
            return sb.ToString();
        }

        /// <summary>
        /// Adjusts the parameter properties.
        /// </summary>
        /// <param name="parameterExpr">The parameter expr.</param>
        /// <param name="parameter">The parameter.</param>
        protected override void AdjustParameterProperties(IParameterExpression parameterExpr, DbParameter parameter)
        {
            base.AdjustParameterProperties(parameterExpr, parameter);
            var sqlParameter = parameter as SqlParameter; 
            if (parameter.DbType == DbType.String)
            {              
                sqlParameter.SqlDbType = SqlDbType.NVarChar; return;
            }
            if (parameter.DbType == DbType.AnsiString)
            {
                sqlParameter.SqlDbType = SqlDbType.VarChar; return;
            }
            if (parameter.DbType == DbType.Binary)
            {
                sqlParameter.SqlDbType = SqlDbType.VarBinary;
                return;
            }
            if (parameter.DbType == DbType.Xml)
            {
                sqlParameter.SqlDbType = SqlDbType.Xml;
                return;
            }
         
        } 

        #endregion
    }
}
