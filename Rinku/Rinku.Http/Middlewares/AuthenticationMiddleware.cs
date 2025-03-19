using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Rinku.Middlewares;

namespace Rinku.Http.Middlewares;
public class AuthenticationMiddleware<T> 
    : AuthenticationMiddlewareBase<T>, INeedToUseWebApp where T : ICredentialClaim<T, ClaimsIdentity> {
    public required string AuthScheme;
    public required string LoginPath;
    public required string LogoutPath;
    public TimeSpan ExpireTimeSpan = TimeSpan.FromMinutes(15);
    public bool SlidingExpiration = true;
    public bool HttpOnly = true;
    public bool SecurePolicy = true;
    public void AddWebBuilder(WebApplicationBuilder builder) {
        builder.Services.AddAuthentication()
                        .AddCookie(AuthScheme, options => {
                            options.LoginPath = '/' + LoginPath;
                            options.LogoutPath = '/' + LogoutPath;
                            options.ExpireTimeSpan = ExpireTimeSpan;
                            options.SlidingExpiration = SlidingExpiration;
                            options.Cookie.HttpOnly = HttpOnly;
                            options.Cookie.SecurePolicy = SecurePolicy
                                                            ? CookieSecurePolicy.Always
                                                            : CookieSecurePolicy.None;
                        });
        builder.Services.AddAuthorization();
    }
    public void UseWebApp(WebApplication app) {
        app.MapGet('/' + LogoutPath, SignOutAndRedirect);
        app.MapPost('/' + LogoutPath, SignOut);
        app.UseAuthentication();
        app.UseAuthorization();
    }
    public Task SignOut(HTTPCtx ctx) => ctx.SignOutAsync(AuthScheme);
    public async Task SignOutAndRedirect(HTTPCtx ctx) {
        await SignOut(ctx);
        ctx.Response.Redirect(LoginPath);
    }
    public Task SignIn(HTTPCtx ctx, T cred, bool isPersistent = true)
        => SignIn(ctx, new ClaimsIdentity(cred.MakeClaim(AuthScheme)), new() { IsPersistent = isPersistent });
    public Task SignIn(HTTPCtx ctx, ClaimsIdentity cred, AuthenticationProperties options) 
        => ctx.SignInAsync(AuthScheme, new(cred), options);
}