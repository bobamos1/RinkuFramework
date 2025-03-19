namespace Rinku.Tools.Processes;
public static class Seg {
    public static Segment<TStep, EndSegment> CloseWith<TStep, TSeg, TDI, TDO, TI, TO>(
        IBuildableStep<TStep, TSeg, TDI, TDO, TI, TO> buildableStep) => new() {
            First = (TStep)buildableStep,
            Current = default
        };
    public static Segment<TStep, TSeg> From<TStep, TSeg, TDI, TDO, TI, TO>(IBuildableStep<TStep, TSeg, TDI, TDO, TI, TO> buildableStep) => new() {
        First = (TStep)buildableStep,
        Current = typeof(TSeg) == typeof(EndSegment) ? default! : (TSeg)buildableStep
    };
}
public static class Seg<TInNeed> {
    public static Segment<IStepAdapter<TDI, TDO, TInNeed>, EndSegment> CloseWith<TStep, TSeg, TDI, TDO, TI, TO>(
        IBuildableStep<TStep, TSeg, TDI, TDO, TI, TO> buildableStep)
        where TStep : IStartStep<TDI, TDO> => new() {
            First = StepInfo<TInNeed>.From(buildableStep),
            Current = default
        };
    public static Segment<IStepAdapter<TDI, TDO, TInNeed>, EndSegment> CloseWith<TStep, TSeg, TDI, TDO, TI, TO>(
        IBuildableStep<TStep, TSeg, TDI, TDO, TI, TO> buildableStep, Func<TDI, TInNeed, TI> adapter) => new() {
            First = StepInfo<TInNeed>.From(buildableStep, adapter),
            Current = default
        };
    public static Segment<IStepAdapter<TDI, TDO, TInNeed>, TSeg> From<TStep, TSeg, TDI, TDO, TI, TO>(
        IBuildableStep<TStep, TSeg, TDI, TDO, TI, TO> buildableStep)
        where TStep : IStartStep<TDI, TDO> => new() {
            First = StepInfo<TInNeed>.From(buildableStep),
            Current = (TSeg)buildableStep
        };
    public static Segment<IStepAdapter<TDI, TDO, TInNeed>, TSeg> From<TStep, TSeg, TDI, TDO, TI, TO>(
        IBuildableStep<TStep, TSeg, TDI, TDO, TI, TO> buildableStep, Func<TDI, TInNeed, TI> adapter) => new() {
            First = StepInfo<TInNeed>.From(buildableStep, adapter),
            Current = (TSeg)buildableStep
        };
}
public static class StepInfo<TInNeed> {
    public static StepAdapterStart<TDIn, TDOut, TInNeed> From<TStep, TSegment, TDIn, TDOut, TIn, TOut>(
        IBuildableStep<TStep, TSegment, TDIn, TDOut, TIn, TOut> buildableStep)
        where TStep : IStartStep<TDIn, TDOut>
            => new() { StartStep = (TStep)buildableStep };
    public static StepAdapter<TDIn, TDOut, TInNeed> From<TStep, TSegment, TDIn, TDOut, TIn, TOut>(
        IBuildableStep<TStep, TSegment, TDIn, TDOut, TIn, TOut> buildableStep,
        Func<TDIn, TInNeed, TIn> adapter)
            => new() { Step = buildableStep, Func = (i, p) => buildableStep.GenerateDisplay(i, adapter(i, p)) };
}