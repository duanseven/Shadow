using System;
using System.Data.Common;
using System.Runtime.Serialization;
using NSun.Data.Configuration;
using NSun.Data.Helper;


namespace NSun.Data
{
    [Serializable]
    [DataContract(Namespace = "http://nsun-shadow.com")]
    [KnownType("KnownTypes")]
    public abstract class QueryCriteria : ICloneable
    {
        #region Member
         
        protected internal Database Db { get; set; }

        protected internal DbTransaction Tran { get; set; }

        [DataMember]
        protected string _tableName;

        [DataMember]
        protected internal QueryType _queryType;

        [DataMember]
        public string IdentyColumnName { get; set; }
    
        /// <summary>
        /// Key is Number
        /// </summary> 
        [DataMember]
        public bool IdentyColumnIsNumber { get; set; }        
         
        [DataMember]
        private Guid GuidQuery { get; set; }

        public string TableName
        {
            get { return _tableName; }
            internal set { _tableName = value; }
        }

        public QueryType QueryType
        {
            get { return _queryType; }
        }

        #endregion

        #region Events

        public event EventHandler Changed;

        #endregion

        #region Constructors

        protected QueryCriteria()
        {
            _queryType = QueryType.Select;
            IdentyColumnIsNumber = false;
            GuidQuery = Guid.NewGuid();
        }

        internal QueryCriteria(string tableNameorsprocorsql)
            : this()
        {
            if (string.IsNullOrEmpty(tableNameorsprocorsql))
                throw new ArgumentNullException("tableNameorsprocorsql");
            _tableName = tableNameorsprocorsql;
        }

        #endregion        

        #region Public Methods

        public QueryCriteria SetTransaction(DbTransaction tran)
        {
            Tran = tran;
            OnChanged();
            return this;
        }

        #endregion

        #region Non-Public Methods

        internal void OnChanged()
        {
            if (Changed != null)
                Changed(this, EventArgs.Empty);
        }

        #endregion

        #region ICloneable Members

        public abstract object Clone();

        #endregion

        #region OutPutCommand

        public virtual DbCommand ToDbCommand()
        {
            if (_queryType == QueryType.Sproc || _queryType == QueryType.Custom)
            {
                return Db.CommandBuilder.BuildCommandSprocOrCustom(this);
            }
            return Db.CommandBuilder.BuildCommand(this);
        }

        public virtual string ToDbCommandText()
        {
            return DataUtils.ToString(ToDbCommand());
        }

        #endregion

        #region Equal

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is QueryCriteria))
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            var item = (QueryCriteria)obj;

            if (item.IsTransient() || this.IsTransient())
                return false;
            else
                return item.GuidQuery == this.GuidQuery;
        }

        public override int GetHashCode()
        {
            return this.GuidQuery.GetHashCode();
        }

        private bool IsTransient()
        {
            return this.GuidQuery == Guid.Empty;
        }

        public static bool operator ==(QueryCriteria left, QueryCriteria right)
        {
            if (Object.Equals(left, null))
                return (Object.Equals(right, null)) ? true : false;
            else
                return left.Equals(right);
        }

        public static bool operator !=(QueryCriteria left, QueryCriteria right)
        {
            return !(left == right);
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
