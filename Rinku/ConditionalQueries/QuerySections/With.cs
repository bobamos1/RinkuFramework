using ConditionalQueries.BaseQuerySection;
using ConditionalQueries.Parsers;
using ConditionalQueries.Interfaces;
using ConditionalQueries.QueryValues;

namespace ConditionalQueries.QuerySections;
public class With : CommaSection<With>, IParsableSection<With> {
    public With(QueryValueSubquery[] Values) : base(Values) { }
    private With(IQueryValue[] Values) : base(Values) { }
    public static string Identifier => "WITH";
    public static With ParseSection(ReadOnlySpan<char> sql)
        => new(QueryValueParser.ParseValues(sql[Identifier.Length..]));
    public override With GetMerged(With section) => new(Values.MergedWith(section.Values));
}