using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
using System.Runtime.Serialization;
using NSun.Data.Configuration;

namespace NSun.Data
{
    [Serializable]
    [DataContract(Namespace = "http://nsun-shadow.com")]
    [KnownType("KnownTypes")]
    public class CustomSqlSection : QueryCriteria, IParameterConditions
    {
        #region Propertys & Field

        [DataMember]
        private readonly List<ParameterEqualsCondition> _parameterConditions;

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

        public CustomSqlSection(string sql)
        {
            _parameterConditions = new List<ParameterEqualsCondition>();
            _queryType = QueryType.Custom;
            Sql = sql;
        }

        protected internal CustomSqlSection(Database db, string sql)
            : this(sql)
        {
            Db = db;
        }

        #endregion

        #region Public Method

        public CustomSqlSection AddInputParameter(string name, DbType type, object value)
        {
            var pe = new ParameterExpression(name, SprocParameterDirection.Input, type);
            _parameterConditions.Add(pe == value);
            return this;
        }

        protected internal CustomSqlSection AddInputParameter(params ParameterEqualsCondition[] parameterConditions)
        {
            if (parameterConditions != null)
            {
                _parameterConditions.AddRange(parameterConditions);
            }
            return this;
        }

        public SubQuery ToSubQuery()
        {
            DbCommand cmd = Db.CommandBuilder.BuildCommandSprocOrCustom(this);
            var expr = new SubQuery { Sql = "(" + cmd.CommandText + ")" };
            foreach (DbParameter parameter in cmd.Parameters)
            {
                expr.ChildExpressions.Add(new ParameterExpression(parameter.ParameterName, parameter.Value,
                                                                  parameter.DbType));
            }
            return expr;
        }

        public SubQuery ToSubQuery(Database db)
        {
            DbCommand cmd = db.CommandBuilder.BuildCommandSprocOrCustom(this);
            var expr = new SubQuery { Sql = "(" + cmd.CommandText + ")" };
            foreach (DbParameter parameter in cmd.Parameters)
            {
                expr.ChildExpressions.Add(new ParameterExpression(parameter.ParameterName, parameter.Value,
                                                                  parameter.DbType));
            }
            return expr;
        }

        public override object Clone()
        {
            var clone = new CustomSqlSection(Db, Sql);
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
