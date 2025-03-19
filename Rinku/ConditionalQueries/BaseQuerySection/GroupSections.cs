using System.Text;
using ConditionalQueries.Interfaces;

namespace ConditionalQueries.BaseQuerySection;
public abstract class GroupSections<T>(IQuerySection[] Sections) : QuerySection<T> where T : GroupSections<T> {
    public string? PreBuildedSQL { get; set; }
    public IQuerySection[] Sections = Sections;
    public override void AddColumns(HashSet<string> columns) { 
        foreach (var section in Sections)
            section.AddColumns(columns);
    }
    public override void AddTables(HashSet<string> table) {
        foreach (var section in Sections)
            section.AddTables(table);
    }
    public override void AddVariables(HashSet<string> optionalVariables) {
        foreach (var section in Sections)
            section.AddVariables(optionalVariables);
    }
    public override bool Valid() {
        foreach (var section in Sections)
            if (!section.Valid())
                return false;
        return true;
    }
    public override bool PreBuild() {
        if (!PreBuildSections()) {
            PreBuildedSQL = null;
            return false;
        }
        PreBuildedSQL = Parse();
        return true;
    }
    public string Parse() {
        StringBuilder sb = new();
        Parse(sb, Query.EmptyConds);
        return sb.ToString();
    }
    public bool PreBuildSections() {
        var completlyPrebuilt = true;
        foreach (var section in Sections)
            if (!section.PreBuild())
                completlyPrebuilt = false;
        return completlyPrebuilt;
    }
    public override bool Parse(StringBuilder sb, HashSet<string> conds) {
        if (PreBuildedSQL is null)
            return Query.AppendSections(sb, conds, Sections);
        sb.Append(PreBuildedSQL);
        return true;
    }
    public override void AppendParsable(StringBuilder sb) => Query.AppendParsableSections(sb, Sections);
    public override string ToString() {
        StringBuilder sb = new();
        AppendParsable(sb);
        return sb.ToString();
    }
}