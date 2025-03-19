using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace HTMLTemplating;
internal class NoRenderer : IHTMLRenderer {
    public bool TryFillBuilderWithReplacing(StringBuilder sb, HTMLItem html) => false;
    public bool TryGet(HTMLItem elem, [MaybeNullWhen(false)] out object item) {
        item = null;
        return false;
    }
}
public class HTMLRenderer : IHTMLRenderer {
    public static readonly IHTMLRenderer NoRenderer = new NoRenderer();
    public HTMLItem? RootElement;
    private readonly Dictionary<HTMLItem, object?> Queue = [];
    public HTMLRenderer() { }
    public HTMLRenderer(HTMLItem RootElement) {
        this.RootElement = RootElement;
    }
    #region outer
    public void ChangeOuter(HTMLItem elemOuterToChange, object? toChangeFor) {
        var res = TryGetOuterFrame(elemOuterToChange, out var frame)
            ? frame.Add(elemOuterToChange, toChangeFor)
            : new FrameForOuterHTMLChangeSingle(elemOuterToChange, toChangeFor);
        if (res != frame)
            Queue[elemOuterToChange.Parent!] = res;
    }
    public bool TryUpdateOuter(HTMLItem elemOuterToChange, object toChangeFor)
        => TryGetOuterFrame(elemOuterToChange, out var frame) 
        && frame.TryUpdateOuter(elemOuterToChange, toChangeFor);
    internal bool TryGetOuterFrame(HTMLItem elemOuterToChange, [MaybeNullWhen(false)] out FrameForOuterHTMLChange frame) {
        frame = null;
        var parent = elemOuterToChange.Parent ?? throw new Exception();
        if (!Queue.TryGetValue(parent, out var toChangeForFound))
            return false;
        if (toChangeForFound is not FrameForOuterHTMLChange)
            return false;
        frame = (FrameForOuterHTMLChange)toChangeForFound;
        return true;
    }
    #endregion
    #region inner
    public void ChangeInner(HTMLItem elemInnerToChange, object? toChangeFor)
        => Queue[elemInnerToChange] = toChangeFor;
    public bool TryUpdateInner(HTMLItem elemInnerToChange, object? toChangeFor)
        => Queue.TryUpdate(elemInnerToChange, toChangeFor);
    #endregion
    #region implementation
    public bool TryFillBuilderWithReplacing(StringBuilder sb, HTMLItem html) {
        if (!Queue.TryGetValue(html, out var item))
            return false;
        if (item is HTMLItem elem)
            elem.FillBuilderBase(sb, this);
        else if (item is IEnumerable<HTMLItem> elems)
            foreach (var elemEnum in elems)
                elemEnum.FillBuilderBase(sb, this);
        else
            sb.Append(item);
        return true;
    }
    public bool TryGet(HTMLItem elem, out object? item) 
        => Queue.TryGetValue(elem, out item);
    public string Render() {
        foreach (var elem in Queue.Keys)
            Interlocked.Increment(ref elem.NbRendererUsing);
        try {
            return RootElement?.ToHTML(this) ?? "";
        }
        finally {
            foreach (var elem in Queue.Keys)
                Interlocked.Decrement(ref elem.NbRendererUsing);
        }
    }
    #endregion
    #region find
    public T? Find<T>() where T : HTMLItem => RootElement?.Find<T>(_ => true, this);
    public bool TryFind<T>([MaybeNullWhen(false)] out T elem) where T : HTMLItem {
        elem = RootElement?.Find<T>(_ => true, this);
        return elem is not null;
    }
    public bool TryFind<T>(Func<T, bool> cond, [MaybeNullWhen(false)] out T elem) where T : HTMLItem {
        elem = RootElement?.Find(cond, this);
        return elem is not null;
    }
    public T? Find<T>(Func<T, bool> cond) where T : HTMLItem => RootElement?.Find(cond, this);
    #endregion
}