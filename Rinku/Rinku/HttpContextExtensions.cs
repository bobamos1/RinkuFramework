global using HTTPCtx = Microsoft.AspNetCore.Http.HttpContext;
global using CookieOptions = Microsoft.AspNetCore.Http.CookieOptions;
global using ClaimsIdentity = System.Security.Claims.ClaimsIdentity;
using Microsoft.AspNetCore.Http;

namespace Rinku; 
public static class HttpContextExtensions {
    public static Task WriteResponse(this HTTPCtx ctx, string contentType, string content) {
        ctx.Response.ContentType = contentType;
        return ctx.Response.WriteAsync(content);
    }
    public static bool IsGet(this HTTPCtx ctx) => ctx.Request.Method == "GET";
    public static bool IsPost(this HTTPCtx ctx) => ctx.Request.Method == "POST";
    public static bool NeedCompleteRender(this HTTPCtx ctx)
        => !ctx.Request.Headers["HX-Request"].Equals("true");
    public static string? GetCookie(this HTTPCtx ctx, string cookieName) 
        => ctx.Request.Cookies[cookieName];
    public static string? GetHeader(this HTTPCtx ctx, string paramName) 
        => ctx.Request.Headers[paramName];
    public static string? GetQueryParam(this HTTPCtx ctx, string paramName) 
        => ctx.Request.Query[paramName];
    public static void SetCulture(this HTTPCtx ctx, string name, string value, CookieOptions options)
        => ctx.Response.Cookies.Append(name, value, options);
}
