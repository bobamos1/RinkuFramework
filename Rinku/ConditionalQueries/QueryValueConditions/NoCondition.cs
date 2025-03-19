using System.Diagnostics.CodeAnalysis;
using System.Text;
using ConditionalQueries.Interfaces;

namespace ConditionalQueries.QueryValueConditions;
public class NoCondition : IQueryValueCondition, IParsableQueryValueCondition {
    private NoCondition() { }
    public static NoCondition Instance { get; private set; } = new();
    public static bool TryParse(ReadOnlySpan<char> condition, [MaybeNullWhen(false)] out IQueryValueCondition res) {
        if (condition.IsEmpty) {
            res = Instance;
            return true;
        }
        res = null;
        return false;
    }

    public void AddVariables(HashSet<string> optionalVariables) { }
    public void AppendParsable(StringBuilder sb) { }
    public bool Valid(HashSet<string> conds) => true;
}
