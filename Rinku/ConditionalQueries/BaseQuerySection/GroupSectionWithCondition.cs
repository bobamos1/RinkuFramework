using System.Text;
using ConditionalQueries.Parsers;
using ConditionalQueries.Interfaces;

namespace ConditionalQueries.BaseQuerySection;
public abstract class GroupSectionWithCondition<T>(IQueryValueCondition Condition, IQuerySection[] Sections) : GroupSections<T>(Sections) where T : GroupSectionWithCondition<T>, IParsableSection<T> {
    public IQueryValueCondition Condition = Condition;
    public override bool Valid() => Condition.Valid(Query.EmptyConds) && base.Valid();
    public override bool PreBuild() {
        if (PreBuildSections() && Condition.Valid(Query.EmptyConds)) {
            PreBuildedSQL = Parse();
            return true;
        }
        PreBuildedSQL = null;
        return false;
    }
    public override bool Parse(StringBuilder sb, HashSet<string> conds) {
        if (PreBuildedSQL is not null) {
            sb.Append(PreBuildedSQL);
            return true;
        }
        if (!Condition.Valid(conds))
            return false;
        sb.Append(T.Identifier).Append(' ');
        if (!base.Parse(sb, conds)) {
            sb.Length -= T.Identifier.Length + 1;
            return false;
        }
        return true;
    }
    public override void AppendParsable(StringBuilder sb) {
        sb.Append(T.Identifier).Append(' ');
        Condition.AppendParsable(sb);
        sb.Append(' ');
        base.AppendParsable(sb);
    }
    public static (IQueryValueCondition, IQuerySection[]) PreParseSection(ReadOnlySpan<char> sql) {
        sql = QueryValueParser.ExtractCondition(sql, out var condition);
        return (QueryValueParser.ParseCondition(condition), QuerySectionParser.ParseSections(sql));
    }
}