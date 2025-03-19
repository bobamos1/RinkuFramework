using ConditionalQueries.BaseQuerySection;
using ConditionalQueries.Parsers;
using ConditionalQueries.Interfaces;

namespace ConditionalQueries.QuerySections;
public class OrderBy(IQueryValue[] Values) : CommaSection<OrderBy>(Values), IParsableSection<OrderBy> {
    public static string Identifier => "ORDER BY";
    public static OrderBy ParseSection(ReadOnlySpan<char> sql)
        => new(QueryValueParser.ParseValues(sql[Identifier.Length..]));

    public override OrderBy GetMerged(OrderBy section) => new(Values.MergedWith(section.Values));
}