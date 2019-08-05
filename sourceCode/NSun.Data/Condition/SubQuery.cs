

using System;
using System.Runtime.Serialization;
using NSun.Data.Configuration;


namespace NSun.Data
{
    [Serializable]
    [DataContract(Namespace = "http://nsun-shadow.com")]
    [KnownType("KnownTypes")]
    public class SubQuery : ExpressionClip
    {
        
        #region Construction

        public SubQuery(string sql)
        {
            Sql = "(" + sql + ")";
        }

        protected internal SubQuery(){}

        #endregion

        #region Public Methods

        public override string ToString()
        {
            return base.ToString();
        }         

        public new SubQuery As(string aliasName)
        {
            _sql.Append(' ');
            base.As(aliasName);
            return this;
        }

        public Condition Exists()
        {
            return new Condition(null, ExpressionOperator.Exists, this);
        }

        public Condition NotExists()
        {
            return Exists().Not();
        }

        public SelectSqlSection Select()
        {
            return new SelectSqlSection(new CustomQueryTable(this));
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
