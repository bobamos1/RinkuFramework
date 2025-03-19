using System.Text;
using ConditionalQueries.Interfaces;
using ConditionalQueries.Parsers;

namespace ConditionalQueries.BaseQuerySection;

public abstract class ConditionSection<T>(IQueryValue[] Values) : CommaSection<T>(Values) where T : ConditionSection<T>, IParsableSection<T> {
    public override bool Parse(StringBuilder sb, HashSet<string> conds) {
        if (PreBuildedSQL is not null) {
            sb.Append(PreBuildedSQL);
            return true;
        }
        sb.Append(T.Identifier).Append(' ');
        if (QuerySectionHelper.JoinBySpace(sb, conds, Values)) {
            QuerySectionHelper.RemoveEndingAndOr(sb);
            return true;
        }
        sb.Length -= T.Identifier.Length;
        return false;
    }
    public override void AppendParsable(StringBuilder sb) {
        sb.Append(T.Identifier).Append(' ');
        QuerySectionHelper.AppendParsableBySpace(sb, Values);
        sb.Length -= QueryConditionParser.AND.Length;
    }
}