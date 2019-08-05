using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text; 

namespace NSun.Data.SqlClient
{
    public class Sql2008QueryCommandBuilder : SqlQueryCommandBuilder
    {
        public new static readonly Sql2008QueryCommandBuilder Instance;

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlQueryCommandBuilder"/> class.
        /// </summary>
        protected Sql2008QueryCommandBuilder()
        {
        }

        static Sql2008QueryCommandBuilder()
        {
            Instance = new Sql2008QueryCommandBuilder();
        }

        #endregion

        /// <summary>
        /// Adjusts the parameter properties.
        /// </summary>
        /// <param name="parameterExpr">The parameter expr.</param>
        /// <param name="parameter">The parameter.</param>
        protected override void AdjustParameterProperties(IParameterExpression parameterExpr, DbParameter parameter)
        {
            base.AdjustParameterProperties(parameterExpr, parameter);
            var sqlParameter = parameter as SqlParameter; 
            if (parameter.DbType == DbType.DateTime2)
            {
                sqlParameter.SqlDbType = SqlDbType.DateTime2; return;
            }
            if (parameter.DbType == DbType.DateTimeOffset)
            {
                sqlParameter.SqlDbType = SqlDbType.DateTimeOffset; return;
            }
            if (parameter.DbType == DbType.Date)
            {
                sqlParameter.SqlDbType = SqlDbType.Date;
                return;
            }
            if (parameter.DbType == DbType.Time)
            {
                sqlParameter.SqlDbType = SqlDbType.Time;
                return;
            }
          
        }
    }
}
