using Rinku.Context;
using Rinku.Tools.HTML;

namespace Rinku.Controllers;
public class RootControllerCred<T>(string AppName, string JSONEndpointName = "api")
    : RootController<HTMLCredContext<T>, JSONContext>(AppName, JSONEndpointName),
    IEndpointHandler<HTMLCredContext<T>>, IEndpointHandler<JSONContext> where T : class {
    public override HTMLCredContext<T> MakeHTMLCtx(IContext ctx) => new(ctx);
    public override JSONContext MakeJSONCtx(IContext ctx) => new(ctx);
}
public class RootControllerCred(string AppName, string JSONEndpointName = "api")
    : RootController<HTMLCredContext, JSONContext>(AppName, JSONEndpointName),
    IEndpointHandler<HTMLCredContext>, IEndpointHandler<JSONContext> {
    public override HTMLCredContext MakeHTMLCtx(IContext ctx) => new(ctx);
    public override JSONContext MakeJSONCtx(IContext ctx) => new(ctx);
}
public class RootController(string AppName, string JSONEndpointName = "api")
    : RootController<HTMLContext, JSONContext>(AppName, JSONEndpointName),
    IEndpointHandler<HTMLContext>, IEndpointHandler<JSONContext> {
    public override HTMLContext MakeHTMLCtx(IContext ctx) => new(ctx);
    public override JSONContext MakeJSONCtx(IContext ctx) => new(ctx);
}
public abstract class RootController<THTML, TJSON> : IEndpointHandler<NoContext>
    where THTML : HTMLContext
    where TJSON : JSONContext {
    public HTMLRoot Root;
    public List<IEndpointHandler<THTML>> HTMLControllers = [];
    public List<IEndpointHandler<TJSON>> JSONControllers = [];
    public HTMLTitle Title;
    public string JSONEndpointName;
    public HTMLHead Head => Root.Head;
    public RootController(string AppName, string JSONEndpointName = "api") {
        this.JSONEndpointName = JSONEndpointName;
        Title = new HTMLTitle(AppName);
        Root = new HTMLRoot();
        Root.Head.AppendChild(Title);
    }
    public abstract TJSON MakeJSONCtx(IContext ctx);
    public abstract THTML MakeHTMLCtx(IContext ctx);
    public virtual Task<bool> TryHandleEndpoint(NoContext ctx) {
        if (ctx.InnerContext is TJSON json)
            return TryHandleEndpoint(json);
        if (ctx.InnerContext is THTML html)
            return TryHandleEndpoint(html);
        if (ctx.Nav.SameSegmentAs(JSONEndpointName)) {
            ctx.Nav.NextSegment();
            return TryHandleEndpoint(MakeJSONCtx(ctx));
        }
        return TryHandleEndpoint(MakeHTMLCtx(ctx));
    }
    public virtual Task<bool> TryHandleEndpoint(TJSON ctx) {
        if (ctx.Nav.CurrentSegment is null)
            return Task.FromResult(false);
        return JSONControllers.TryHandle(ctx);
    }
    public virtual Task<bool> TryHandleEndpoint(THTML ctx) {
        if (ctx.Nav.CurrentSegment is null) {
            ctx.RootElement = Root;
            return ctx.SetHTML();
        }
        if (ctx.Ctx.NeedCompleteRender()) {
            ctx.RootElement = Root;
            ctx.Cache("title", Title)
                .Cache("body", Root);
        }
        return HTMLControllers.TryHandle(ctx);
    }
    public virtual RootController<THTML, TJSON> AddController<TC>(TC controller)
        where TC : IEndpointHandler<THTML>, IEndpointHandler<TJSON> {
        HTMLControllers.Add(controller);
        JSONControllers.Add(controller);
        return this;
    }
}
