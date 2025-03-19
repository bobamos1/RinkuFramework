namespace Rinku.Tools.Processes;

public static class TwoBranchBuilder {
    public static Segment<TFirst, EndSegment> Then<TFirst, TDI, TDO, TO, TTStep, TTI, TTF1, TTF2>(
        this Segment<TFirst, TwoBranchStep<TTStep, TDI, TDO, TTI, TO>> segment,
         Segment<TTF1, EndSegment> nextSegment1,
         Segment<TTF2, EndSegment> nextSegment2)
        where TTF1 : IStepBaseInput<TDI, TDO, TO>
        where TTF2 : IStepBaseInput<TDI, TDO, TO> {
        segment.Current.NextStep1 = nextSegment1.First;
        segment.Current.NextStep2 = nextSegment2.First;
        return segment.Close();
    }
}
public abstract class TwoBranchStep<TStep, TDIn, TDOut, TIn, TOut>(string? StepName)
    : BuildableStep<TStep, TwoBranchStep<TStep, TDIn, TDOut, TIn, TOut>, TDIn, TDOut, TIn, TOut>(StepName) {
    public IStepBaseInput<TDIn, TDOut, TOut> NextStep1 = null!;
    public IStepBaseInput<TDIn, TDOut, TOut> NextStep2 = null!;
    public override bool IsInit(bool canThrow) {
        if (NextStep1 is null)
            return canThrow ? throw new ArgumentNullException($"Missing nextstep for {StepName} branch 1, {this}") : false;
        if (NextStep2 is null)
            return canThrow ? throw new ArgumentNullException($"Missing nextstep for {StepName} branch 2, {this}") : false;
        return true;
    }
    public override void PopulateProcessSteps(ProcessSteps<TDIn, TDOut> processSteps) {
        IsInit(true);
        processSteps.Add(NextStep1);
        processSteps.Add(NextStep2);
    }
}