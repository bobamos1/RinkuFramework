using System.Text;
using ConditionalQueries.Interfaces;

namespace ConditionalQueries.BaseQuerySection;

public abstract class CommaSection<T>(IQueryValue[] Values) : QuerySection<T> where T : CommaSection<T>, IParsableSection<T> {
    public string? PreBuildedSQL { get; set; }
    public IQueryValue[] Values = Values;

    public override void AddColumns(HashSet<string> columns) { }
    public override void AddTables(HashSet<string> table) { }
    public override void AddVariables(HashSet<string> optionalVariables) {
        foreach (var value in Values)
            value.AddVariables(optionalVariables);
    }
    public override bool Valid() {
        foreach (var value in Values)
            if (!value.Valid())
                return false;
        return true;
    }
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
        if (QuerySectionHelper.JoinByComma(sb, conds, Values))
            return true;
        sb.Length -= T.Identifier.Length + 1;
        return false;
    }
    public override void AppendParsable(StringBuilder sb) {
        sb.Append(T.Identifier).Append(' ');
        QuerySectionHelper.AppendParsableByComa(sb, Values);
    }
    public override string ToString() {
        StringBuilder sb = new();
        AppendParsable(sb);
        return sb.ToString();
    }
}