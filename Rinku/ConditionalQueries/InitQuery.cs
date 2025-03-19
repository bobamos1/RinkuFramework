using ConditionalQueries.Interfaces;
using ConditionalQueries.Parsers;
using ConditionalQueries.QuerySections;

namespace ConditionalQueries;
public partial class Query {
    #region variables
    public static char IgnoreCondition { get; set; } = '*';
    public static readonly SectionParser<Drop> Drop = new();
    public static readonly SectionParser<With> With = new();
    public static readonly SectionParser<Insert> Insert = new();
    public static readonly SectionParser<Update> Update = new();
    public static readonly SectionParser<Set> Set = new();
    public static readonly SectionParser<Values> Values = new();
    public static readonly SectionParser<DeleteFrom> DeleteFrom = new();
    public static readonly SectionParser<Delete> Delete = new();
    public static readonly SectionParser<Select> Select = new();
    public static readonly SectionParser<From> From = new();
    public static readonly SectionParser<Join> Join = new(1);
    public static readonly SectionParser<InnerJoin> InnerJoin = new(1);
    public static readonly SectionParser<OuterJoin> OuterJoin = new(1);
    public static readonly SectionParser<LeftJoin> LeftJoin = new(1);
    public static readonly SectionParser<RightJoin> RightJoin = new(1);
    public static readonly SectionParser<CrossJoin> CrossJoin = new(1);
    public static readonly SectionParser<Where> Where = new();
    public static readonly SectionParser<UnionAll> UnionAll = new(2);
    public static readonly SectionParser<Union> Union = new(2);
    public static readonly SectionParser<GroupBy> GroupBy = new();
    public static readonly SectionParser<Having> Having = new();
    public static readonly SectionParser<OrderBy> OrderBy = new();

    public static readonly GroupSectionParser<Joins> Joins = new();
    public static readonly GroupSectionParser<Unions> Unions = new();

    public static readonly TryParseCondition NoCondition = QueryValueConditions.NoCondition.TryParse;
    public static readonly TryParseCondition MultipleRequiredCondition = QueryValueConditions.MultipleRequiredCondition.TryParse;
    public static readonly TryParseCondition MultipleOptionalCondition = QueryValueConditions.MultipleOptionalCondition.TryParse;
    public static readonly TryParseCondition SingleCondition = QueryValueConditions.SingleCondition.TryParse;

    public static readonly TryParseQueryValue QueryValueSubquery = QueryValues.QueryValueSubquery.TryParse;
    public static readonly TryParseQueryValue QueryValueSimple = QueryValues.QueryValueSimple.TryParse;

    public static readonly ParseSpecialCondition VariablesAsConditions = SpecialConditions.VariablesAsConditions.Parse;
    #endregion
    public static Query ParseQuery(ReadOnlySpan<char> SQL) => new(QuerySectionParser.ParseSections(SQL));
    public static implicit operator Query(ReadOnlySpan<char> SQL) => new(QuerySectionParser.ParseSections(SQL));
    public static implicit operator Query(IQuerySection[] Sections) => new(Sections);
    public static implicit operator Query(string SQL) => QuerySectionParser.ParseSections(SQL);
    public static ISectionParser[] Parsers { get; set; } = [
        Drop,
        With,
        Insert,
        Update,
        Set,
        Values,
        DeleteFrom,
        Delete,
        Select,
        From,
        Join,
        InnerJoin,
        OuterJoin,
        LeftJoin,
        RightJoin,
        CrossJoin,
        Where,
        UnionAll,
        Union,
        GroupBy,
        Having,
        OrderBy
    ];
    public static Dictionary<int, IGroupSectionParser> GroupParsers { get; set; } = new() {
        [1] = Joins,
        [2] = Unions
    };
    public static TryParseCondition[] ConditionParsers { get; set; } = [
        NoCondition,
        MultipleRequiredCondition,
        MultipleOptionalCondition,
        SingleCondition
    ];
    public static TryParseQueryValue[] QueryValueParsers { get; set; } = [
        QueryValueSubquery,
        QueryValueSimple
    ];
    public static Dictionary<char, ParseSpecialCondition> SpecialCaseConditions { get; set; } = new() {
        ['#'] = VariablesAsConditions
    };
    public static string[] SubqueriesIdentifiers { get; set; } = [
        QuerySections.With.Identifier,
        QuerySections.Select.Identifier
    ];
    public static string[] PossibleStartingPartSelect { get; set; } = [
        "DISTINCT",
        "TOP(#)"
    ];
}
