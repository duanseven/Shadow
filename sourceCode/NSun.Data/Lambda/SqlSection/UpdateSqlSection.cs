using System; 
using System.Runtime.Serialization; 
using NSun.Data.Configuration;
using NSun.Data.Lambda;

namespace NSun.Data
{
    [Serializable]
    [DataContract(Namespace = "http://nsun-shadow.com")]
    [KnownType("KnownTypes")]
    public class UpdateSqlSection<TTable> : UpdateSqlSection where TTable : class,IBaseEntity
    {
        #region Construction

        internal UpdateSqlSection(Database db, IQueryTable table)
            : this(db, table.GetTableName())
        { }

        internal UpdateSqlSection(Database db, string tablename)
            : base(db, tablename)
        { }

        public UpdateSqlSection() :
            base(CommonUtils.GetThisQueryTable<TTable>())
        { }


        #endregion

        #region Public Methods
         
        public UpdateSqlSection<TTable> AddColumn(System.Linq.Expressions.Expression<Func<TTable, object>> fun, object value)
        {
            QueryColumn qc = fun.Body.GetQueryColumn<TTable>();
            this.AddColumn(qc, value);
            return this;
        }

        public UpdateSqlSection<TTable> Where(System.Linq.Expressions.Expression<Func<TTable, bool>> fun)
        {
            Condition where = ExpressionUtil.Eval<TTable>(fun);
            this.Where(where);
            return this;
        }

        public UpdateSqlSection<TTable> Where<ITable>(System.Linq.Expressions.Expression<Func<ITable, bool>> fun)  where ITable :class, IBaseEntity
        {
            Condition where = ExpressionUtil.Eval<ITable>(fun);
            this.Where(where);
            return this;
        }

        public UpdateSqlSection<TTable> Join<ITable>(System.Linq.Expressions.Expression<Func<ITable, bool>> fun)  where ITable :class, IBaseEntity
        {
            var where = ExpressionUtil.Eval<ITable>(fun);
            Join(BaseDbQuery<ITable>.Table.EntityInfo, where);
            return this;
        }

        public UpdateSqlSection<TTable> Join<ITable>(string joinTableAliasName, System.Linq.Expressions.Expression<Func<ITable, bool>> fun)  where ITable :class, IBaseEntity
        {
            var where = ExpressionUtil.Eval<ITable>(fun);
            Join(BaseDbQuery<ITable>.Table.EntityInfo, joinTableAliasName, where);
            return this;
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
