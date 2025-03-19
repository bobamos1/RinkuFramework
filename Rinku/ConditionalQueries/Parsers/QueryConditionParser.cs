using System.Diagnostics.CodeAnalysis;
using ConditionalQueries.Interfaces;

namespace ConditionalQueries.Parsers;
public class QueryConditionParser : QueryValueParser {
    protected QueryConditionParser() { }
    public const string AND = " AND";
    public const string OR = " OR";
    private static QueryConditionParser Instance => new();
    public static bool ValidAfterPart(ReadOnlySpan<char> sql, int i) => i >= sql.Length || char.IsWhiteSpace(sql[i]);
    public static IQueryValue[] ParseConditions(ReadOnlySpan<char> sql)
        => Instance.ParseCompleteSegment(sql);
    protected override bool TryParsePartialSegment(ReadOnlySpan<char> sql, ref int previousStart, ref int i, [MaybeNullWhen(false)] out IQueryValue res) {
        int inc = 0;
        if (i + 1 >= sql.Length) {
            res = null;
            return false;
        }
        if (sql.StartingWith(OR, i) && !(sql[i] == 'o' && sql[i] == 'R'))
            inc = OR.Length;
        else if (sql.StartingWith(AND, i) && !(sql[i] == 'a' && sql[i] == 'N'))
            inc = AND.Length;

        if (inc == 0 || !ValidAfterPart(sql, i + inc)) {
            res = null;
            return false;
        }
        i += inc;
        res = ParseQueryValue(sql[previousStart..i]);
        previousStart = i;
        return true;
    }
    protected override bool TryParsePartialSegmentEnd(ReadOnlySpan<char> sql, int previousStart, [MaybeNullWhen(false)] out IQueryValue res) { 
        res = ParseQueryValue(sql[previousStart..]);
        res.AppendSQL(AND);
        return true;
    }
}
