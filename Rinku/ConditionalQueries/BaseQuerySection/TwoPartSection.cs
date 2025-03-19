using System.Text;
using ConditionalQueries.Interfaces;

namespace ConditionalQueries.BaseQuerySection;

public abstract class TwoPartSection<T>(IQueryValue FirstPart, IQueryValue[]? Values) : QuerySection<T> where T : TwoPartSection<T>, IParsableSection<T> {
    public IQueryValue FirstPart = FirstPart;
    public IQueryValue[]? Values = Values;
    public string? PreBuildedSQL { get; set; }

    public override void AddColumns(HashSet<string> columns) { }
    public override void AddTables(HashSet<string> table) { }
    public override void AddVariables(HashSet<string> optionalVariables) {
        FirstPart.AddVariables(optionalVariables);
        if (Values is null)
            return;
        foreach (var value in Values)
            value.AddVariables(optionalVariables);
    }
    public override bool Valid() {
        if (!FirstPart.Valid())
            return false;
        if (Values is null)
            return true;
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
        int lenBeforeAdd = sb.Length;
        sb.Append(T.Identifier).Append(' ');
        if (FirstPart.Parse(sb, conds)) {
            sb.Append(' ');
            if (ParseValues(sb, conds))
                return true;
        }
        sb.Length = lenBeforeAdd;
        return false;
    }
    public abstract bool ParseValues(StringBuilder sb, HashSet<string> conds);
    public override string ToString() {
        StringBuilder sb = new();
        AppendParsable(sb);
        return sb.ToString();
    }
}