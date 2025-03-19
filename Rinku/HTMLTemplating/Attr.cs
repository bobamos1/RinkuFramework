using System.Text;

namespace HTMLTemplating;

public struct Attr(string Name, object Value) {
    public string Name = Name;
    public object Value = Value;
    public readonly string SanitizedValue => SanitizeValue(Value.ToString());
    private static readonly char[] SpecialChars = ['&', '<', '>', '"', '\''];
    public static string SanitizeValue(string? value) {
        if (string.IsNullOrEmpty(value))
            return string.Empty;
        if (value.IndexOfAny(SpecialChars) == -1)
            return value;
        var sb = new StringBuilder(value.Length);
        foreach (char c in value)
            switch (c) {
                case '&':
                    sb.Append("&amp;");
                    break;
                case '<':
                    sb.Append("&lt;");
                    break;
                case '>':
                    sb.Append("&gt;");
                    break;
                case '"':
                    sb.Append("&quot;");
                    break;
                case '\'':
                    sb.Append("&#39;");
                    break;
                default:
                    sb.Append(c);
                    break;
            }
        return sb.ToString();
    }
}
public class Attributes : List<Attr>, IAttributes {
    public void FillBuilder(StringBuilder sb) {
        foreach (var attribute in this)
            sb.Append(attribute.Name).Append("=\"").Append(attribute.SanitizedValue).Append("\" ");
        sb.Length--;
    }
}