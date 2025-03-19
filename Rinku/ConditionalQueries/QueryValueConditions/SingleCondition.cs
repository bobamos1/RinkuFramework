using System.Diagnostics.CodeAnalysis;
using System.Text;
using ConditionalQueries.Interfaces;

namespace ConditionalQueries.QueryValueConditions;
public class SingleCondition(string Identifier) : IQueryValueCondition, IParsableQueryValueCondition {
    public string Identifier = Identifier;
    public static string? GetVariable(string SQL, int indexOfAt) {
        for (int j = indexOfAt + 1; j < SQL.Length; j++)
            if (SQL[j] is ' ' or '=' or ',' or ')')
                return SQL[(indexOfAt + 1)..j];
        return null;
    }
    public static bool TryParse(ReadOnlySpan<char> condition, [MaybeNullWhen(false)] out IQueryValueCondition res) {
        if (condition.IsEmpty) {
            res = null;
            return false;
        }
        res = new SingleCondition(condition.ToString());
        return true;
    }
    public void AddVariables(HashSet<string> optionalVariables) => optionalVariables.Add(Identifier);
    public virtual void AppendParsable(StringBuilder sb) => sb.Append("/*").Append(Identifier).Append("*/");
    public bool Valid(HashSet<string> conds) => conds.Contains(Identifier);
}
public class SingleConditionByVariable(string Identifier) : SingleCondition(Identifier) {
    public override void AppendParsable(StringBuilder sb) => sb.Append("/*#*/");
}