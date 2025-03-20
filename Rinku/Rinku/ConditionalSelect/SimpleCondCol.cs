using System.Text;
using Data;
using Rinku.Context;

namespace Rinku.ConditionalSelect;
public abstract class SimpleCondCol<T>(string Name) : IConditionalSelectValue {
    public string Name { get; } = Name;
    public abstract void AppendSQL(StringBuilder sb, IContext ctx);
    public virtual void AddColumns(HashSet<string> columns) => columns.Add(Name);
    public virtual void AddColAsConds(HashSet<string> conds) => conds.Add(Name);
    public virtual bool ParseSQL(StringBuilder sb, IContext ctx, IDTMaker dtMaker) {
        AppendSQL(sb, ctx);
        sb.Append(" AS [").Append(Name).Append(']');
        dtMaker.Add<T>(Name);
        return true;
    }
}
