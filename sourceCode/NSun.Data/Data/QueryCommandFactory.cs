using System;
using System.Data.Common;
using System.Data;
using System.Runtime.InteropServices;

namespace NSun.Data
{
    public class QueryCommandFactory
    {
        #region Member

        public QueryCommandBuilder CommandBuilder { get; set; }

        #endregion

        #region Construction

        public QueryCommandFactory(QueryCommandBuilder cmdBuilder)
        {
            CommandBuilder = cmdBuilder;
        }

        #endregion

        #region Public Methods

        public DbCommand CreateCommand(QueryCriteria criteria, bool isCountCommand)
        {
            if (criteria == null)
                throw new ArgumentNullException("criteria");

            DbCommand cmd;
            if (criteria.QueryType == QueryType.Sproc)
            {
                cmd = CommandBuilder.GetDbProviderFactory().CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = criteria.TableName;
            }
            else if (criteria.QueryType == QueryType.Custom)
            {
                cmd = CommandBuilder.GetDbProviderFactory().CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = criteria.TableName;
            }
            else
            {
                cmd = CommandBuilder.BuildCommand(criteria, isCountCommand);
            } 
            if (criteria.QueryType == QueryType.Sproc || criteria.QueryType == QueryType.Custom)
            {
                var cri = criteria as IParameterConditions;
                var sprocCmd = new SprocDbCommand(cmd, CommandBuilder);
                foreach (var parameterCondition in cri.ParameterConditions)
                {
                    sprocCmd.AddParameter(parameterCondition);
                }
                return sprocCmd.Command;
            }
            return cmd;
        }

        #endregion
    }
}
