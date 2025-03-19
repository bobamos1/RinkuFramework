using System.Text;

namespace HTMLTemplating;
internal abstract class FrameForOuterHTMLChange : HTMLItem {
    public override void FillBuilderOpening(StringBuilder sb, IHTMLRenderer renderer) { }
    public override void FillBuilderContent(StringBuilder sb)
        => Parent!.FillBuilderContent(sb);
    public abstract FrameForOuterHTMLChange Add(HTMLItem elemInnerToChange, object? toChangeFor);
    public abstract bool TryUpdateOuter(HTMLItem elemInnerToChange, object? toChangeFor);
    public override void FillBuilderClosing(StringBuilder sb, IHTMLRenderer renderer) { }
    public static void RenderReplacingItem(object? item, StringBuilder sb, IHTMLRenderer renderer) {
        if (item is HTMLItem elem)
            elem.FillBuilderBase(sb, renderer);
        else if (item is IEnumerable<HTMLItem> elems)
            foreach (var elemEnum in elems)
                elemEnum.FillBuilderBase(sb, renderer);
        else
            sb.Append(item);
    }
}
internal class FrameForOuterHTMLChangeSingle : FrameForOuterHTMLChange {
    public HTMLItem ElemToReplace;
    public object? ItemToReplaceFor;
    public FrameForOuterHTMLChangeSingle(HTMLItem elemToReplace, object? itemToReplaceFor) {
        Parent = elemToReplace.Parent;
        ElemToReplace = elemToReplace;
        ItemToReplaceFor = itemToReplaceFor;
    }
    public override void FillBuilderChilds(StringBuilder sb, IHTMLRenderer renderer) {
        foreach (var child in Parent!.Childs()!) {
            if (child != ElemToReplace)
                child.FillBuilderBase(sb, renderer);
            else if (ItemToReplaceFor is HTMLItem elem)
                elem.FillBuilderBase(sb, renderer);
            else
                sb.Append(ItemToReplaceFor);
        }
    }
    public override FrameForOuterHTMLChange Add(HTMLItem elemInnerToChange, object? toChangeFor) {
        if (elemInnerToChange == ElemToReplace) {
            ItemToReplaceFor = toChangeFor;
            return this;
        }
        return new FrameForOuterHTMLChangeMultiple(new() {
            [ElemToReplace] = ItemToReplaceFor,
            [elemInnerToChange] = toChangeFor
        });
    }
    public override bool TryUpdateOuter(HTMLItem elemInnerToChange, object? toChangeFor) {
        if (elemInnerToChange != ElemToReplace)
            return false;
        ItemToReplaceFor = toChangeFor;
        return true;
    }
}
internal class FrameForOuterHTMLChangeMultiple : FrameForOuterHTMLChange {
    public Dictionary<HTMLItem, object?> ElemsToReplace;
    public FrameForOuterHTMLChangeMultiple(Dictionary<HTMLItem, object?> ElemsToReplace) {
        Parent = ElemsToReplace.First().Key.Parent;
        this.ElemsToReplace = ElemsToReplace;
    }
    public override void FillBuilderChilds(StringBuilder sb, IHTMLRenderer renderer) {
        foreach (var child in Parent!.Childs()!) {
            if (!ElemsToReplace.TryGetValue(child, out var item))
                child.FillBuilderBase(sb, renderer);
            else if (item is HTMLItem elem)
                elem.FillBuilderBase(sb, renderer);
            else
                sb.Append(item);
        }
    }
    public override FrameForOuterHTMLChange Add(HTMLItem elemInnerToChange, object? toChangeFor) {
        ElemsToReplace[elemInnerToChange] = toChangeFor;
        return this;
    }

    public override bool TryUpdateOuter(HTMLItem elemInnerToChange, object? toChangeFor) 
        => ElemsToReplace.TryUpdate(elemInnerToChange, toChangeFor);
}