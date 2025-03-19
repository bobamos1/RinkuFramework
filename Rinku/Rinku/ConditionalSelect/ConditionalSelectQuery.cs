using System.Text;
using ConditionalQueries;
using ConditionalQueries.Interfaces;
using ConditionalQueries.Parsers;
using ConditionalQueries.QuerySections;
using DataTable;
using Rinku.Context;

namespace Rinku.ConditionalSelect;
public class ConditionalSelectQuery<T>(IQuerySection[] Sections, ColWithCond<T>[] Columns) : Query(Sections) where T : IContext {
    public ConditionalSelectQuery(string SQL, ColWithCond<T>[] Columns) 
        : this(QuerySectionParser.ParseSections(SQL), Columns) { }
    public ColWithCond<T>[] Columns = Columns;
    public bool[] IsAddingCond = null!;
    public KeyValuePair<IQuerySection, int> SelectSectionAndIndex;

    public override void PreBuild() {
        base.PreBuild();
        IsAddingCond = new bool[Columns.Length];
        QueryString = null;
        var optVars = base.GetOptionalVariables();
        HashSet<string> condsCol = [];
        for (int i = 0; i < Sections.Length; i++)
            if (Sections[i].GetType() == typeof(Select)) {
                SelectSectionAndIndex = new(Sections[i], i);
                break;
            }
        if (SelectSectionAndIndex.Key is null)
            throw new Exception();
        for (int i = 0; i < Columns.Length; i++) {
            Columns[i].Column.AddColAsConds(condsCol);
            foreach (var cond in condsCol)
                if (optVars.Contains(cond)) {
                    IsAddingCond[i] = true;
                    break;
                }
            condsCol.Clear();
        }
    }
    public override HashSet<string> GetColumns() {
        var cols = base.GetColumns();
        foreach (var column in Columns)
            column.Column.AddColumns(cols);
        cols.Remove("*");
        return cols;
    }
    public string? Parse(T ctx, HashSet<string> conds, IDTMaker dtMaker) {
        var sb = new StringBuilder();
        SelectSectionAndIndex.Key.Parse(sb, conds);
        sb.Length--;
        var lenBefore = sb.Length;
        for (int i = 0; i < Columns.Length; i++)
            if (Columns[i].TryParseSQL(sb, ctx, dtMaker)) {
                sb.Append(", ");
                if (IsAddingCond[i])
                    Columns[i].Column.AddColAsConds(conds);
            }
        if (sb.Length == lenBefore)
            return null;
        sb.Length -= 2;
        sb.Append(' ');
        if (SelectSectionAndIndex.Value == 0)
            return ParseSectionsFrom(1, sb, conds);
        Span<char> selectPart = new char[sb.Length];
        sb.CopyTo(0, selectPart, sb.Length);
        sb.Length = 0;
        if (SelectSectionAndIndex.Value == 1) {
            if (Sections[0].Parse(sb, conds))
                sb.Append(' ');
            sb.Append(selectPart);
            return ParseSectionsFrom(2, sb, conds);
        }
        for (int i = 0; i < SelectSectionAndIndex.Value; i++)
            if (Sections[i].Parse(sb, conds))
                sb.Append(' ');
        sb.Append(selectPart);
        return ParseSectionsFrom(SelectSectionAndIndex.Value + 1, sb, conds);
    }
    public string ParseSectionsFrom(int i, StringBuilder sb, HashSet<string> conds) {
        for (; i < Sections.Length; i++)
            if (Sections[i].Parse(sb, conds))
                sb.Append(' ');
        sb.Length--;
        return sb.ToString();
    }
    public ConditionalSelectQuery<T> MergeWith(ConditionalSelectQuery<T> query)
        => new(GetMergedSections(this, query), Columns.MergedWith(query.Columns));
}
