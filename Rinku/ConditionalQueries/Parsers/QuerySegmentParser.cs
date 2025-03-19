using System.Diagnostics.CodeAnalysis;

namespace ConditionalQueries.Parsers;
public abstract class QuerySegmentParser<T> {
    protected T[] ParseCompleteSegment(ReadOnlySpan<char> sql) {
        List<T> parsedValues = [];
        int previousStart = 0;
        int parentesisDepth = 0;
        bool inBracket = false;
        bool inQuote = false;
        bool inComment = false;
        bool inSingleLineComment = false;
        for (int i = 0; i < sql.Length; i++) {
            switch (sql[i]) {
                case '(':
                    parentesisDepth++;
                    break;
                case ')':
                    parentesisDepth--;
                    break;
                case '[':
                    inBracket = true;
                    break;
                case ']':
                    inBracket = false;
                    break;
                case '\'':
                    inQuote = !inQuote;
                    break;
                case '/':
                    if (i + 1 < sql.Length && sql[i + 1] == '*')
                        inComment = true;
                    break;
                case '*':
                    if (i + 1 < sql.Length && sql[i + 1] == '/')
                        inComment = false;
                    break;
                case '-':
                    if (i + 1 < sql.Length && sql[i + 1] == '-')
                        inSingleLineComment = true;
                    break;
                case '\n':
                    if (inSingleLineComment)
                        inSingleLineComment = false;
                    break;
                default:
                    if (inComment || inBracket || inQuote || parentesisDepth != 0)
                        break;
                    if (TryParsePartialSegment(sql, ref previousStart, ref i, out var res))
                        parsedValues.Add(res);
                    break;
            }
        }
        if (inSingleLineComment || inComment || inBracket || inQuote || parentesisDepth != 0)
            throw new Exception();
        if (TryParsePartialSegmentEnd(sql, previousStart, out var resEnd))
            parsedValues.Add(resEnd);
        return [.. parsedValues];
    }
    protected abstract bool TryParsePartialSegment(ReadOnlySpan<char> sql, ref int previousStart, ref int i, [MaybeNullWhen(false)] out T res);
    protected abstract bool TryParsePartialSegmentEnd(ReadOnlySpan<char> sql, int previousStart, [MaybeNullWhen(false)] out T res);
}
