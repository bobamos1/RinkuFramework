using System.Text;
using ConditionalQueries.BaseQuerySection;
using ConditionalQueries.Parsers;
using ConditionalQueries.Interfaces;

namespace ConditionalQueries.QuerySections;
public class Insert(IQueryValue FirstPart, IQueryValue[]? Values) : TwoPartSection<Insert>(FirstPart, Values), IParsableSection<Insert> {
    public static string Identifier => "INSERT INTO";
    public static Insert ParseSection(ReadOnlySpan<char> sql) {
        var startValuesIndex = sql.IndexOf('(');
        if (startValuesIndex == -1)
            startValuesIndex = sql.Length;
        var firstPart = QueryValueParser.ParseQueryValue(sql[Identifier.Length..startValuesIndex]);
        if (startValuesIndex == sql.Length)
            return new(firstPart, null);
        var endValuesIndex = sql.LastIndexOf(')');
        if (endValuesIndex == -1)
            throw new Exception();
        return new(firstPart, QueryValueParser.ParseValues(sql[(startValuesIndex + 1)..endValuesIndex]));
    }

    public override bool ParseValues(StringBuilder sb, HashSet<string> conds) {
        if (Values is null)
            return true;
        sb.Append('(');
        if (!QuerySectionHelper.JoinByComma(sb, conds, Values))
            return false;
        sb.Append(')');
        return true;
    }
    public override void AddTables(HashSet<string> tables) => tables.Add(FirstPart.ExtractObjectName());
    public override void AppendParsable(StringBuilder sb) {
        sb.Append(Identifier).Append(' ');
        FirstPart.AppendParsable(sb);
        if (Values is null)
            return;
        sb.Append(" (");
        QuerySectionHelper.AppendParsableByComa(sb, Values);
        sb.Append(')');
    }
    public override string ToString() {
        StringBuilder sb = new();
        AppendParsable(sb);
        return sb.ToString();
    }
    public override Insert GetMerged(Insert section) => new(FirstPart, ArrayHelper.Merge(Values, section.Values));
}