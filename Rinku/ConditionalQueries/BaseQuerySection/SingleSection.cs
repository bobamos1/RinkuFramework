using System.Text;
using ConditionalQueries.Interfaces;

namespace ConditionalQueries.BaseQuerySection;

public abstract class SingleSection<T>(IQueryValue Value) : QuerySection<T> where T : SingleSection<T>, IParsableSection<T> {
    public string? PreBuildedSQL { get; set; }
    public IQueryValue Value = Value;

    public override void AddColumns(HashSet<string> columns) { }
    public override void AddTables(HashSet<string> table) { }
    public override void AddVariables(HashSet<string> optionalVariables) => Value.AddVariables(optionalVariables);
    public override bool Valid() => Value.Valid();
    public override bool PreBuild() {
        PreBuildedSQL = null;
        if (!Valid())
            return false;
        var sb = new StringBuilder();
        if (!Parse(sb, Query.EmptyConds))
            return false;
        PreBuildedSQL = sb.ToString();
        return true;
    }
    public override bool Parse(StringBuilder sb, HashSet<string> conds) {
        if (PreBuildedSQL is not null) {
            sb.Append(PreBuildedSQL);
            return true;
        }
        sb.Append(T.Identifier).Append(' ');
        if (Value.Parse(sb, conds))
            return true;
        sb.Length -= T.Identifier.Length;
        return false;
    }
    public override void AppendParsable(StringBuilder sb) {
        sb.Append(T.Identifier).Append(' ');
        Value.AppendParsable(sb);
    }
    public override string ToString() {
        StringBuilder sb = new();
        AppendParsable(sb);
        return sb.ToString();
    }
}