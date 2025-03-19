using ConditionalQueries.BaseQuerySection;
using ConditionalQueries.Parsers;
using ConditionalQueries.Interfaces;

namespace ConditionalQueries.QuerySections;
public class Where(IQueryValue[] Values) : ConditionSection<Where>(Values), IParsableSection<Where> {
    public static string Identifier => "WHERE";
    public static Where ParseSection(ReadOnlySpan<char> sql)
        => new(QueryConditionParser.ParseConditions(sql[Identifier.Length..]));
    public override Where GetMerged(Where section) => new(Values.MergedWith(section.Values));
}