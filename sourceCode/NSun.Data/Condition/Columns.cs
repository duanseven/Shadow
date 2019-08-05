using System;
using System.Data;
using System.Runtime.Serialization;
using NSun.Data.Configuration;


namespace NSun.Data
{
    [Serializable]
    [DataContract(Namespace = "http://nsun-shadow.com")]
    [KnownType("KnownTypes")]
    public class QueryColumn : ExpressionClip, IColumn
    {
        #region IColumn 成员

        [DataMember]
        public string ColumnName { get; set; }

        [DataMember]
        public string PropertyName { get; set; }

        #endregion

        #region Constructors

        public QueryColumn(string columnName, DbType dbtype)
        {
            if (string.IsNullOrEmpty(columnName))
                throw new ArgumentNullException("columnName");
            PropertyName = string.Empty;
            ColumnName = columnName;
            DataType = dbtype;            
            Sql = columnName.ToDatabaseObjectName();
        }

        public QueryColumn(string columnName, DbType dbtype, string propertyname)
        {
            if (string.IsNullOrEmpty(columnName))
                throw new ArgumentNullException("columnName");

            PropertyName = propertyname;
            ColumnName = columnName;
            DataType = dbtype;
            Sql = columnName.ToDatabaseObjectName();
        }

        public QueryColumn(ExpressionClip expr, string columnName, DbType dbtype)
            : this(columnName, dbtype)
        {
            if (ReferenceEquals(expr, null))
                throw new ArgumentNullException("expr");

            Sql = expr.Sql;
            if (expr.ChildExpressions != null)
                ChildExpressions.AddRange(expr.ChildExpressions);
        }

        #endregion         

        #region Public Methods

        public override object Clone()
        {
            var clone = new QueryColumn((ExpressionClip)base.Clone(), ColumnName, DataType);
            return clone;
        }

        public Assignment Set(ExpressionClip value)
        {
            return new Assignment(this,
                                  ((object)value) == null
                                      ? (IExpression)NullExpression.Value
                                      : value);
        }

        public Assignment Set(object value)
        {
            return new Assignment(this, new ParameterExpression(value, DataType));
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
