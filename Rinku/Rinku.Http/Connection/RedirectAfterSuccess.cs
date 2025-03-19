using HTMLTemplating;
using Rinku.Context;
using Rinku.Http.ConnectionClasses;
using Rinku.Http.Middlewares;

namespace Rinku.Http.Connection;
public class RedirectAfterSuccess(string RedirectPath, string Target, string? StepName = null) : HTMXEndStep<RedirectAfterSuccess, int>(StepName) {
    public string RedirectPath = RedirectPath;
    public string Target = Target;

    public override Task<HTMLItem> GenerateDisplay(IContext ctx, int id) {
        //do login
        ctx.Parent.GetMiddleware<AuthenticationMiddleware<User>>()
            .SignIn(ctx.Ctx, new User(1, [1]), true);
        return Task.FromResult<HTMLItem>((HTMLRaw)
            $@"<div hx-get=""{RedirectPath}"" hx-trigger=""load"" hx-target=""{Target}"" hx-push-url=""true""></div>");
    }
}
