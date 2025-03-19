using System.Text;
using ConditionalQueries.BaseQuerySection;
using ConditionalQueries.Parsers;
using ConditionalQueries.Interfaces;

namespace ConditionalQueries.QuerySections;
public class Values(IQueryValue[] Values) : CommaSection<Values>(Values), IParsableSection<Values> {
    public static string Identifier => "VALUES";
    public static Values ParseSection(ReadOnlySpan<char> sql) {
        var index = sql.IndexOf('(');
        if (index == -1)
            index = 0;
        var lastIndex = sql.LastIndexOf(')');
        return new(QueryValueParser.ParseValues(sql[(index + 1)..lastIndex]));
    }
    public override bool Parse(StringBuilder sb, HashSet<string> conds) {
        if (PreBuildedSQL is not null) {
            sb.Append(PreBuildedSQL);
            return true;
        }
        sb.Append(Identifier).Append(' ').Append('(');
        if (QuerySectionHelper.JoinByComma(sb, conds, Values)) {
            sb.Append(')');
            return true;
        }
        sb.Length -= Identifier.Length + 2;
        return false;
    }
    public override void AppendParsable(StringBuilder sb) {
        sb.Append(Identifier).Append(" (");
        QuerySectionHelper.AppendParsableByComa(sb, Values);
        sb.Append(')');
    }
    public override string ToString() {
        StringBuilder sb = new();
        AppendParsable(sb);
        return sb.ToString();
    }
    public override Values GetMerged(Values section) => new(Values.MergedWith(section.Values));
}