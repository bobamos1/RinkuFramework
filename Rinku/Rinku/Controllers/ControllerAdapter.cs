using Rinku.Context;

namespace Rinku.Controllers;
public class ControllerAdapter<T, TCtx>(IEndpointHandler<TCtx> Controller) : IEndpointHandler<T> 
    where TCtx : T where T : IContext {
    public IEndpointHandler<TCtx> Controller = Controller;
    public Task<bool> TryHandleEndpoint(T ctx) {
        if (ctx is not TCtx ctxTc)
            return Task.FromResult(false);
        return Controller.TryHandleEndpoint(ctxTc);
    }
}
public static class AdapterHelper {
    public static void AddAdapt<T, TCtx>(this List<IEndpointHandler<T>> list, IEndpointHandler<TCtx> controller)
        where T : IContext
        where TCtx : T {
        list.Add(new ControllerAdapter<T, TCtx>(controller));
    }
    public static async Task<bool> TryHandle<T>(this List<IEndpointHandler<T>> list, T ctx)
        where T : IContext {
        foreach (var controller in list)
            if (await controller.TryHandleEndpoint(ctx))
                return true;
        return false;
    }
}