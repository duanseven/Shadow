using System;
using System.Runtime.Serialization;
using NSun.Data.Configuration;
using System.Text;


namespace NSun.Data
{
    [Serializable]
    [DataContract(Namespace = "http://nsun-shadow.com")]
    [KnownType("KnownTypes")]
    public class CustomWithQueryTable : CustomQueryTable
    {
        #region Construction

        public CustomWithQueryTable(string subSql, string asName)
        {
            var sb = new StringBuilder();
            sb.Append("WITH ");
            sb.Append(asName);
            sb.Append(" AS ");
            sb.Append("(");
            sb.Append(subSql);
            sb.Append(")");
            AsName = asName;
            _tableName = sb.ToString();
        }

        public CustomWithQueryTable(SubQuery subQuery, string asName)
        {
            var sb = new StringBuilder();
            sb.Append("WITH ");
            sb.Append(asName);
            sb.Append(" AS ");
            sb.Append(subQuery.Sql);
            AsName = asName;
            _tableName = sb.ToString();
            if (subQuery.ChildExpressions.Count > 0)
            {
                this.ChildExpressions.AddRange(subQuery.ChildExpressions);
            }
        }

        #endregion

        #region KnownTypes

        static Type[] KnownTypes()
        {
            return KnownTypeRegistry.Instance.KnownTypes;
        }

        #endregion
    }
    [Serializable]
    [DataContract(Namespace = "http://nsun-shadow.com")]
    [KnownType("KnownTypes")]
    public class CustomQueryTable : ExpressionClip, IQueryTable
    {
        #region Propertys & Fields

        [DataMember]
        protected string _tableName;

        [DataMember]
        public string AsName { get; set; }

        public new string Sql
        {
            get
            {
                return _tableName;
            }
        }

        #endregion

        #region Construction

        protected CustomQueryTable()
        {
        }

        public CustomQueryTable(string subSql)
        {
            _tableName = subSql;
        }

        public CustomQueryTable(string subSql, string asName)
        {
            AsName = asName;
            _tableName = " (" + subSql + ") " + asName;
        }

        public CustomQueryTable(SubQuery subQuery)
        {
            _tableName = subQuery.Sql;
            if (subQuery.ChildExpressions.Count > 0)
            {
                this.ChildExpressions.AddRange(subQuery.ChildExpressions);
            }
        }

        public CustomQueryTable(SubQuery subQuery, string asName)
        {
            AsName = asName;
            _tableName = subQuery.Sql + asName;
            if (subQuery.ChildExpressions.Count > 0)
            {
                this.ChildExpressions.AddRange(subQuery.ChildExpressions);
            }
        }

        #endregion

        #region IQueryTable 成员

        public string GetTableName()
        {
            return _tableName;
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
