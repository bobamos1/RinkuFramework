using System.Text;

namespace ConditionalQueries.Interfaces;

public interface IQuerySection : IQuerySegment {
    public void AddTables(HashSet<string> table);
    public void AddColumns(HashSet<string> columns);
    public void AddVariables(HashSet<string> optionalVariables);
    public bool Valid();
    public bool PreBuild();
    public void AppendParsable(StringBuilder sb);
    public IQuerySection GetMerged(IQuerySection section);
}
public abstract class QuerySection<T> : IQuerySection where T : QuerySection<T> {
    public abstract void AddColumns(HashSet<string> columns);
    public abstract void AddTables(HashSet<string> table);
    public abstract void AddVariables(HashSet<string> optionalVariables);
    public abstract void AppendParsable(StringBuilder sb);
    public IQuerySection GetMerged(IQuerySection section) => GetMerged((T)section);
    public abstract T GetMerged(T section);
    public abstract bool Parse(StringBuilder sb, HashSet<string> conds);
    public abstract bool PreBuild();
    public abstract bool Valid();
}