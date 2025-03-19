using ConditionalQueries.BaseQuerySection;
using ConditionalQueries.Parsers;
using ConditionalQueries.Interfaces;

namespace ConditionalQueries.QuerySections;
public class Set(IQueryValue[] Values) : CommaSection<Set>(Values), IParsableSection<Set> {
    public static string Identifier => "SET";
    public static Set ParseSection(ReadOnlySpan<char> sql)
        => new(QueryValueParser.ParseValues(sql[Identifier.Length..]));
    public override Set GetMerged(Set section) => new(Values.MergedWith(section.Values));
}