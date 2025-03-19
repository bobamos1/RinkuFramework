namespace Rinku.Tools.Processes;
public interface IStepBaseBase<TDIn, TDOut>;
public interface IStepBaseInput<TDIn, TDOut, in TIn> : IStepBaseBase<TDIn, TDOut> {
    public TDOut GenerateDisplay(TDIn dInput, TIn input);
}
public interface IStepBaseOutput {
    public bool IsInit(bool canThrow);
}
public interface IStepBaseOutput<TDIn, TDOut> : IStepBaseBase<TDIn, TDOut>, IStepBaseOutput {
    public string StepName { get; }
    public string ProcessName { set; }
    public TDOut ProcessAnswer(TDIn dInput);
    public void PopulateProcessSteps(ProcessSteps<TDIn, TDOut> processSteps);
}
public interface IStepBase<TDIn, TDOut, in TIn, out TOut>
    : IStepBaseOutput<TDIn, TDOut>,
    IStepBaseInput<TDIn, TDOut, TIn>;
public interface IEndStep<TDIn, TDOut, in TIn>
    : IStepBaseInput<TDIn, TDOut, TIn>;
public interface IStartStep<TDIn, TDOut> : IStepBaseOutput<TDIn, TDOut> {
    public TDOut GenerateDisplay(TDIn dInput);
}
public interface IStepAdapter<TDIn, TDOut, in TIn> : IStepBaseInput<TDIn, TDOut, TIn>, IStepBaseOutput {
    public IStepBaseBase<TDIn, TDOut> Step { get; }
}
public class ProcessSteps<TDIn, TDOut>(IStartStep<TDIn, TDOut> Start) : Dictionary<string, IStepBaseOutput<TDIn, TDOut>>() {
    public IStartStep<TDIn, TDOut> Start = Start;
    public ProcessSteps<TDIn, TDOut> Add<TIn>(IStepBaseInput<TDIn, TDOut, TIn> stepInput) {
        IStepBaseBase<TDIn, TDOut> stepBase = stepInput;
        if (stepInput is IStepAdapter<TDIn, TDOut, TIn> stepInfo)
            stepBase = stepInfo.Step;
        if (stepBase is not IStepBaseOutput<TDIn, TDOut> step)
            return this;
        if (TryGetValue(step.StepName, out var val)) {
            if (val == step)
                return this;
            throw new Exception($"Cannot have the same step name in the same process {step.StepName}");
        }
        this[step.StepName] = step;
        step.PopulateProcessSteps(this);
        return this;
    }
    public void UpdateProcessName(string ProcessName) {
        foreach (var step in this)
            step.Value.ProcessName = ProcessName;
    }
}