using Rinku.Context;

namespace Rinku.Controllers;
public interface IControllerr {
    public Task<bool> TryHandleEndpoint(IContext ctx);
    public bool SupportsContextType(Type ctxType);
}
public interface IEndpointHandler<in T> where T : IContext {
    public Task<bool> TryHandleEndpoint(T ctx);
}
/*
public abstract class Controller<T> : IController {
    public bool SupportsContextType(Type ctxType) => typeof(T).IsAssignableFrom(ctxType);
    public virtual Task<bool> TryHandleEndpoint(IContext ctx) => ctx is T c 
        ? HandleEndpoint(c) 
        : Task.FromResult(false);
    public abstract Task<bool> HandleEndpoint(T ctx);
}
public abstract class Controller<T1, T2> : IController {
    public bool SupportsContextType(Type ctxType) 
        => typeof(T1).IsAssignableFrom(ctxType) || typeof(T2).IsAssignableFrom(ctxType);
    public virtual Task<bool> TryHandleEndpoint(IContext ctx) => ctx switch {
        T1 c1 => HandleEndpoint(c1),
        T2 c2 => HandleEndpoint(c2),
        _ => Task.FromResult(false)
    };
    public abstract Task<bool> HandleEndpoint(T1 ctx);
    public abstract Task<bool> HandleEndpoint(T2 ctx);
}
public static class ControllerExtension {
    public static bool TryAdd<T>(this List<IController> controllers, IController controller) {
        if (!controller.SupportsContextType(typeof(T)))
            return false;
        controllers.Add(controller);
        return true;
    }
    public static async Task<bool> TryHandle(this IEnumerable<IController> controllers, IContext ctx) {
        foreach (var controller in controllers)
            if (await controller.TryHandleEndpoint(ctx))
                return true;
        return false;
    }
}
*/