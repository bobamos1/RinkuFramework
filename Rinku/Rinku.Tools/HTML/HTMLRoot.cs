using System.Globalization;
using System.Text;
using HTMLTemplating;

namespace Rinku.Tools.HTML;
public class HTMLRoot : HTMLItem {
    public readonly HTMLHead Head;
    public HTMLRoot() {
        Head = new();
        AsParentFor(Head);
    }
    public override void FillBuilderOpening(StringBuilder sb, IHTMLRenderer renderer) {
        sb.Append("<!DOCTYPE html><html lang=\"");
        sb.Append(CultureInfo.CurrentCulture.Name);
        sb.Append("\">");
        Head.FillBuilderBase(sb, renderer);
        sb.Append("<body>");
    }
    public override void FillBuilderClosing(StringBuilder sb, IHTMLRenderer renderer)
        => sb.Append("</body></html>");
    public override IEnumerable<HTMLItem>? Childs() {
        yield return Head;
        if (_childs is null)
            yield break;
        foreach (var child in _childs)
            yield return child;
    }
    public override IEnumerable<HTMLItem>? Childs(IHTMLRenderer renderer) {
        yield return Head;
        IEnumerable<HTMLItem>? childs = null;
        if (renderer.TryGet(this, out var item) && item is HTMLItem elem)
            childs = elem.Childs();
        if (childs is null) {
            if (_childs is null)
                yield break;
            childs = _childs;
        }
        foreach (var child in childs)
            yield return child;
    }
}
public class HTMLHead() : HTMLElementBase<Attributes>("head");
public class HTMLTitle(string BaseTitle) : HTMLElement<Attributes>("title", BaseTitle);