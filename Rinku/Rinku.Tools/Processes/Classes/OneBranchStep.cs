namespace Rinku.Tools.Processes;
public static class OneBranchBuilder {
    public static Segment<TFirst, TNewSeg> Then<TFirst, TNewSeg, TDI, TDO, TO, TNewOut, TTStep, TTI, TTF1, TTI1>(
        this Segment<TFirst, OneBranchStep<TTStep, TDI, TDO, TTI, TO>> segment,
         IBuildableStep<TTF1, TNewSeg, TDI, TDO, TTI1, TNewOut> nextStep)
        where TTF1 : IStepBaseInput<TDI, TDO, TO> {
        segment.Current.NextStep = (TTF1)nextStep;
        return segment.AsSegment(nextStep);
    }
    public static Segment<TFirst, TNewSeg> ThenAdapt<TFirst, TNewSeg, TDI, TDO, TO, TNewOut, TTStep, TTI, TTF1, TTI1>(
        this Segment<TFirst, OneBranchStep<TTStep, TDI, TDO, TTI, TO>> segment,
         IBuildableStep<TTF1, TNewSeg, TDI, TDO, TTI1, TNewOut> nextStep)
        where TTF1 : IStartStep<TDI, TDO> {
        segment.Current.NextStep = StepInfo<TO>.From(nextStep);
        return segment.AsAdaptSegment(nextStep);
    }
    public static Segment<TFirst, TNewSeg> ThenAdapt<TFirst, TNewSeg, TDI, TDO, TO, TNewOut, TTStep, TTI, TTF1, TTI1>(
        this Segment<TFirst, OneBranchStep<TTStep, TDI, TDO, TTI, TO>> segment,
         IBuildableStep<TTF1, TNewSeg, TDI, TDO, TTI1, TNewOut> nextStep,
         Func<TDI, TO, TTI1> adapter) {
        segment.Current.NextStep = StepInfo<TO>.From(nextStep, adapter);
        return segment.AsAdaptSegment(nextStep);
    }
    public static Segment<TFirst, TNewSeg> Then<TFirst, TNewSeg, TDI, TDO, TO, TNewOut, TTStep, TTI, TTF1>(
        this Segment<TFirst, OneBranchStep<TTStep, TDI, TDO, TTI, TO>> segment,
         Segment<TTF1, TNewSeg> nextSegment)
        where TTF1 : IStepBaseInput<TDI, TDO, TO> {
        segment.Current.NextStep = nextSegment.First;
        return segment.ReplaceCurrent(nextSegment.Current);
    }
}
public abstract class OneBranchStep<TStep, TDIn, TDOut, TIn, TOut>(string? StepName)
    : BuildableStep<TStep, OneBranchStep<TStep, TDIn, TDOut, TIn, TOut>, TDIn, TDOut, TIn, TOut>(StepName) {
    public IStepBaseInput<TDIn, TDOut, TOut> NextStep = null!;
    public override bool IsInit(bool canThrow) {
        if (NextStep is null)
            return canThrow ? throw new ArgumentNullException($"Missing nextstep for {StepName}, {this}") : false;
        return true;
    }
    public override void PopulateProcessSteps(ProcessSteps<TDIn, TDOut> processSteps) {
        IsInit(true);
        processSteps.Add(NextStep);
    }
}