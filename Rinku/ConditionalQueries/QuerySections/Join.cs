using ConditionalQueries.BaseQuerySection;
using ConditionalQueries.Interfaces;

namespace ConditionalQueries.QuerySections;
public class Joins(IQuerySection[] Sections) : GroupSections<Joins>(Sections), IParsableGroupSection<Joins> {
    public static Joins ParseGroupSection(List<IQuerySection> sections) => new([.. sections]);
    public override Joins GetMerged(Joins section) => new(Sections.MergedWith(section.Sections));
}
public class Join(IQueryValue FirstPart, IQueryValue[]? Values) : JoinSectionBase<Join>(FirstPart, Values), IParsableSection<Join> {
    private Join(ValueTuple<IQueryValue, IQueryValue[]?> Values) : this(Values.Item1, Values.Item2) { }
    public static string Identifier => "JOIN";
    public static Join ParseSection(ReadOnlySpan<char> sql) 
        => new(PreParseSection(sql));
    public override Join GetMerged(Join section) => new(FirstPart, ArrayHelper.Merge(Values, section.Values));
}
public class InnerJoin(IQueryValue FirstPart, IQueryValue[]? Values) : JoinSectionBase<InnerJoin>(FirstPart, Values), IParsableSection<InnerJoin> {
    private InnerJoin(ValueTuple<IQueryValue, IQueryValue[]?> Values) : this(Values.Item1, Values.Item2) { }
    public static string Identifier => "INNER JOIN";
    public static InnerJoin ParseSection(ReadOnlySpan<char> sql)
        => new(PreParseSection(sql));
    public override InnerJoin GetMerged(InnerJoin section) => new(FirstPart, ArrayHelper.Merge(Values, section.Values));
}
public class OuterJoin(IQueryValue FirstPart, IQueryValue[]? Values) : JoinSectionBase<OuterJoin>(FirstPart, Values), IParsableSection<OuterJoin> {
    private OuterJoin(ValueTuple<IQueryValue, IQueryValue[]?> Values) : this(Values.Item1, Values.Item2) { }
    public static string Identifier => "OUTER JOIN";
    public static OuterJoin ParseSection(ReadOnlySpan<char> sql) 
        => new(PreParseSection(sql));
    public override OuterJoin GetMerged(OuterJoin section) => new(FirstPart, ArrayHelper.Merge(Values, section.Values));
}
public class LeftJoin(IQueryValue FirstPart, IQueryValue[]? Values) : JoinSectionBase<LeftJoin>(FirstPart, Values), IParsableSection<LeftJoin> {
    private LeftJoin(ValueTuple<IQueryValue, IQueryValue[]?> Values) : this(Values.Item1, Values.Item2) { }
    public static string Identifier => "LEFT JOIN";
    public static LeftJoin ParseSection(ReadOnlySpan<char> sql) 
        => new(PreParseSection(sql));
    public override LeftJoin GetMerged(LeftJoin section) => new(FirstPart, ArrayHelper.Merge(Values, section.Values));
}
public class RightJoin(IQueryValue FirstPart, IQueryValue[]? Values) : JoinSectionBase<RightJoin>(FirstPart, Values), IParsableSection<RightJoin> {
    private RightJoin(ValueTuple<IQueryValue, IQueryValue[]?> Values) : this(Values.Item1, Values.Item2) { }
    public static string Identifier => "RIGHT JOIN";
    public static RightJoin ParseSection(ReadOnlySpan<char> sql)
        => new(PreParseSection(sql));
    public override RightJoin GetMerged(RightJoin section) => new(FirstPart, ArrayHelper.Merge(Values, section.Values));
}
public class CrossJoin(IQueryValue FirstPart, IQueryValue[]? Values) : JoinSectionBase<CrossJoin>(FirstPart, Values), IParsableSection<CrossJoin> {
    private CrossJoin(ValueTuple<IQueryValue, IQueryValue[]?> Values) : this(Values.Item1, Values.Item2) { }
    public static string Identifier => "CROSS JOIN";
    public static CrossJoin ParseSection(ReadOnlySpan<char> sql)
        => new(PreParseSection(sql));
    public override CrossJoin GetMerged(CrossJoin section) => new(FirstPart, ArrayHelper.Merge(Values, section.Values));
}