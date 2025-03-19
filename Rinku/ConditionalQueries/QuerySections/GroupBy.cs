using ConditionalQueries.BaseQuerySection;
using ConditionalQueries.Parsers;
using ConditionalQueries.Interfaces;

namespace ConditionalQueries.QuerySections;
public class GroupBy(IQueryValue[] Values) : CommaSection<GroupBy>(Values), IParsableSection<GroupBy> {
    public static string Identifier => "GROUP BY";
    public static GroupBy ParseSection(ReadOnlySpan<char> sql)
        => new(QueryValueParser.ParseValues(sql[Identifier.Length..]));
    public override GroupBy GetMerged(GroupBy section) => new(Values.MergedWith(section.Values));
}