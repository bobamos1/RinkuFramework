global using HTTPCtx = Microsoft.AspNetCore.Http.HttpContext;
using Microsoft.AspNetCore.Builder;
using Rinku.Context;
using Rinku.Http.Middlewares;

namespace Rinku.Http;
public static class PipelineExtensions {
    public static void AddRinkuApp(this WebApplicationBuilder builder, RinkuApp rinkuApp) {
        foreach (var middleware in rinkuApp.Middlewares)
            if (middleware is INeedToUseWebApp need)
                need.AddWebBuilder(builder);
    }
    public static void UseRinkuApp(this WebApplication app, RinkuApp rinkuApp, HashSet<string> excludedPaths) {
        foreach (var middleware in rinkuApp.Middlewares)
            if (middleware is INeedToUseWebApp need)
                need.UseWebApp(app);
        app.Use(async (ctx, next) => {
            if (excludedPaths.Contains(ctx.Request.Path)
             || !await rinkuApp.TryHandleEndpoint(new NoContext(ctx, ctx.Request.Path, rinkuApp)))
                await next();
        });
    }
}
