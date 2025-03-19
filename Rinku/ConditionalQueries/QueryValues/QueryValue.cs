using System.Text;
using ConditionalQueries.Interfaces;

namespace ConditionalQueries.QueryValues;
public abstract class QueryValue(IQueryValueCondition Condition) : IQueryValue {
    public abstract void AppendSQL(string sql);
    public IQueryValueCondition Condition { get; set; } = Condition;
    public virtual void AddVariables(HashSet<string> optionalVariables) => Condition.AddVariables(optionalVariables);
    public void AppendParsable(StringBuilder sb) {
        Condition.AppendParsable(sb);
        AppendParsableValue(sb);
    }
    public abstract void AppendParsableValue(StringBuilder sb);
    public abstract string ExtractObjectName();
    public bool Parse(StringBuilder sb, HashSet<string> conds) {
        if (!Condition.Valid(conds))
            return false;
        return ParseValue(sb, conds);
    }
    public abstract bool ParseValue(StringBuilder sb, HashSet<string> conds);
    public bool Valid() => Valid(Query.EmptyConds);
    public virtual bool Valid(HashSet<string> conds) => Condition.Valid(conds);
    public override string ToString() {
        StringBuilder sb = new();
        AppendParsable(sb);
        return sb.ToString();
    }
    public static string ExtractObjectName(string sql) {
        if (sql.EndsWith(']')) {
            for (int i = sql.Length - 2; i >= 0; i--)
                if (sql[i] == '[')
                    return sql[(i + 1)..^1];
            return sql;
        }
        for (int i = sql.Length - 1; i >= 0; i--)
            if (sql[i] is ' ' or '.')
                return sql[(i + 1)..];
        return sql;
    }
}