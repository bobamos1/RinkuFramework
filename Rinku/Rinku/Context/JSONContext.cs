namespace Rinku.Context; 
public class JSONContext(IContext ctx) : IContext {
    public HTTPCtx Ctx { get; } = ctx.Ctx;
    public PathNavigator Nav { get; init; } = ctx.Nav;
    public Dictionary<string, object?> Items { get; set; } = ctx.Items;
    public RinkuApp Parent { get; } = ctx.Parent;
    public static JSONContext? FromCtx(IContext ctx) => new(ctx);
}