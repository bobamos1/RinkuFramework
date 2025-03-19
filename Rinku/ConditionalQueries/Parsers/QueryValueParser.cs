using System.Diagnostics.CodeAnalysis;
using ConditionalQueries.Interfaces;

namespace ConditionalQueries.Parsers;
public class QueryValueParser : QuerySegmentParser<IQueryValue> {
    protected QueryValueParser() { }
    private static QueryValueParser Instance => new();
    public static IQueryValue[] ParseValues(ReadOnlySpan<char> sql)
        => Instance.ParseCompleteSegment(sql);
    protected override bool TryParsePartialSegment(ReadOnlySpan<char> sql, ref int previousStart, ref int i, [MaybeNullWhen(false)] out IQueryValue res) {
        if (sql[i] != ',') {
            res = null;
            return false;
        }
        res = ParseQueryValue(sql[previousStart..i]);
        previousStart = ++i;
        return true;
    }
    protected override bool TryParsePartialSegmentEnd(ReadOnlySpan<char> sql, int previousStart, [MaybeNullWhen(false)] out IQueryValue res) {
        res = ParseQueryValue(sql[previousStart..]);
        return true;
    }
    public static IQueryValue ParseQueryValue(ReadOnlySpan<char> sql) {
        sql = ExtractCondition(sql, out var condition);
        IQueryValue? res = null;
        foreach (var valueParser in Query.QueryValueParsers)
            if (valueParser(sql, out res))
                break;
        if (res is null)
            throw new Exception();
        if (condition.Length == 1 && Query.SpecialCaseConditions.TryGetValue(condition[0], out var specialCaseParse)) {
            res.Condition = specialCaseParse(res);
            return res;
        }
        res.Condition = ParseCondition(condition);
        return res;
    }
    public static IQueryValueCondition ParseCondition(ReadOnlySpan<char> condition) { 
        foreach (var conditionParser in Query.ConditionParsers)
            if (conditionParser(condition, out var cond))
                return cond;
        throw new Exception();
    }
    public static ReadOnlySpan<char> ExtractCondition(ReadOnlySpan<char> sql, out ReadOnlySpan<char> condition) {
        if (sql.IsEmpty)
            throw new Exception();
        bool inSingleLineComment = false;
        int indexAfterCondition = 0;
        bool inCondition = false;
        condition = sql[0..0];
        for (int i = 0; i < sql.Length - 1; i++) {
            var item = sql[i];
            if (inCondition) {
                if (item == '*' && sql[i + 1] == '/') {
                    condition = sql.ExtractTrim(indexAfterCondition, i);
                    indexAfterCondition = i + 2;
                    inCondition = false;
                    break;
                }
                if (inSingleLineComment && item == '\n') {
                    condition = sql.ExtractTrim(indexAfterCondition, i);
                    indexAfterCondition = i + 1;
                    inCondition = false;
                    break;
                }
                continue;
            }
            if (item == '/' && sql[i + 1] == '*') {
                if (i + 2 < sql.Length && sql[i + 2] == Query.IgnoreCondition)
                    break;
                inCondition = true;
                indexAfterCondition = i + 2;
                i++;
                continue;
            }
            if (item == '-' && sql[i + 1] == '-') {
                inCondition = inSingleLineComment = true;
                indexAfterCondition = i + 2;
                i++;
                continue;
            }
            if (Query.SpecialCaseConditions.ContainsKey(item)) {
                condition = sql[i..(i+1)];
                indexAfterCondition = i + 1;
                break;
            }
            if (item is ' ' or '\r' or '\t' or '\n')
                continue;
            break;
        }
        if (inCondition)
            throw new Exception();
        return sql[indexAfterCondition..];
    }
}
