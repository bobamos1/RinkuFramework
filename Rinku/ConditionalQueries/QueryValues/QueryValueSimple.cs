using System.Diagnostics.CodeAnalysis;
using System.Text;
using ConditionalQueries.Interfaces;

namespace ConditionalQueries.QueryValues;
public class QueryValueSimple(string SQL, IQueryValueCondition Condition) : QueryValue(Condition), IParsableQueryValue {
    public string SQL = SQL;
    public override void AppendSQL(string sql) => SQL += sql;
    public override void AppendParsableValue(StringBuilder sb) => sb.Append(SQL);
    public override string ExtractObjectName() => ExtractObjectName(SQL);
    public override bool ParseValue(StringBuilder sb, HashSet<string> conds) {
        sb.Append(SQL);
        return true;
    }
    public static bool TryParse(ReadOnlySpan<char> sql, [MaybeNullWhen(false)] out IQueryValue res) {
        res = new QueryValueSimple(sql.ExtractTrim(0, sql.Length).ToString(), null!);
        return true;
    }
}
