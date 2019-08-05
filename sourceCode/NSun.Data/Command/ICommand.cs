using System.Collections.ObjectModel;

namespace NSun.Data
{
    public interface IConditionSection
    {
        Condition ConditionWhere { get; } 
    }

    public interface ICudSection
    {
        ReadOnlyCollection<Assignment> Assignments { get; }
        //QueryCriteria AddColumn(QueryColumn column, object value);
    }

    public interface ISelectSection
    {
        ReadOnlyCollection<ExpressionClip> ResultColumns { get; }        
    }

    public interface IParameterConditions
    {
        ReadOnlyCollection<ParameterEqualsCondition> ParameterConditions { get; }
    }
}
