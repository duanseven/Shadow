using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.OracleClient;
using System.Linq;
using System.Text;

namespace NSun.Data.OracleClient
{
    public class Oracle9iQueryCommandBuilder : OracleQueryCommandBuilder
    {
          /// <summary>
        /// The singleton.
        /// </summary>
        public static readonly Oracle9iQueryCommandBuilder Instance;

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="OracleQueryCommandBuilder"/> class.
        /// </summary>
        protected Oracle9iQueryCommandBuilder()
        {
        }

        /// <summary>
        /// Initializes the <see cref="OracleQueryCommandBuilder"/> class.
        /// </summary>
        static Oracle9iQueryCommandBuilder()
        {
            Instance = new Oracle9iQueryCommandBuilder();
        }

        #endregion

        protected override void AdjustParameterProperties(IParameterExpression parameterExpr, DbParameter parameter)
        {
            base.AdjustParameterProperties(parameterExpr, parameter);
            var oracleParameter = parameter as OracleParameter; 

            if (parameter.DbType == System.Data.DbType.DateTime || parameter.DbType == System.Data.DbType.Time)
            {
                oracleParameter.OracleType = OracleType.Timestamp;
                oracleParameter.Size = 4;
                return;
            }
          
        }
    }
}
