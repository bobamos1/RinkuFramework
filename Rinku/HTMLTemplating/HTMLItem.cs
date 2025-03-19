using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace HTMLTemplating;
public interface IHTMLRenderer {
    public bool TryFillBuilderWithReplacing(StringBuilder sb, HTMLItem html);
    public bool TryGet(HTMLItem elem, out object? item);
}
public abstract class HTMLItem {
    internal int NbRendererUsing = 0;
    private static readonly char[] SpecialChars = ['&', '<', '>'];
    public static string SanitizeContent(string? content) {
        if (string.IsNullOrEmpty(content))
            return string.Empty;
        if (content.IndexOfAny(SpecialChars) == -1)
            return content;
        var sb = new StringBuilder(content.Length);
        foreach (char c in content)
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
                default:
                    sb.Append(c);
                    break;
            }
        return sb.ToString();
    }
    public HTMLItem? Parent { get; protected set; }
    internal void SetParent(HTMLItem? parent) => Parent = parent;
    protected void AsParentFor(HTMLItem elem) => elem.Parent = this;
    protected List<HTMLItem>? _childs;
    #region rendering
    public string ToHTML(IHTMLRenderer renderer) {
        var sb = new StringBuilder();
        FillBuilderBase(sb, renderer);
        return sb.ToString();
    }
    public void FillBuilderBase(StringBuilder sb, IHTMLRenderer renderer) {
        FillBuilderOpening(sb, renderer);
        if (NbRendererUsing <= 0 || !renderer.TryFillBuilderWithReplacing(sb, this)) { 
            FillBuilderContent(sb);
            FillBuilderChilds(sb, renderer);
        }
        FillBuilderClosing(sb, renderer);
    }
    public abstract void FillBuilderOpening(StringBuilder sb, IHTMLRenderer renderer);
    public virtual void FillBuilderContent(StringBuilder sb) { }
    public virtual void FillBuilderChilds(StringBuilder sb, IHTMLRenderer renderer) {
        if (_childs is null)
            return;
        foreach (var child in _childs)
            child.FillBuilderBase(sb, renderer);
    }
    public abstract void FillBuilderClosing(StringBuilder sb, IHTMLRenderer renderer);
    #endregion
    #region elem manipulation
    public virtual bool Remove() {
        if (Parent is null)
            return true;
        if (Parent._childs is null)
            return false;
        if (Parent._childs.Remove(this))
            return false;
        Parent = null;
        return true;
    }
    public virtual HTMLItem AppendChild(HTMLItem elem) {
        if (elem.Parent is not null)
            throw new Exception();
        _childs ??= [];
        _childs.Add(elem);
        elem.Parent = this;
        return this;
    }
    public virtual HTMLItem PrependChild(HTMLItem elem) {
        if (elem.Parent is not null)
            throw new Exception();
        _childs ??= [];
        _childs.Insert(0, elem);
        elem.Parent = this;
        return this;
    }
    protected bool CheckIndexInParent(out int index) {
        if (Parent is null || Parent._childs is null) {
            index = -1;
            return false;
        }
        index = Parent._childs.IndexOf(this);
        return true;
    }
    public virtual HTMLItem PrependSibling(HTMLItem elem) {
        if (elem.Parent is not null || !CheckIndexInParent(out var index))
            throw new Exception();
        Parent!._childs!.Insert(index, elem);
        elem.Parent = Parent;
        return this;
    }
    public virtual HTMLItem AppendSibling(HTMLItem elem) {
        if (elem.Parent is not null || !CheckIndexInParent(out var index))
            throw new Exception();
        Parent!._childs!.Insert(index + 1, elem);
        elem.Parent = Parent;
        return this;
    }
    #endregion
    #region elem naviagtion
    public virtual IEnumerable<HTMLItem>? Childs() => _childs;
    public virtual IEnumerable<HTMLItem>? Childs(IHTMLRenderer renderer) {
        if (renderer.TryGet(this, out var item) && item is HTMLItem elem)
            return elem.Childs();
        return Childs();
    }
    public virtual HTMLItem? PreviousSibling() {
        if (CheckIndexInParent(out var index))
            return null;
        if (index - 1 == 0)
            return null;
        return Parent!._childs![index - 1];
    }
    public virtual HTMLItem? NextSibling() {
        if (CheckIndexInParent(out var index))
            return null;
        if (index + 1 == Parent!._childs!.Count)
            return null;
        return Parent!._childs![index + 1];
    }
    #endregion
    #region finds
    public T? Find<T>() where T : HTMLItem => Find<T>(_ => true, null);
    public bool Find<T>([MaybeNullWhen(false)] out T elem) where T : HTMLItem {
        elem = Find<T>(_ => true, null);
        return elem is not null;
    }
    public bool Find<T>(Func<T, bool> cond, [MaybeNullWhen(false)] out T elem) where T : HTMLItem {
        elem = Find(cond, null);
        return elem is not null;
    }
    public T? Find<T>(Func<T, bool> cond) where T : HTMLItem => Find(cond, null);
    public virtual T? Find<T>(Func<T, bool> cond, IHTMLRenderer? renderer) where T : HTMLItem {
        if (this is T conv && cond(conv))
            return conv;
        var childs = renderer is null ? Childs() : Childs(renderer);
        if (childs is null)
            return null;
        foreach (var child in childs) {
            var res = child.Find(cond, renderer);
            if (res is not null)
                return res;
        }
        return null;
    }
    #endregion
}
