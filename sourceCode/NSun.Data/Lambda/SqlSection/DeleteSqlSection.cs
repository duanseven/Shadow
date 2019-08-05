using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization; 
using NSun.Data.Configuration;
using NSun.Data.Lambda;

namespace NSun.Data
{
    [Serializable]
    [DataContract(Namespace = "http://nsun-shadow.com")]
    [KnownType("KnownTypes")]
    public class DeleteSqlSection<TTable> : DeleteSqlSection where TTable : class,IBaseEntity
    {
        #region Construction

        internal DeleteSqlSection(Database db, IQueryTable table)
            : this(db, table.GetTableName())
        { }

        internal DeleteSqlSection(Database db, string tablename)
            : base(db, tablename)
        { }

        public DeleteSqlSection() :
            base(CommonUtils.GetThisQueryTable<TTable>())
        { }

        #endregion

        #region Public Methods

        public DeleteSqlSection<TTable> Where(System.Linq.Expressions.Expression<Func<TTable, bool>> fun)
        {
            Condition where = ExpressionUtil.Eval(fun);
            Where(where);
            return this;
        }

        public DeleteSqlSection<TTable> Where<ITable>(System.Linq.Expressions.Expression<Func<ITable, bool>> fun)
            where ITable : class, IBaseEntity
        {
            Condition where = ExpressionUtil.Eval(fun);
            Where(where);
            return this;
        }

        public DeleteSqlSection<TTable> Join<ITable>(System.Linq.Expressions.Expression<Func<ITable, bool>> fun) where ITable : class, IBaseEntity
        {
            Condition where = ExpressionUtil.Eval<ITable>(fun);
            Join(BaseDbQuery<ITable>.Table.EntityInfo, where);
            return this;
        }

        public DeleteSqlSection<TTable> Join<ITable>(string joinTableAliasName, System.Linq.Expressions.Expression<Func<ITable, bool>> fun) where ITable : class, IBaseEntity
        {
            Condition where = ExpressionUtil.Eval<ITable>(fun);
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
