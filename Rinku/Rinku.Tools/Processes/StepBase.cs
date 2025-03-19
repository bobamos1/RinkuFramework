using System.Runtime.CompilerServices;

namespace Rinku.Tools.Processes;
public abstract class EndStepBase<TDIn, TDOut, TIn>(string? stepName)
    : IEndStep<TDIn, TDOut, TIn> {
    public string StepName => (stepName ?? "ProcessStep" + GetHashCode().ToString()).ToLower();
    public abstract TDOut GenerateDisplay(TDIn dInput, TIn input);
}
public abstract class BuildableEndStep<TStep, TDIn, TDOut, TIn>(string? StepName)
    : EndStepBase<TDIn, TDOut, TIn>(StepName),
    IBuildableStep<TStep, EndSegment, TDIn, TDOut, TIn, EndSegment>
    where TStep : BuildableEndStep<TStep, TDIn, TDOut, TIn> {
    public static implicit operator Segment<TStep, EndSegment>(
        BuildableEndStep<TStep, TDIn, TDOut, TIn> endStep) => new() {
            First = (TStep)endStep,
            Current = default
        };
}
public abstract class StepBase<TDIn, TDOut, TIn, TOut>
    : IStepBase<TDIn, TDOut, TIn, TOut> {
    public string StepName { get; init; }
    public string Endpoint = null!;
    protected string _processName = null!;
    public string ProcessName { set {
            _processName = value;
            Endpoint = $"{_processName}/{StepName}";
        }
    }
    public StepBase(string? StepName) {
        this.StepName = (StepName ?? "ProcessStep" + GetHashCode().ToString()).ToLower();
    }
    public abstract TDOut ProcessAnswer(TDIn dInput);
    public abstract bool IsInit(bool canThrow);
    public abstract void PopulateProcessSteps(ProcessSteps<TDIn, TDOut> processSteps);
    public abstract TDOut GenerateDisplay(TDIn dInput, TIn input);
}
public abstract class BuildableStep<TStep, TSegment, TDIn, TDOut, TIn, TOut>(string? StepName)
    : StepBase<TDIn, TDOut, TIn, TOut>(StepName), IBuildableStep<TStep, TSegment, TDIn, TDOut, TIn, TOut>
    where TSegment : BuildableStep<TStep, TSegment, TDIn, TDOut, TIn, TOut>;