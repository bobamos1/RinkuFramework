using System.Text;
using ConditionalQueries.Interfaces;

namespace ConditionalQueries.BaseQuerySection;
public static class QuerySectionHelper {
    public static bool JoinByComma(StringBuilder sb, HashSet<string> conditions, IEnumerable<IQuerySegment> segments) {
        bool added = false;
        foreach (var segment in segments)
            if (segment.Parse(sb, conditions)) {
                sb.Append(", ");
                added = true;
            }
        if (added)
            sb.Length -= 2;
        return added;
    }
    public static bool JoinBySpace(StringBuilder sb, HashSet<string> conditions, IEnumerable<IQuerySegment> segments) {
        bool added = false;
        foreach (var segment in segments)
            if (segment.Parse(sb, conditions)) {
                sb.Append(' ');
                added = true;
            }
        if (added)
            sb.Length--;
        return added;
    }
    public static void AppendParsableByComa(StringBuilder sb, IQueryValue[] values) {
        for (int i = 0; i < values.Length; i++) {
            values[i].AppendParsable(sb);
            sb.Append(", ");
        }
        sb.Length -= 2;
    }
    public static void AppendParsableBySpace(StringBuilder sb, IQueryValue[] values) {
        for (int i = 0; i < values.Length; i++) {
            values[i].AppendParsable(sb);
            sb.Append(' ');
        }
        sb.Length--;
    }
    public static void RemoveEndingAndOr(StringBuilder sb) {
        var sbLength = sb.Length;
        if (sb[sbLength - 4] == ' ' && sb[sbLength - 3] is 'A' or 'a' && sb[sbLength - 2] is 'N' or 'n' && sb[sbLength - 1] is 'D' or 'd')
            sb.Length -= 4;
        else if (sb[sbLength - 3] == ' ' && sb[sbLength - 2] is 'O' or 'o' && sb[sbLength - 1] is 'R' or 'r')
            sb.Length -= 3;
    }
}
