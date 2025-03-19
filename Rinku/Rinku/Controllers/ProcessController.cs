using HTMLTemplating;
using Rinku.Context;
using Rinku.Tools.Processes;

namespace Rinku.Controllers;
public class ProcessController : IEndpointHandler<HTMLContext>, IEndpointHandler<JSONContext> {
    public string ParentKey;
    public string Name;
    public ProcessSteps<IContext, Task<HTMLItem>> Process;
    public ProcessController(string Name, string ParentKey, ProcessSteps<IContext, Task<HTMLItem>> Process) {
        this.Name = Name;
        this.ParentKey = ParentKey;
        this.Process = Process;
        this.Process.UpdateProcessName(this.Name);
    }
    public virtual Task<bool> ConditionBefore(HTMLContext ctx) => Task.FromResult(ctx.Nav.SameSegmentAs(Name));
    public virtual Task<bool> HandleAfterMainProcess(HTMLContext ctx) {
        if (!ctx.TryChangeInner("title", Name))
            _ = 1;//would use htmx/js to update title
        return Task.FromResult(true);
    }
    public async Task<bool> TryHandleEndpoint(HTMLContext ctx) {
        if (!await ConditionBefore(ctx))
            return false;
        if (!await HandleProcess(ctx))
            return false;
        if (!await HandleAfterMainProcess(ctx))
            return false;
        return await ctx.SetHTML();
    }
    public async Task<bool> HandleProcess(HTMLContext ctx) {
        if (ctx.Ctx.IsPost()) {
            ctx.Nav.NextSegment();
            return await HandleProcessStep(ctx);
        }
        var res = await Process.Start.GenerateDisplay(ctx);
        ctx.SetRoot(ParentKey, res);
        return true;
    }
    public virtual async Task<bool> HandleProcessStep(HTMLContext ctx) {
        if (ctx.Nav.CurrentSegment is null)
            return false;
        else if (Process.TryGetValue(ctx.Nav.CurrentSegment, out var step))
            ctx.RootElement = await step.ProcessAnswer(ctx);
        return true;
    }

    public Task<bool> TryHandleEndpoint(JSONContext ctx) {
        return Task.FromResult(false);
    }
}
