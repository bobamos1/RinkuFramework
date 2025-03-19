using Rinku.Context;
using Rinku.Controllers;

namespace Rinku.Middlewares;
public abstract class AuthenticationMiddlewareBase<T> : IEndpointHandler<NoContext> where T : ICredentialClaim<T, ClaimsIdentity> {
    public Task<bool> TryHandleEndpoint(NoContext ctx) {
        if (ctx.Ctx.User.Identity is not ClaimsIdentity claims)
            return Task.FromResult(true);
        var cred = T.FromClaim(claims);
        if (cred is null)
            return Task.FromResult(true);
        ctx.SetCredential(cred);
        return Task.FromResult(true);
    }
}