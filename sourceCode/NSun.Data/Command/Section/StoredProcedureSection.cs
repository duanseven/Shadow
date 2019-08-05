using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using NSun.Data.Configuration;


namespace NSun.Data
{
    [Serializable]
    [DataContract(Namespace = "http://nsun-shadow.com")]
    [KnownType("KnownTypes")]
    public class StoredProcedureSection : QueryCriteria, IParameterConditions
    {
        #region Property & Field

        [DataMember]
        private readonly List<ParameterEqualsCondition> _parameterConditions;

        [DataMember]
        internal string Sql
        {
            get { return TableName; }
            set { TableName = value; }
        }

        public ReadOnlyCollection<ParameterEqualsCondition> ParameterConditions
        {
            get { return new ReadOnlyCollection<ParameterEqualsCondition>(_parameterConditions); }
        }

        #endregion

        #region Construction

        public StoredProcedureSection(string sql)
        {
            _parameterConditions = new List<ParameterEqualsCondition>();
            _queryType = QueryType.Sproc;
            Sql = sql;
        }

        public StoredProcedureSection(Database db, string sprocname)
            : this(sprocname)
        {
            Db = db;
        }

        #endregion

        #region Public Methods

        public StoredProcedureSection AddInputParameter(string name, DbType type, object value)
        {
            var pe = new ParameterExpression(name, SprocParameterDirection.Input, type);
            _parameterConditions.Add(pe == value);
            return this;
        }

        public StoredProcedureSection AddOutputParameter(string name, DbType type, int size)
        {
            var pe = new ParameterExpression(name, SprocParameterDirection.Output, type, size);
            _parameterConditions.Add(pe == null);
            return this;
        }

        public StoredProcedureSection AddInputOutputParameter(string name, DbType type, object value, int size)
        {
            var pe = new ParameterExpression(name, SprocParameterDirection.InputOutput, type, size);
            _parameterConditions.Add(pe == value);
            return this;
        }

        public StoredProcedureSection AddReturnValueParameter(string name, DbType type, int size)
        {
            var pe = new ParameterExpression(name, SprocParameterDirection.ReturnValue, type, size);
            _parameterConditions.Add(pe == null);
            return this;
        }

        public void AddParameters(params ParameterEqualsCondition[] parameterConditions)
        {
            if (parameterConditions != null)
            {
                _parameterConditions.AddRange(parameterConditions);
            }
        }

        public object GetOutputParameterValue(DbCommand cmd, string sprocParamname)
        {
            foreach (DbParameter p in cmd.Parameters)
            {
                if (p.Direction != ParameterDirection.Input
                    && string.Compare(
                       p.ParameterName,
                       Db.CommandBuilder.ToParameterName(sprocParamname),
                       StringComparison.OrdinalIgnoreCase) == 0)
                {
                    if (p.Value == DBNull.Value)
                        return null;
                    return p.Value;
                }
            }
            return null;
        }

        public Dictionary<string, object> GetOutputParameterValues(DbCommand cmd)
        {
            var outValues = new Dictionary<string, object>();
            for (int i = 0; i < cmd.Parameters.Count; ++i)
            {
                if (cmd.Parameters[i].Direction == ParameterDirection.InputOutput || cmd.Parameters[i].Direction == ParameterDirection.Output || cmd.Parameters[i].Direction == ParameterDirection.ReturnValue)
                {
                    //outValues.Add(cmd.Parameters[i].ParameterName.Substring(1, cmd.Parameters[i].ParameterName.Length - 1),                    
                    outValues.Add(cmd.Parameters[i].ParameterName,
                        cmd.Parameters[i].Value == DBNull.Value ? null : cmd.Parameters[i].Value);
                }
            }
            return outValues;
        }

        public override string ToDbCommandText()
        {
            StringBuilder sb = new StringBuilder(" ");
            foreach (var parameterEqualsCondition in ParameterConditions)
            {
                sb.Append("@");
                sb.Append(parameterEqualsCondition.LeftExpression.ID);
                sb.Append(" ");
                if (parameterEqualsCondition.RightExpression != null && parameterEqualsCondition.RightExpression.Value != null)
                    sb.Append(parameterEqualsCondition.RightExpression.Value);
                else
                    sb.Append("NULL");
                sb.Append(",");
            }
            return Sql + sb.ToString().TrimEnd(',');
        }

        public override object Clone()
        {
            var clone = new StoredProcedureSection(Db, Sql);
            foreach (var assignment in ParameterConditions)
            {
                clone._parameterConditions.Add(assignment);
            }
            clone.IdentyColumnName = IdentyColumnName;
            clone.IdentyColumnIsNumber = IdentyColumnIsNumber;
            return clone;
        }

        #endregion

        #region KnownTypes

        static Type[] KnownTypes()
        {
            return KnownTypeRegistry.Instance.KnownTypes;
        }

        #endregion
    }
}
