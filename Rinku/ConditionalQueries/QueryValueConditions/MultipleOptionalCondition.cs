using System.Diagnostics.CodeAnalysis;
using System.Text;
using ConditionalQueries.Interfaces;

namespace ConditionalQueries.QueryValueConditions;
public class MultipleOptionalCondition(string[] Identifiers) : IQueryValueCondition, IParsableQueryValueCondition {
    public string[] Identifiers = Identifiers;

    public static bool TryParse(ReadOnlySpan<char> condition, [MaybeNullWhen(false)] out IQueryValueCondition res) {
        if (!condition.Contains(',')) {
            res = null;
            return false;
        }
        res = new MultipleOptionalCondition(condition.SplitTrim());
        return true;
    }

    public void AddVariables(HashSet<string> optionalVariables) => optionalVariables.UnionWith(Identifiers);
    public void AppendParsable(StringBuilder sb) {
        sb.Append("/*");
        foreach (var identifier in Identifiers)
            sb.Append(identifier).Append(',');
        sb.Length--;
        sb.Append("*/");
    }
    public bool Valid(HashSet<string> conds) => Identifiers.Any(conds.Contains);
}
