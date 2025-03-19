using System.Diagnostics.CodeAnalysis;
using ConditionalQueries.Interfaces;

namespace ConditionalQueries.Parsers;
public interface IJoinGroup;
public class QuerySectionParser : QuerySegmentParser<IQuerySection> {
    public static IQuerySection[] ParseSections(ReadOnlySpan<char> SQL)
        => new QuerySectionParser().ParseCompleteSegment(SQL);
    public ISectionParser? PreviousParser;
    public List<IQuerySection>? GroupBuffer;
    public int SectionIndexToLookFrom = 0;
    public void SetSectionIndexToLookFrom(int index, ISectionParser parser) {
        if (parser.Group > 0) {
            index--;
            for (; index >= 0; index--)
                if (Query.Parsers[index].Group != parser.Group)
                    break;
        }
        SectionIndexToLookFrom = index + 1;
    }
    protected override bool TryParsePartialSegment(ReadOnlySpan<char> sql, ref int previousStart, ref int i, [MaybeNullWhen(false)] out IQuerySection res) {
        int j = SectionIndexToLookFrom;
        for (; j < Query.Parsers.Length; j++)
            if (Query.Parsers[j].IsStartSection(sql, i))
                break;
        res = null;
        if (j == Query.Parsers.Length)
            return false;
        var parser = Query.Parsers[j];
        SetSectionIndexToLookFrom(j, parser);
        if (PreviousParser is not null)
            res = PreviousParser.ParseSection(sql[previousStart..i]);
        previousStart = i;
        i += parser.IncrementLength;
        if (res is null) {
            PreviousParser = parser;
            return false;
        }
        return HandleGroup(ref res, parser);
    }
    protected override bool TryParsePartialSegmentEnd(ReadOnlySpan<char> sql, int previousStart, [MaybeNullWhen(false)] out IQuerySection res) {
        if (PreviousParser is null) {
            res = null;
            return false;
        }
        res = PreviousParser.ParseSection(sql[previousStart..]);
        return HandleGroup(ref res, null);
    }
    public virtual bool HandleGroup(ref IQuerySection section, ISectionParser? newParser) {
        if (PreviousParser!.Group != 0) {
            GroupBuffer ??= [];
            GroupBuffer.Add(section);
            if (newParser is not null && PreviousParser.Group == newParser.Group) {
                PreviousParser = newParser;
                return false;
            }
        }
        if (GroupBuffer is not null && GroupBuffer.Count != 0) {
            section = Query.GroupParsers[PreviousParser.Group].ParseGroupSection(GroupBuffer);
            GroupBuffer.Clear();
        }
        PreviousParser = newParser;
        return true;
    }
}
