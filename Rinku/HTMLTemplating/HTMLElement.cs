using System.Text;

namespace HTMLTemplating;
public interface IAttributes {
    public void FillBuilder(StringBuilder sb);
}
public class HTMLElementBase<T>(string TagName, T? Attributes = default) : HTMLItem where T : IAttributes {
    public string TagName = TagName;
    public T? Attributes = Attributes;
    public override void FillBuilderOpening(StringBuilder sb, IHTMLRenderer renderer) {
        sb.Append('<').Append(TagName);
        if (Attributes is not null) {
            sb.Append(' ');
            Attributes.FillBuilder(sb);
        }
        sb.Append('>');
    }
    public override void FillBuilderClosing(StringBuilder sb, IHTMLRenderer renderer) => sb.Append("</").Append(TagName).Append('>');
}
public class HTMLElement<T>(string TagName, object? Content = default, T? Attributes = default) : HTMLElementBase<T>(TagName, Attributes) where T : IAttributes {
    public object? Content = Content;
    public override void FillBuilderContent(StringBuilder sb) => sb.Append(SanitizeContent(Content?.ToString()));
}
public class HTMLElement(string TagName, object? Content = null, Attributes? Attributes = default)
    : HTMLElement<Attributes>(TagName, Content, Attributes);