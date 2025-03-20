using System.Text;
using Data;
using Rinku.Context;

namespace Rinku.ConditionalSelect;
public struct ColWithCond<T> where T : IContext {
    public IConditionalSelectColumnCondition<T>? Condition;
    public IConditionalSelectValue Column;
    public readonly bool TryParseSQL(StringBuilder sb, T ctx, IDTMaker dtMaker) {
        if (Condition is not null && !Condition.Valid(ctx))
            return false;
        return Column.ParseSQL(sb, ctx, dtMaker);
    }
}
public class CondColList<T> where T : IContext {
    public static implicit operator ColWithCond<T>[](CondColList<T> list) => [.. list.Columns];
    private readonly List<ColWithCond<T>> Columns = [];
    private int _lastIndex = -1;
    public CondColList<T> Add(IConditionalSelectValue col) {
        _lastIndex++;
        Columns.Add(new ColWithCond<T>() {
            Condition = null,
            Column = col
        });
        return this;
    }
    public CondColList<T> Add<TType>(string name, string sql)
        => Add(new SingleValCondCol<TType>(name, sql));
    public CondColList<T> Add<TTypeID, TTypeVal>(string name, string sqlId, string sqlValue)
        => Add(new RefValCondCol<TTypeID, TTypeVal>(name, sqlId, sqlValue));
    public CondColList<T> AddNull<TType>(string name, string sql)
        where TType : struct
        => Add(new SingleValCondCol<TType?>(name, sql));
    public CondColList<T> AddNull<TTypeID, TTypeVal>(string name, string sqlId, string sqlValue)
        => Add(new NullRefValCondCol<TTypeID, TTypeVal>(name, sqlId, sqlValue));
    public CondColList<T> When(IConditionalSelectColumnCondition<T> cond) {
        Columns[_lastIndex] = new() {
            Condition = cond,
            Column = Columns[_lastIndex].Column
        };
        return this;
    }
    public CondColList<T> WhenHasRoles<TUnit>(TUnit[]? roles) => When(new HasRoleCondition<T, TUnit>(roles));
}
public interface IConditionalSelectValue {
    public string Name { get; }
    public bool ParseSQL(StringBuilder sb, IContext ctx, IDTMaker dtMaker);
    public void AddColumns(HashSet<string> columns);
    public void AddColAsConds(HashSet<string> conds);
}