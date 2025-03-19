using HTMLTemplating;
using Rinku.Context;
using Rinku.Http.Connection;
using Rinku.Tools.Processes;

namespace Rinku.Http.Connection;
public interface IHTMXFrom<TIn> : IStepBaseInput<IContext, Task<HTMLItem>, TIn>;
public abstract class HTMXStep<TStep, TIn, TOut, TExtracted>(string? StepName)
    : OneBranchStep<TStep, IContext, Task<HTMLItem>, TIn, TOut>(StepName)
    where TStep : HTMXStep<TStep, TIn, TOut, TExtracted> {
    public abstract TExtracted ExtractAnswer(IContext ctx);
    public abstract Task<HTMLItem> ProcessAnswer(IContext displayInput, TExtracted extracted);
    public override Task<HTMLItem> ProcessAnswer(IContext ctx)
        => ProcessAnswer(ctx, ExtractAnswer(ctx));
}
public abstract class HTMXStep2<TStep, TIn, TOut, TExtracted>(string? StepName)
    : TwoBranchStep<TStep, IContext, Task<HTMLItem>, TIn, TOut>(StepName)
    where TStep : HTMXStep2<TStep, TIn, TOut, TExtracted> {
    public abstract TExtracted ExtractAnswer(IContext ctx);
    public abstract Task<HTMLItem> ProcessAnswer(IContext displayInput, TExtracted extracted);
    public override Task<HTMLItem> ProcessAnswer(IContext ctx)
        => ProcessAnswer(ctx, ExtractAnswer(ctx));
}
public interface IHTMXStartStep : IStartStep<IContext, Task<HTMLItem>>;
public abstract class HTMXEndStep<TStep, TIn>(string? StepName)
    : BuildableEndStep<TStep, IContext, Task<HTMLItem>, TIn>(StepName)
    where TStep : HTMXEndStep<TStep, TIn>;
public static class HTMXShortcutHelper {
    public static ProcessSteps<IContext, Task<HTMLItem>> Build<TFirst>(
        this Segment<TFirst, EndSegment> completeSegment)
        where TFirst : IStartStep<IContext, Task<HTMLItem>>
        => completeSegment.First.Build();
}