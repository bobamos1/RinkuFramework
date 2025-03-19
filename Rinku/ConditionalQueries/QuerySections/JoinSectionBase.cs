using System.Text;
using ConditionalQueries.BaseQuerySection;
using ConditionalQueries.Parsers;
using ConditionalQueries.Interfaces;

namespace ConditionalQueries.QuerySections;
public abstract class JoinSectionBase<T>(IQueryValue FirstPart, IQueryValue[]? Values) : TwoPartSection<T>(FirstPart, Values), IJoinGroup where T : JoinSectionBase<T>, IParsableSection<T> {
    public const string ON = "ON ";
    public override bool ParseValues(StringBuilder sb, HashSet<string> conds) {
        if (Values is null || Values.Length == 0) {
            sb.Length--;
            return true;
        }
        sb.Append(ON);
        if (!QuerySectionHelper.JoinBySpace(sb, conds, Values))
            return false;
        QuerySectionHelper.RemoveEndingAndOr(sb);
        return true;
    }
    public static (IQueryValue, IQueryValue[]?) PreParseSection(ReadOnlySpan<char> sql) {
        var indexOfOn = sql.FindIndex(ON);
        if (indexOfOn == -1)
            return (QueryValueParser.ParseQueryValue(sql[T.Identifier.Length..]), null);
        var table = QueryValueParser.ParseQueryValue(sql[T.Identifier.Length..indexOfOn]);
        return (table, QueryConditionParser.ParseConditions(sql[(indexOfOn + ON.Length)..]));
    }
    public override void AppendParsable(StringBuilder sb) {
        sb.Append(T.Identifier).Append(' ');
        FirstPart.AppendParsable(sb);
        if (Values is null)
            return;
        sb.Append(' ').Append(ON);
        QuerySectionHelper.AppendParsableBySpace(sb, Values);
        sb.Length -= QueryConditionParser.AND.Length;
    }
}