using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;
using ConditionalQueries;
using ConditionalQueries.Interfaces;
using ConditionalQueries.QueryValues;
using Rinku.Tools.Globalization;

namespace Rinku.Tools.Queries;
public class QueryValueWord(string SQLBefore, string SQLAfter, Word SQLWord, IQueryValueCondition Condition) : QueryValue(Condition) {
    public override void AppendSQL(string sql) => SQLAfter += sql;
    public string SQLBefore = SQLBefore;
    public Word SQLWord = SQLWord;
    public string SQLAfter = SQLAfter;
    public override bool Valid(HashSet<string> conds) => false;
    public override void AppendParsableValue(StringBuilder sb) {
        sb.Append("/*").Append(Query.IgnoreCondition);
        foreach (var trad in SQLWord) {
            if (trad.Key == CultureInfo.InvariantCulture)
                continue;
            sb.Append(trad.Key).Append('|').Append(trad.Value).Append('&');
        }
        sb.Length--;
        sb.Append("*/").Append(SQLBefore).Append('{')
            .Append(SQLWord.GetTranslation(CultureInfo.InvariantCulture))
            .Append('}').Append(SQLAfter);
    }
    public override string ExtractObjectName() => ExtractObjectName(SQLAfter);
    public override bool ParseValue(StringBuilder sb, HashSet<string> conds) {
        if (SQLAfter.Length > 0)
            sb.Append(SQLBefore);
        sb.Append(SQLWord);
        if (SQLAfter.Length > 0)
            sb.Append(SQLAfter);
        return true;
    }
    public static bool TryParse(ReadOnlySpan<char> sql, [MaybeNullWhen(false)] out IQueryValue res) {
        var startTrads = 0;
        var endTrads = 0;
        var openBracketIndex = 0;
        var closeBracketIndex = 0;
        var nbTrad = 0;
        res = null;
        if (sql.Length < 3)
            return false;
        var currentCheck = '/';
        for (int i = 0; i < sql.Length; i++) {
            var c = sql[i];
            if (char.IsWhiteSpace(c))
                continue;
            if (c == '|')
                if (startTrads > 0 && endTrads == 0)
                    nbTrad++;
            if (c != currentCheck) {
                if (currentCheck == '/')
                    return false;
                continue;
            }
            if (currentCheck == '/') {
                if (i + 2 >= sql.Length || sql[i + 1] != '*')
                    return false;
                if (sql[i + 2] == Query.IgnoreCondition)
                    i++;
                i++;
                startTrads = i + 1;
                currentCheck = '*';
                continue;
            }
            if (currentCheck == '*') {
                if (nbTrad == 0)
                    return false;
                if (i + 1 >= sql.Length || sql[i + 1] != '/')
                    return false;
                endTrads = i;
                i++;
                currentCheck = '{';
                continue;
            }
            if (currentCheck == '{') {
                openBracketIndex = i;
                currentCheck = '}';
                continue;
            }
            if (currentCheck == '}') {
                closeBracketIndex = i + 1;
                break;
            }
        }
        if (endTrads == 0 || openBracketIndex + 1 >= sql.Length)
            return false;
        Word word = new(nbTrad);
        var startTrad = 0;
        for (int j = startTrads; j < endTrads; j++) {
            if (sql[j] == '|')
                startTrad = j;
            else if (sql[j] == '&') {
                if (!TryGetCulture(sql, startTrads, startTrad, out var culture))
                    return false;
                word.Add(culture, sql.ExtractTrim(startTrad + 1, j).ToString());
                startTrads = j + 1;
            }
        }
        if (!TryGetCulture(sql, startTrads, startTrad, out var lastCulture))
            return false;
        word.Add(lastCulture, sql[(startTrad + 1)..endTrads].ToString());
        string invariant;
        string sqlBefore;
        string sqlAfter;
        if (openBracketIndex == 0) {
            sqlAfter = sqlBefore = "";
            invariant = sql.ExtractTrim(endTrads + 2, sql.Length).ToString();
        }
        else {
            invariant = sql.ExtractTrim(openBracketIndex + 1, closeBracketIndex - 1).ToString();
            sqlBefore = sql.ExtractTrim(endTrads + 2, openBracketIndex + 1)[..^1].ToString();
            sqlAfter = sql.ExtractTrim(closeBracketIndex - 1, sql.Length)[1..].ToString();
        }
        word.Add(CultureInfo.InvariantCulture, invariant);
        res = new QueryValueWord(sqlBefore, sqlAfter, word, null!);
        return true;
    }
    public static bool TryGetCulture(ReadOnlySpan<char> sql, int start, int end, [MaybeNullWhen(false)] out CultureInfo culture) {
        try {
            culture = new CultureInfo(sql.ExtractTrim(start, end).ToString());
            return true;
        }
        catch {
            culture = null;
            return false;
        }
    }
}
