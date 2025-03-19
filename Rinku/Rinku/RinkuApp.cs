using Rinku.Context;
using Rinku.Controllers;

namespace Rinku;
public class RinkuApp(IEndpointHandler<NoContext> RootController, IEndpointHandler<NoContext>[] Middlewares)
    : IEndpointHandler<NoContext> {
    public T GetMiddleware<T>() {
        foreach (var middleware in Middlewares)
            if (middleware is T m)
                return m;
        throw new Exception($"Middleware {nameof(T)} does not exist");
    }
    public IEndpointHandler<NoContext>[] Middlewares = Middlewares;
    public IEndpointHandler<NoContext> RootController = RootController;
    public async Task<bool> TryHandleEndpoint(NoContext ctx) {
        try {
            foreach (var middleware in Middlewares)
                if (!await middleware.TryHandleEndpoint(ctx))
                    return false;
            return await RootController.TryHandleEndpoint(ctx);
        }
        catch (Exception) {
            return false;
        }
        finally {
            if (ctx.InnerContext is IDisposable disp)
                disp.Dispose();
        }
    }
}