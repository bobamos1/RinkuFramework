using ConditionalQueries.Interfaces;

namespace ConditionalQueries.Parsers;

public readonly record struct SectionParser<T>(int Group = 0) : ISectionParser where T : IParsableSection<T>, IQuerySection {
    public readonly int IncrementLength => T.Identifier.Length;
    public readonly Type Type => typeof(T);
    public readonly bool IsStartSection(ReadOnlySpan<char> sql, int i) {
        return sql.StartingWith(T.Identifier, i)
            && (i - 1 < 0 || char.IsWhiteSpace(sql[i - 1]))
            && (i + 1 < sql.Length || char.IsWhiteSpace(sql[i + 1]));
    }
    public readonly IQuerySection ParseSection(ReadOnlySpan<char> sql) => T.ParseSection(sql);
}
public readonly record struct GroupSectionParser<T> : IGroupSectionParser where T : IParsableGroupSection<T>, IQuerySection {
    public readonly Type Type => typeof(T);
    public readonly IQuerySection ParseGroupSection(List<IQuerySection> sections) => T.ParseGroupSection(sections);
}