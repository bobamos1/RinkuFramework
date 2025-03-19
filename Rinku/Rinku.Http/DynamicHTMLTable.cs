using System.Text;
using HTMLTemplating;
using Rinku.Tools.Data;
using Rinku.Tools.HTML;
namespace Rinku.Http;
/*
public interface IParsable<T> {
    public T Parse(int index, DataTable dt);
}
*/
/*
public class DynamicHTMLTable(DataTable DT, IParsable<HTMLItem>[] Parsers) : HTMLItem {
    public DataTable DT = DT;
    public IParsable<HTMLItem>[] Parsers = Parsers;
    public override void FillBuilderOpening(StringBuilder sb, IHTMLRenderer renderer) {
        sb.Append("<table>");
        for (int i = 0; i < dt.Count; i++) {
            var row = new HTMLElement(Tag.TR);
            foreach (var prop in Parsers)
                row.AppendChild(prop.Parse(i, dt));
            table.AppendChild(row);
        }
        return table.ToHTML(HTMLRenderer.NoRenderer);
    }
    public override void FillBuilderChilds(StringBuilder sb, IHTMLRenderer renderer) { }
    public override void FillBuilderClosing(StringBuilder sb, IHTMLRenderer renderer) { }
}
*/