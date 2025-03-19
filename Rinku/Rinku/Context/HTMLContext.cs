using HTMLTemplating;

namespace Rinku.Context;
public class HTMLCredContext<T>(IContext ctx) : HTMLCredContext(ctx) where T : class {
    public new T? Credential => base.Credential as T;
}
public class HTMLCredContext(IContext ctx) : HTMLContext(ctx), ICredentialContext {
    public object? Credential { get; set; } = ctx.GetCredential();
}
public class HTMLContext(IContext ctx) : HTMLRenderer, IContext {
    public HTTPCtx Ctx { get; set; } = ctx.Ctx;
    public PathNavigator Nav { get; set; } = ctx.Nav;
    public Dictionary<string, object?> Items { get; set; } = ctx.Items;
    public RinkuApp Parent { get; } = ctx.Parent;

    public Dictionary<string, HTMLItem> CachedElems = ctx is HTMLContext c ? c.CachedElems : [];
    public HTMLContext Cache(string key, HTMLItem elem) {
        CachedElems.Add(key, elem);
        return this;
    }
    public void SetRoot(string parentKey, HTMLItem elem) {
        if (TryChangeInner(parentKey, elem))
            return;
        RootElement = elem;
    }
    public bool TryChangeInner(string key, object? valueToChangeFor) {
        if (!CachedElems.TryGetValue(key, out var elem))
            return false;
        ChangeInner(elem, valueToChangeFor);
        return true;
    }
    public bool TryChangeOuter(string key, object? valueToChangeFor) {
        if (!CachedElems.TryGetValue(key, out var elem))
            return false;
        ChangeOuter(elem, valueToChangeFor);
        return true;
    }
    public async Task<bool> SetHTML() {
        if (RootElement is null)
            return false;
        await Ctx.WriteResponse("text/html", Render());
        return true;
    }
}