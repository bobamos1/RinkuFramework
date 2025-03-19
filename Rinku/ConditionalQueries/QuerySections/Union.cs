using ConditionalQueries.BaseQuerySection;
using ConditionalQueries.Interfaces;

namespace ConditionalQueries.QuerySections;
public class Unions(IQuerySection[] Sections) : GroupSections<Unions>(Sections), IParsableGroupSection<Unions> {
    public static Unions ParseGroupSection(List<IQuerySection> sections) => new([.. sections]);
    public override Unions GetMerged(Unions section) => new(Sections.MergedWith(section.Sections));
}
public class Union(IQueryValueCondition Condition, IQuerySection[] Sections) : GroupSectionWithCondition<Union>(Condition, Sections), IParsableSection<Union> {
    public static string Identifier => "UNION";
    private Union((IQueryValueCondition, IQuerySection[]) init) : this(init.Item1, init.Item2) { }
    public static Union ParseSection(ReadOnlySpan<char> sql)
        => new(PreParseSection(sql[Identifier.Length..]));
    public override Union GetMerged(Union section) => new(Condition, Sections.MergedWith(section.Sections));
}
public class UnionAll(IQueryValueCondition Condition, IQuerySection[] Sections) : GroupSectionWithCondition<UnionAll>(Condition, Sections), IParsableSection<UnionAll> {
    public static string Identifier => "UNION ALL";
    private UnionAll((IQueryValueCondition, IQuerySection[]) init) : this(init.Item1, init.Item2) {}
    public static UnionAll ParseSection(ReadOnlySpan<char> sql)
        => new(PreParseSection(sql[Identifier.Length..]));
    public override UnionAll GetMerged(UnionAll section) => new(Condition, Sections.MergedWith(section.Sections));
}