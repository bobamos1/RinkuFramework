using System.Text;

namespace HTMLTemplating;
public class HTMLRaw(string ContentBefore, string? ContentAfter = null) : HTMLItem {
    public static readonly HTMLRaw Empty = new("");
    public static implicit operator HTMLRaw(string SQL) => new(SQL);
    public string ContentBefore = ContentBefore;
    public string? ContentAfter = ContentAfter;
    public override void FillBuilderClosing(StringBuilder sb, IHTMLRenderer renderer) => sb.Append(ContentBefore);
    public override void FillBuilderOpening(StringBuilder sb, IHTMLRenderer renderer) => sb.Append(ContentAfter);
    public override void FillBuilderChilds(StringBuilder sb, IHTMLRenderer renderer) {
        if (string.IsNullOrEmpty(ContentAfter))
            return;
        base.FillBuilderChilds(sb, renderer);
    }
}
