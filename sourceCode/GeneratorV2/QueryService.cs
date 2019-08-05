using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Globalization;
using NSun.Data;

namespace GeneratorEntity
{

    public interface IQueryService
    { 
        DataTable Query(QueryCriteria criteria);
 
        int Execute(QueryCriteria criteria, bool isCountQuery);
    }
    
    public sealed class QueryService : IQueryService
    {
        private readonly DBQuery _cmdFac; 

        public QueryService(DBQuery cmdFac)
        {
            if (cmdFac == null)
                throw new ArgumentNullException("cmdFac");

            _cmdFac = cmdFac;
        }

        #region IQueryService Members

        public DataTable Query(QueryCriteria criteria)
        {
            var cmd = _cmdFac.PrepareCommand(criteria,true);
            using (var conn = cmd.Connection)
            {
                if (conn.State != ConnectionState.Open)
                    conn.Open();
                var result = new DataTable(criteria.TableName) { Locale = CultureInfo.InvariantCulture };
                var adapter = _cmdFac.CommandFactory.CommandBuilder.GetDbProviderFactory().CreateDataAdapter();
                adapter.SelectCommand = cmd;
                adapter.FillSchema(result,SchemaType.Source);
                return result;
            }
        }


        internal DataColumn QuerySchemaIncrement(QueryCriteria criteria)
        {
            var cmd = _cmdFac.PrepareCommand(criteria, true);  
            using (var conn = cmd.Connection)
            {
                if (conn.State != ConnectionState.Open)
                    conn.Open();
                var result = new DataTable(criteria.TableName) { Locale = CultureInfo.InvariantCulture };
                var adapter = _cmdFac.CommandFactory.CommandBuilder.GetDbProviderFactory().CreateDataAdapter();
                adapter.SelectCommand = cmd;
                adapter.FillSchema(result, SchemaType.Source);
                foreach (DataColumn item in result.Columns)
                {
                    if (item.AutoIncrement)
                    {
                        return item;
                    }
                }
                return null;
            }
        }

        internal DataColumn QuerySchemaPrimary(QueryCriteria criteria)
        {
            var cmd = _cmdFac.PrepareCommand(criteria, true);  
            using (var conn = cmd.Connection)
            {
                if (conn.State != ConnectionState.Open)
                    conn.Open();
                var result = new DataTable(criteria.TableName) { Locale = CultureInfo.InvariantCulture };
                var adapter = _cmdFac.CommandFactory.CommandBuilder.GetDbProviderFactory().CreateDataAdapter();
                adapter.SelectCommand = cmd;
                adapter.FillSchema(result, SchemaType.Source);
                if (result.PrimaryKey.Length>0)
                {
                    return result.PrimaryKey[0];
                }
                return null;
            }
        } 

        public int Execute(QueryCriteria criteria, bool isCountQuery)
        {
            var cmd = _cmdFac.PrepareCommand(criteria, true);
            using (var conn = cmd.Connection)
            {
                if (conn.State != ConnectionState.Open)
                    conn.Open();

                return isCountQuery ? Convert.ToInt32(cmd.ExecuteScalar()) : cmd.ExecuteNonQuery();
            }
        }

        #endregion
    
    }
}
