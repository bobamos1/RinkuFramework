using ConditionalQueries.BaseQuerySection;
using ConditionalQueries.Parsers;
using ConditionalQueries.Interfaces;

namespace ConditionalQueries.QuerySections;
public class Update(IQueryValue Value) : SingleSection<Update>(Value), IParsableSection<Update> {
    public static string Identifier => "UPDATE";
    public static Update ParseSection(ReadOnlySpan<char> sql)
        => new(QueryValueParser.ParseQueryValue(sql[Identifier.Length..]));
    public override void AddTables(HashSet<string> tables) => tables.Add(Value.ExtractObjectName());
    public override Update GetMerged(Update section) => new(Value);
}