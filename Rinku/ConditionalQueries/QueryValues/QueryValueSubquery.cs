using System.Diagnostics.CodeAnalysis;
using System.Text;
using ConditionalQueries.Interfaces;

namespace ConditionalQueries.QueryValues;
public class QueryValueSubquery(string SQLBefore, Query Subquery, string SQLAfter, IQueryValueCondition Condition) : QueryValue(Condition), IParsableQueryValue {
    public string SQLBefore = SQLBefore;
    public Query Subquery = Subquery;
    public string SQLAfter = SQLAfter;
    public override void AppendSQL(string sql) => SQLAfter += sql;
    public override bool Valid(HashSet<string> conds) => base.Valid(conds) && Subquery.QueryString is not null;
    public override void AppendParsableValue(StringBuilder sb) {
        if (SQLBefore.Length == 0)
            sb.Append('(');
        else
            sb.Append(SQLBefore).Append(" (");
        Query.AppendParsableSections(sb, Subquery.Sections);
        if (SQLAfter.Length == 0)
            sb.Append(')');
        else
            sb.Append(") ").Append(SQLAfter);
    }

    public override string ExtractObjectName() => ExtractObjectName(SQLAfter);

    public override bool ParseValue(StringBuilder sb, HashSet<string> conds) {
        if (SQLBefore.Length == 0)
            sb.Append('(');
        else
            sb.Append(SQLBefore).Append(" (");
        if (!Subquery.Parse(sb, conds)) {
            sb.Length -= SQLBefore.Length + 1;
            return false;
        }
        if (SQLAfter.Length == 0)
            sb.Append(')');
        else
            sb.Append(") ").Append(SQLAfter);
        return true;
    }
    public static bool IsSubqueryStart(ReadOnlySpan<char> sql, int i, out int inc) {
        foreach (var identifier in Query.SubqueriesIdentifiers)
            if (sql.StartingWith(identifier, i)) {
                inc = identifier.Length;
                return true;
            }
        inc = 0;
        return false;
    }
    public static bool TryParse(ReadOnlySpan<char> sql, [MaybeNullWhen(false)] out IQueryValue res) {
        var parentesisLevel = 0;
        var hasSubquery = false;
        bool checkForSubQuery = false;
        bool inBracket = false;
        bool inQuote = false;
        int indexStartNewQuery = -1;
        for (int i = 0; i < sql.Length; i++) {
            var item = sql[i];
            if (item == '[')
                inBracket = true;
            if (item == '\'') {
                inQuote = !inQuote;
                continue;
            }
            if (inBracket || inQuote) {
                if (item == ']')
                    inBracket = false;
                continue;
            }
            if (checkForSubQuery) {
                if (item is ' ' or '\r' or '\n')
                    continue;
                if (IsSubqueryStart(sql, i, out var inc)) {
                    indexStartNewQuery = i;
                    i += inc;
                    hasSubquery = true;
                }
            }
            checkForSubQuery = false;

            if (item == '(') {
                parentesisLevel++;
                if (!hasSubquery)
                    checkForSubQuery = true;
                continue;
            }
            if (item == ')') {
                parentesisLevel--;
                if (hasSubquery && parentesisLevel == 0) {
                    res = new QueryValueSubquery(
                        sql.ExtractTrim(0, indexStartNewQuery - 1).ToString(),
                        Query.ParseQuery(sql[indexStartNewQuery..i]),
                        sql.ExtractTrim(i + 1, sql.Length).ToString(),
                        null!
                    );
                    return true;
                }
            }
        }
        if (checkForSubQuery || hasSubquery || parentesisLevel is > 0 or < -1)
            throw new Exception();
        res = null;
        return false;
    }
}
