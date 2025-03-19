namespace ConditionalQueries.Interfaces;
public interface IParsableSection<T> where T : IQuerySection {
    public abstract static string Identifier { get; }
    public abstract static T ParseSection(ReadOnlySpan<char> sql);
}
public interface IParsableGroupSection<T> where T : IQuerySection {
    public abstract static T ParseGroupSection(List<IQuerySection> sections);
}
public interface ISectionParser {
    public int IncrementLength { get; }
    public int Group { get; }
    public Type Type { get; }
    public bool IsStartSection(ReadOnlySpan<char> sql, int i);
    public IQuerySection ParseSection(ReadOnlySpan<char> sql);
}

public interface IGroupSectionParser {
    public Type Type { get; }
    public IQuerySection ParseGroupSection(List<IQuerySection> sections);
}
