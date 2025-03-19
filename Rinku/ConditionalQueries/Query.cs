using System.Text;
using ConditionalQueries.Interfaces;

namespace ConditionalQueries;
public partial class Query : IQuerySegment {
    public static readonly HashSet<string> EmptyConds = [];
    public readonly IQuerySection[] Sections;
    public string? QueryString { get; protected set; }
    public Query(IQuerySection[] Sections) {
        this.Sections = Sections;
        PreBuild();
    }
    public override string ToString() => GetParsableQuery();
    #region getters
    public virtual HashSet<string> GetTables() {
        HashSet<string> tables = [];
        foreach (var section in Sections)
            section.AddTables(tables);
        return tables;
    }
    public virtual HashSet<string> GetColumns() {
        HashSet<string> columns = [];
        foreach (var section in Sections)
            section.AddColumns(columns);
        return columns;
    }
    public virtual HashSet<string> GetOptionalVariables() {
        HashSet<string> optionalVariables = [];
        foreach (var section in Sections)
            section.AddVariables(optionalVariables);
        return optionalVariables;
    }
    public virtual HashSet<string> GetSQLVariables() => GetSQLVariables(GetOptionalVariables());
    public virtual HashSet<string> GetSQLVariables(HashSet<string> optionalVariables) {
        HashSet<string> sqlVariables = [];
        AppendVariables(Parse(optionalVariables) ?? string.Empty, sqlVariables);
        return sqlVariables;
    }
    public static void AppendVariables(string SQL, HashSet<string> variables) {
        for (int i = 0; i < SQL.Length; i++)
            if (SQL[i] == '@') {
                var vari = GetVariable(SQL, i + 1);
                if (vari is not null)
                    variables.Add(vari);
            }
    }
    public static string? GetVariable(string SQL, int indexOfAt) {
        for (int j = indexOfAt + 1; j < SQL.Length; j++)
            if (SQL[j] is ' ' or '=' or ',' or ')')
                return SQL[(indexOfAt + 1)..j];
        return SQL[(indexOfAt + 1)..];
    }
    #endregion
    #region compiling
    public virtual void PreBuild() {
        QueryString = null;
        var allSectionValid = true;
        foreach (var section in Sections)
            if (!section.PreBuild())
                allSectionValid = false;
        if (!allSectionValid)
            return;
        var sb = new StringBuilder();
        AppendSections(sb, EmptyConds, Sections);
        QueryString = sb.ToString();
    }
    #endregion
    #region parsing
    public string? Parse() => Parse(EmptyConds);
    public string? Parse(HashSet<string> conds) {
        if (QueryString is not null)
            return QueryString;
        var sb = new StringBuilder();
        if (!AppendSections(sb, conds, Sections))
            return null;
        return sb.ToString();
    }
    public virtual bool Parse(StringBuilder sb, HashSet<string> conds) {
        if (QueryString is null)
            return AppendSections(sb, conds, Sections);
        sb.Append(QueryString);
        return true;
    }
    public static bool AppendSections(StringBuilder sb, HashSet<string> conds, IQuerySection[] sections) {
        bool sectionAdded = false;
        foreach (var section in sections) {
            if (section.Parse(sb, conds)) {
                sectionAdded = true;
                sb.Append(' ');
            }
        }
        if (sectionAdded)
            sb.Length--;
        return sectionAdded;
    }
    public string GetParsableQuery() {
        var sb = new StringBuilder();
        AppendParsableSections(sb, Sections);
        return sb.ToString();
    }
    public static void AppendParsableSections(StringBuilder sb, IQuerySection[] sections) {
        foreach (var section in sections) {
            section.AppendParsable(sb);
            sb.Append(' ');
        }
        sb.Length--;
    }
    #endregion
    public Query MergeWith(Query query) => new(GetMergedSections(this, query));
    public static IQuerySection[] GetMergedSections(Query query1, Query query2) {
        int i1 = 0;
        int i2 = 0;
        List<IQuerySection> sections = [];
        foreach (var parser in Parsers) {
            if (i1 >= query1.Sections.Length) {
                FinishFill(sections, i2, query2.Sections);
                break;
            }
            if (i2 >= query2.Sections.Length) {
                FinishFill(sections, i1, query1.Sections);
                break;
            }
            if (query1.Sections[i1].GetType() == query2.Sections[i2].GetType()) {
                sections.Add(query1.Sections[i1].GetMerged(query2.Sections[i2]));
                i1++;
                i2++;
            }
            else if (query1.Sections[i1].GetType() == parser.Type) {
                sections.Add(query1.Sections[i1]);
                i1++;
            }
            else if (query2.Sections[i2].GetType() == parser.Type) {
                sections.Add(query2.Sections[i2]);
                i2++;
            }
        }
        return [.. sections];
    }
    public static void FinishFill(List<IQuerySection> newSections, int i, IQuerySection[] oldSections) {
        for (; i < oldSections.Length; i++)
            newSections.Add(oldSections[i]);
    }
}
