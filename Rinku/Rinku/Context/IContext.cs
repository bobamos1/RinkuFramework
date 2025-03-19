namespace Rinku.Context;
public interface IContext {
    public HTTPCtx Ctx { get; }
    public PathNavigator Nav { get; }
    public Dictionary<string, object?> Items { get; }
    public RinkuApp Parent { get; }
}