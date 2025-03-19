using ConditionalQueries.BaseQuerySection;
using ConditionalQueries.Parsers;
using ConditionalQueries.Interfaces;

namespace ConditionalQueries.QuerySections;
public class Drop(IQueryValue Value) : SingleSection<Drop>(Value), IParsableSection<Drop> {
    public static string Identifier => "DROP";
    public static Drop ParseSection(ReadOnlySpan<char> sql)
        => new(QueryValueParser.ParseQueryValue(sql[Identifier.Length..]));
    public override void AddTables(HashSet<string> tables) => tables.Add(Value.ExtractObjectName());
    public override Drop GetMerged(Drop section) => new(Value);
}