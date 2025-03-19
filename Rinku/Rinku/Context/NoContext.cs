namespace Rinku.Context;

public class NoContext(HTTPCtx Ctx, string Path, RinkuApp Parent) : IContext {
    public IContext? InnerContext;
    public HTTPCtx Ctx { get; } = Ctx;
    public PathNavigator Nav { get; init; } = new PathNavigator(Path);
    public Dictionary<string, object?> Items { get; set; } = [];
    public RinkuApp Parent { get; } = Parent;
}
