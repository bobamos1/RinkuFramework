using System.Text;

namespace HTMLTemplating;
public class HTMLPlaceholder(string ID) : HTMLItem {
    public string ID { get; } = ID;
    public override void FillBuilderClosing(StringBuilder sb, IHTMLRenderer renderer) { }
    public override void FillBuilderOpening(StringBuilder sb, IHTMLRenderer renderer) { }
}
public static class PlaceholderExtension {
    public static HTMLPlaceholder? FindPlaceholder(this HTMLItem elem, string id) => elem.FindPlaceholder<HTMLPlaceholder>(id);
    public static T? FindPlaceholder<T>(this HTMLItem elem, string id) where T : HTMLPlaceholder => elem.Find<T>(e => e.ID == id);
    public static HTMLPlaceholder? FindPlaceholder(this HTMLRenderer renderer, string id) => renderer.FindPlaceholder<HTMLPlaceholder>(id);
    public static T? FindPlaceholder<T>(this HTMLRenderer renderer, string id) where T : HTMLPlaceholder => renderer.Find<T>(e => e.ID == id);
}