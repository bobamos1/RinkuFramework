using Rinku.Tools.StringExtensions;

namespace Rinku.Context;
public class PathNavigator {
    public PathNavigator(string path) {
        Segments = GetSegments(path);
        NextSegment();
    }
    public List<string> Segments;
    public int SegmentIndex { get; private set; } = -1;
    public string? CurrentSegment { get; private set; }
    public PathNavigator NextSegment() {
        SegmentIndex++;
        CurrentSegment = SegmentIndex < Segments.Count ? Segments[SegmentIndex] : null;
        return this;
    }
    public bool SameSegmentAs(string segment)
        => CurrentSegment is not null && CurrentSegment.Same(segment);
    public static List<string> GetSegments(string url) {
        List<string> segments = [];
        var previousSlash = 0;
        int i;
        for (i = 0; i < url.Length; i++) {
            var c = url[i];
            if (c == '?')
                break;
            if (c != '/')
                continue;
            if (i == 0) {
                previousSlash = i + 1;
                continue;
            }
            segments.Add(url[previousSlash..i]);
            previousSlash = i + 1;
        }
        if (previousSlash < i)
            segments.Add(url[previousSlash..i]);
        return segments;
    }
}
