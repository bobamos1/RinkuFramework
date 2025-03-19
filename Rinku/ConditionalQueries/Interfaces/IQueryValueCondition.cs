using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace ConditionalQueries.Interfaces;
public delegate bool TryParseCondition(ReadOnlySpan<char> condition, [MaybeNullWhen(false)] out IQueryValueCondition res);
public interface IQueryValueCondition {
    public bool Valid(HashSet<string> conds);
    public void AddVariables(HashSet<string> optionalVariables);
    public void AppendParsable(StringBuilder sb);
}
public interface IParsableQueryValueCondition {
    public static abstract bool TryParse(ReadOnlySpan<char> condition, [MaybeNullWhen(false)] out IQueryValueCondition res);

}