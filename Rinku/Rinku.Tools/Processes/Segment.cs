namespace Rinku.Tools.Processes;
public struct Segment<TFirst, TCurrent> {
    public required TFirst First;
    public TCurrent _current;
    public required TCurrent Current {
        readonly get {
            if (ShouldBeClosed())
                throw new Exception("Segment should be closed since current allready has been used");
            return _current;
        }
        set => _current = value;
    }
    public readonly bool ShouldBeClosed() {
        if (_current is IStepBaseOutput output)
            return output.IsInit(false);
        return true;
    }
    public Segment<TFirst, TNewCurrent> AsSegment<TNewCurrent, TDIn, TDOut, TOut, TNewOut, TF>(
        IBuildableStep<TF, TNewCurrent, TDIn, TDOut, TOut, TNewOut> step) => new() {
            First = First,
            Current = (TNewCurrent)step
        };
    public Segment<TFirst, TNewCurrent> AsAdaptSegment<TNewCurrent, TDIn, TDOut, TNewIn, TNewOut, TF>(
        IBuildableStep<TF, TNewCurrent, TDIn, TDOut, TNewIn, TNewOut> step) => new() {
            First = First,
            Current = (TNewCurrent)step
        };
    public Segment<TFirst, TNewCurrent> ReplaceCurrent<TNewCurrent>(TNewCurrent newCurrent) => new() {
        First = First,
        Current = newCurrent
    };
    public Segment<TFirst, EndSegment> Close() => new() {
        First = First,
        Current = default
    };
}
public struct StepAdapter<TDIn, TDOut, TIn> : IStepAdapter<TDIn, TDOut, TIn> {
    public IStepBaseBase<TDIn, TDOut> Step { get; set; }
    public Func<TDIn, TIn, TDOut> Func;
    public readonly TDOut GenerateDisplay(TDIn dInput, TIn input) => Func(dInput, input);
    public readonly bool IsInit(bool canThrow) => Step.IsInit();
}
public struct StepAdapterStart<TDIn, TDOut, TIn> : IStepAdapter<TDIn, TDOut, TIn> {
    public IStartStep<TDIn, TDOut> StartStep;
    public readonly IStepBaseBase<TDIn, TDOut> Step => StartStep;
    public readonly TDOut GenerateDisplay(TDIn dInput, TIn input) => StartStep.GenerateDisplay(dInput);
    public readonly bool IsInit(bool canThrow) => StartStep.IsInit();
}
public static class SegmentExtensions {
    public static bool IsInit<TDIn, TDOut>(this IStepBaseBase<TDIn, TDOut> step) {
        if (step is IStepBaseOutput output)
            return output.IsInit(false);
        return true;
    }
    public static ProcessSteps<TDIn, TDOut> Build<TDIn, TDOut>(
        this IStartStep<TDIn, TDOut> start) {
        ProcessSteps<TDIn, TDOut> processSteps = new(start);
        start.PopulateProcessSteps(processSteps);
        return processSteps;
    }
    public static ProcessSteps<TDIn, TDOut> Build<TFirst, TDIn, TDOut>(
        this Segment<TFirst, EndSegment> completeSegment)
        where TFirst : IStartStep<TDIn, TDOut>
        => completeSegment.First.Build();
}
public struct EndSegment;
public struct NA;
public interface IBuildableStep<TStep, TSegment, TDIn, TDOut, TIn, TOut>
    : IStepBaseInput<TDIn, TDOut, TIn>;