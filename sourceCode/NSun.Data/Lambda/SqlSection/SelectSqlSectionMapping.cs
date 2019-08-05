using System;
using System.Runtime.Serialization;
using NSun.Data.Configuration;  
using NSun.Data.Lambda;

namespace NSun.Data
{
    [Serializable]
    [DataContract(Namespace = "http://nsun-shadow.com")]
    [KnownType("KnownTypes")]
    public class SelectSqlSection<TTable> : SelectSqlSection where TTable : class,IBaseEntity
    {
        #region Construction

        internal SelectSqlSection(Database db, string tablename)
            : base(db, tablename)
        { }

        public SelectSqlSection() :
            base(CommonUtils.GetThisQueryTable<TTable>())
        { }

        #endregion 

        #region KnownTypes

        static Type[] KnownTypes()
        {
            return KnownTypeRegistry.Instance.KnownTypes;
        }

        #endregion

        #region Select

        public SelectSqlSection<TTable> Select(System.Linq.Expressions.Expression<Func<TTable, object>> columns)
        { 
            Select(LambdaUtil.GetExpressionClip(columns));
            return this;
        }
        
        public SelectSqlSection<TTable> Select<ITable>(System.Linq.Expressions.Expression<Func<ITable, object>> columns)  where ITable :class, IBaseEntity
        {
            Select(LambdaUtil.GetExpressionClip<ITable>(columns));
            return this;
        }

        public SelectSqlSection<TTable> Select<ITable>(System.Linq.Expressions.Expression<Func<TTable,ITable,object>> columns) where ITable : class, IBaseEntity
        {
            Select(LambdaUtil.GetExpressionClip<TTable, ITable>(columns));
            return this;
        }

        public SelectSqlSection<TTable> BindColumn(System.Linq.Expressions.Expression<Func<TTable, object>> columns) 
        {
            BindColumn(LambdaUtil.GetExpressionClip(columns));
            return this;
        }

        public SelectSqlSection<TTable> BindColumn<ITable>(System.Linq.Expressions.Expression<Func<ITable, object>> columns) where ITable : class, IBaseEntity
        {
            BindColumn(LambdaUtil.GetExpressionClip<ITable>(columns));
            return this;
        }

        public SelectSqlSection<TTable> BindColumn<ITable>(System.Linq.Expressions.Expression<Func<TTable, ITable, object>> columns) where ITable : class, IBaseEntity
        {
            BindColumn(LambdaUtil.GetExpressionClip<TTable, ITable>(columns));
            return this;
        }

        #endregion

        #region Where

        public SelectSqlSection<TTable> Where(System.Linq.Expressions.Expression<Func<TTable, bool>> fun)
        {
            Condition where = ExpressionUtil.Eval(fun);
            Where(where);
            return this;
        }

        public SelectSqlSection<TTable> Where<ITable>(System.Linq.Expressions.Expression<Func<ITable, bool>> fun)  where ITable :class, IBaseEntity
        {
            Condition where = ExpressionUtil.Eval<ITable>(fun);
            Where(where);
            return this;
        }

        public SelectSqlSection<TTable> Where<ITable>(System.Linq.Expressions.Expression<Func<TTable, ITable, bool>> fun)  where ITable :class, IBaseEntity
        {
            Condition where = ExpressionUtil.Eval<TTable, ITable>(fun);
            Where(where);
            return this;
        }

        #endregion

        #region OrderBy

        public SelectSqlSection<TTable> SortBy(System.Linq.Expressions.Expression<Func<TTable, object>> fun)
        {
            this.SortBy(LambdaUtil.GetOrderByClip<TTable>(fun)[0]);
            return this;
        }

        public SelectSqlSection<TTable> SortBy<ITable>(System.Linq.Expressions.Expression<Func<ITable, object>> fun)  where ITable :class, IBaseEntity
        {
            this.SortBy(LambdaUtil.GetOrderByClip<ITable>(fun)[0]);
            return this;
        }

        public SelectSqlSection<TTable> SortBy<ITable>(System.Linq.Expressions.Expression<Func<TTable, ITable, object>> fun)  where ITable :class, IBaseEntity
        {
            this.SortBy(LambdaUtil.GetOrderByClip<TTable, ITable>(fun)[0]);
            return this;
        }

        public SelectSqlSection<TTable> ThenSortBy(System.Linq.Expressions.Expression<Func<TTable, object>> fun)
        {
            this.ThenSortBy(LambdaUtil.GetOrderByClip<TTable>(fun)[0]);
            return this;
        }

        public SelectSqlSection<TTable> ThenSortBy<ITable>(System.Linq.Expressions.Expression<Func<ITable, object>> fun)  where ITable :class, IBaseEntity
        {
            this.ThenSortBy(LambdaUtil.GetOrderByClip<ITable>(fun)[0]);
            return this;
        }

        public SelectSqlSection<TTable> ThenSortBy<ITable>(System.Linq.Expressions.Expression<Func<TTable, ITable, object>> fun)  where ITable :class, IBaseEntity
        {
            this.ThenSortBy(LambdaUtil.GetOrderByClip<TTable, ITable>(fun)[0]);
            return this;
        }

        #endregion

        #region Having

        public SelectSqlSection<TTable> Having(System.Linq.Expressions.Expression<Func<TTable, bool>> fun)
        {
            Condition where = ExpressionUtil.Eval<TTable>(fun);
            this.Having(where);
            return this;
        }

        public SelectSqlSection<TTable> Having<ITable>(System.Linq.Expressions.Expression<Func<ITable, bool>> fun)  where ITable :class, IBaseEntity
        {
            Condition where = ExpressionUtil.Eval<ITable>(fun);
            this.Having(where);
            return this;
        }

        public SelectSqlSection<TTable> Having<ITable>(System.Linq.Expressions.Expression<Func<TTable, ITable, bool>> fun)  where ITable :class, IBaseEntity
        {
            Condition where = ExpressionUtil.Eval<TTable, ITable>(fun);
            this.Having(where);
            return this;
        }
        #endregion

        #region GroupBy

        public SelectSqlSection<TTable> GroupBy(System.Linq.Expressions.Expression<Func<TTable, object>> fun)
        {
            this.GroupBy(LambdaUtil.GetQueryColumn<TTable>(fun));
            return this;
        }

        public SelectSqlSection<TTable> GroupBy<ITable>(System.Linq.Expressions.Expression<Func<ITable, object>> fun)  where ITable :class, IBaseEntity
        {
            this.GroupBy(LambdaUtil.GetQueryColumn<ITable>(fun));
            return this;
        }

        public SelectSqlSection<TTable> GroupBy<ITable>(System.Linq.Expressions.Expression<Func<TTable, ITable, object>> fun)
             where ITable :class, IBaseEntity
        {
            this.GroupBy(LambdaUtil.GetQueryColumn<TTable, ITable>(fun));
            return this;
        }
        #endregion

        #region Join

        public SelectSqlSection<TTable> Join<ITable>(string joinTableAliasName, System.Linq.Expressions.Expression<Func<TTable, ITable, bool>> joinOnWhere)  where ITable :class, IBaseEntity
        {
            this.Join(BaseDbQuery<ITable>.Table.EntityInfo, joinTableAliasName, ExpressionUtil.Eval<TTable, ITable>(joinOnWhere));
            return this;
        }

        public SelectSqlSection<TTable> Join<ITable>(System.Linq.Expressions.Expression<Func<TTable, ITable, bool>> joinOnWhere)  where ITable :class, IBaseEntity
        {
            this.Join(BaseDbQuery<ITable>.Table.EntityInfo, ExpressionUtil.Eval<TTable, ITable>(joinOnWhere));
            return this;
        }

        public SelectSqlSection<TTable> Join<FTable, ETable>(System.Linq.Expressions.Expression<Func<FTable, ETable, bool>> joinOnWhere)
            where FTable : class, IBaseEntity
            where ETable : class, IBaseEntity
        {
            this.Join(BaseDbQuery<FTable>.Table.EntityInfo, ExpressionUtil.Eval<FTable, ETable>(joinOnWhere));
            return this;
        }

        public SelectSqlSection<TTable> LeftJoin<ITable>(string joinTableAliasName, System.Linq.Expressions.Expression<Func<TTable, ITable, bool>> joinOnWhere)  where ITable :class, IBaseEntity
        {
            this.LeftJoin(BaseDbQuery<ITable>.Table.EntityInfo, joinTableAliasName, ExpressionUtil.Eval<TTable, ITable>(joinOnWhere));
            return this;
        }

        public SelectSqlSection<TTable> LeftJoin<ITable>(System.Linq.Expressions.Expression<Func<TTable, ITable, bool>> joinOnWhere)  where ITable :class, IBaseEntity
        {
            this.LeftJoin(BaseDbQuery<ITable>.Table.EntityInfo, ExpressionUtil.Eval<TTable, ITable>(joinOnWhere));
            return this;
        }

        public SelectSqlSection<TTable> LeftJoin<FTable, ETable>(System.Linq.Expressions.Expression<Func<FTable, ETable, bool>> joinOnWhere)
            where FTable : class, IBaseEntity
            where ETable : class, IBaseEntity
        {
            this.LeftJoin(BaseDbQuery<FTable>.Table.EntityInfo, ExpressionUtil.Eval<FTable, ETable>(joinOnWhere));
            return this;
        }

        public SelectSqlSection<TTable> RightJoin<ITable>(string joinTableAliasName, System.Linq.Expressions.Expression<Func<TTable, ITable, bool>> joinOnWhere)  where ITable :class, IBaseEntity
        {
            this.RightJoin(BaseDbQuery<ITable>.Table.EntityInfo, joinTableAliasName, ExpressionUtil.Eval<TTable, ITable>(joinOnWhere));
            return this;
        }

        public SelectSqlSection<TTable> RightJoin<ITable>(System.Linq.Expressions.Expression<Func<TTable, ITable, bool>> joinOnWhere)  where ITable :class, IBaseEntity
        {
            this.RightJoin(BaseDbQuery<ITable>.Table.EntityInfo, ExpressionUtil.Eval<TTable, ITable>(joinOnWhere));
            return this;
        }

        public SelectSqlSection<TTable> RightJoin<FTable, ETable>(System.Linq.Expressions.Expression<Func<FTable, ETable, bool>> joinOnWhere)
            where FTable : class, IBaseEntity
            where ETable : class, IBaseEntity
        {
            this.RightJoin(BaseDbQuery<FTable>.Table.EntityInfo, ExpressionUtil.Eval<FTable, ETable>(joinOnWhere));
            return this;
        }

        public SelectSqlSection<TTable> FullJoin<ITable>(string joinTableAliasName, System.Linq.Expressions.Expression<Func<TTable, ITable, bool>> joinOnWhere)  where ITable :class, IBaseEntity
        {
            this.FullJoin(BaseDbQuery<ITable>.Table.EntityInfo, joinTableAliasName, ExpressionUtil.Eval<TTable, ITable>(joinOnWhere));
            return this;
        }

        public SelectSqlSection<TTable> FullJoin<ITable>(System.Linq.Expressions.Expression<Func<TTable, ITable, bool>> joinOnWhere)  where ITable :class, IBaseEntity
        {
            this.FullJoin(BaseDbQuery<ITable>.Table.EntityInfo, ExpressionUtil.Eval<TTable, ITable>(joinOnWhere));
            return this;
        }

        public SelectSqlSection<TTable> FullJoin<FTable, ETable>(System.Linq.Expressions.Expression<Func<FTable, ETable, bool>> joinOnWhere)
            where FTable : class, IBaseEntity
            where ETable : class, IBaseEntity
        {
            this.FullJoin(BaseDbQuery<FTable>.Table.EntityInfo, ExpressionUtil.Eval<FTable, ETable>(joinOnWhere));
            return this;
        }
        #endregion

    }
}
