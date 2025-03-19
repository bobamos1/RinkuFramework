using System.Text;
using ConditionalQueries.BaseQuerySection;
using ConditionalQueries.Parsers;
using ConditionalQueries.Interfaces;

namespace ConditionalQueries.QuerySections;
public class Select(IQueryValue[] Values) : CommaSection<Select>(Values), IParsableSection<Select> {
    public string? FirstPart;
    public static string Identifier => "SELECT";
    public static Select ParseSection(ReadOnlySpan<char> sql) { 
        var newStart = Identifier.Length;
        var firstPart = ExtractFirstSelectPart(sql, ref newStart);
        var values = QueryValueParser.ParseValues(sql[newStart..]);
        return new(values) { FirstPart = firstPart };
    }
    public override void AddColumns(HashSet<string> columns) => columns.UnionWith(ExtractObjectNamesOfValues(Values));
    public static IEnumerable<string> ExtractObjectNamesOfValues(IEnumerable<IQueryValue> values) {
        foreach (var value in values)
            yield return value.ExtractObjectName();
    }
    public override bool Parse(StringBuilder sb, HashSet<string> conds) {
        if (FirstPart is null)
            return base.Parse(sb, conds);
        if (PreBuildedSQL is not null) {
            sb.Append(PreBuildedSQL);
            return true;
        }
        sb.Append(Identifier).Append(' ').Append(FirstPart).Append(' ');
        if (QuerySectionHelper.JoinByComma(sb, conds, Values))
            return true;
        sb.Length -= Identifier.Length + FirstPart.Length + 2;
        return false;
    }
    public override void AppendParsable(StringBuilder sb) {
        if (FirstPart is null) {
            base.AppendParsable(sb);
            return; 
        }
        sb.Append(Identifier).Append(' ').Append(FirstPart).Append(' ');
        QuerySectionHelper.AppendParsableByComa(sb, Values);
    }
    public static string? ExtractFirstSelectPart(ReadOnlySpan<char> sql, ref int newStart) {
        while (newStart < sql.Length && char.IsWhiteSpace(sql[newStart]))
            newStart++;
        foreach (var look in Query.PossibleStartingPartSelect) {
            var i = MatchStart(sql, newStart, look);
            if (i > -1) {
                var res = sql[newStart..i].ToString();
                newStart = i;
                return res;
            }
        }
        return null;
    }
    public static int MatchStart(ReadOnlySpan<char> sql, int i, string look) {
        for (var j = 0; j < look.Length; j++, i++) {
            if (i > sql.Length)
                return -1;
            if ((sql[i] | 32) != (look[j] | 32)) {
                if (look[j] != '#')
                    return -1;
                do
                    i++;
                while (i < sql.Length && char.IsDigit(sql[i]));
                i--;
                continue;
            }
        }
        return i;
    }
    public override Select GetMerged(Select section) {
        if (section.Values.Length > 0 && section.Values[0].ExtractObjectName() == "*")
            return new Select(Values) { FirstPart = FirstPart };
        return new(Values.MergedWith(section.Values)) { FirstPart = FirstPart };
    }
}