using ConditionalQueries.BaseQuerySection;
using ConditionalQueries.Parsers;
using ConditionalQueries.Interfaces;

namespace ConditionalQueries.QuerySections;
public class Delete(IQueryValue Value) : SingleSection<Delete>(Value), IParsableSection<Delete> {
    public static string Identifier => "DELETE";
    public static Delete ParseSection(ReadOnlySpan<char> sql)
        => new(QueryValueParser.ParseQueryValue(sql[Identifier.Length..]));
    public override void AddTables(HashSet<string> tables) => tables.Add(Value.ExtractObjectName());

    public override Delete GetMerged(Delete section) => new(Value);
}
public class DeleteFrom(IQueryValue Value) : SingleSection<DeleteFrom>(Value), IParsableSection<DeleteFrom> {
    public static string Identifier => "DELETE FROM";
    public static DeleteFrom ParseSection(ReadOnlySpan<char> sql)
        => new(QueryValueParser.ParseQueryValue(sql[Identifier.Length..]));
    public override void AddTables(HashSet<string> tables) => tables.Add(Value.ExtractObjectName());
    public override DeleteFrom GetMerged(DeleteFrom section) => new(Value);
}