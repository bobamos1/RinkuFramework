using ConditionalQueries.BaseQuerySection;
using ConditionalQueries.Parsers;
using ConditionalQueries.Interfaces;

namespace ConditionalQueries.QuerySections;
public class From(IQueryValue Value) : SingleSection<From>(Value), IParsableSection<From> {
    public static string Identifier => "FROM";
    public static From ParseSection(ReadOnlySpan<char> sql)
        => new(QueryValueParser.ParseQueryValue(sql[Identifier.Length..]));
    public override void AddTables(HashSet<string> tables) => tables.Add(Value.ExtractObjectName());
    public override From GetMerged(From section) => new(Value);
}