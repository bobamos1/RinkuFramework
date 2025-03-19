using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace ConditionalQueries.Interfaces;

public delegate bool TryParseQueryValue(ReadOnlySpan<char> sql, [MaybeNullWhen(false)] out IQueryValue res);
public interface IQueryValue : IQuerySegment {
    public IQueryValueCondition Condition { get; set; }
    public void AddVariables(HashSet<string> optionalVariables);
    public void AppendParsable(StringBuilder sb);
    public string ExtractObjectName();
    public bool Valid();
    public bool Valid(HashSet<string> conds);
    public void AppendSQL(string sql);
}
public interface IParsableQueryValue {
    public abstract static bool TryParse(ReadOnlySpan<char> sql, [MaybeNullWhen(false)] out IQueryValue res);
}