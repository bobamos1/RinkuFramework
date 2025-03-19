using System.Text;

namespace Rinku.Http.ConditionalColumns;
public abstract class SimpleCondCol2<T>(string Name) {
    public Type Type = typeof(T);
    public string Name = Name;
    public abstract void AppendSQL(StringBuilder sb);
    public virtual void AddColumns(HashSet<string> columns) => columns.Add(Name);
    public virtual void AddColAsConds(HashSet<string> conds) => conds.Add(Name);
    public bool ParseSQL(StringBuilder sb, Dictionary<string, int> template) {
        AppendSQL(sb);
        sb.Append(" AS [").Append(Name).Append(']');
        template[Name] = template.Count;
        return true;
    }
}
