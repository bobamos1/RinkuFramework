using ConditionalQueries.BaseQuerySection;
using ConditionalQueries.Parsers;
using ConditionalQueries.Interfaces;

namespace ConditionalQueries.QuerySections;
public class Having(IQueryValue[] Values) : ConditionSection<Having>(Values), IParsableSection<Having> {
    public static string Identifier => "HAVING";
    public static Having ParseSection(ReadOnlySpan<char> sql)
        => new(QueryValueParser.ParseValues(sql[Identifier.Length..]));
    public override Having GetMerged(Having section) => new(Values.MergedWith(section.Values));
}