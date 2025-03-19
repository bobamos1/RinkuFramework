using System.Diagnostics.CodeAnalysis;
using System.Text;
using ConditionalQueries.Interfaces;

namespace ConditionalQueries.QueryValueConditions;
public class MultipleRequiredCondition(string[] Identifiers) : IQueryValueCondition, IParsableQueryValueCondition {
    public const char CharIdentifier = '|';
    public string[] Identifiers = Identifiers;

    public static bool TryParse(ReadOnlySpan<char> condition, [MaybeNullWhen(false)] out IQueryValueCondition res) {
        if (condition.IsEmpty || condition[0] != CharIdentifier || !condition.Contains(',')) {
            res = null;
            return false;
        }
        res = new MultipleRequiredCondition(condition[1..].SplitTrim());
        return true;
    }

    public void AddVariables(HashSet<string> optionalVariables) => optionalVariables.UnionWith(Identifiers);
    public virtual void AppendParsable(StringBuilder sb) {
        sb.Append("/*");
        sb.Append(CharIdentifier);
        foreach (var identifier in Identifiers)
            sb.Append(identifier).Append(',');
        sb.Length--;
        sb.Append("*/");
    }
    public bool Valid(HashSet<string> conds) => Identifiers.All(conds.Contains);
}
public class MultipleRequiredConditionByVariable(string[] Identifier) : MultipleRequiredCondition(Identifier) {
    public override void AppendParsable(StringBuilder sb) => sb.Append("/*#*/");
}
