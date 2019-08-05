
namespace NSun.Data
{
    /// <summary>
    /// The interface represnts an abstract query column expression
    /// </summary>
    public interface IColumn : IExpression
    {
        string ColumnName { get; }

        System.Data.DbType DataType { get; }
    }
}
