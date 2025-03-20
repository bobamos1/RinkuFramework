namespace Processes;

public interface IStartStep<TDIn, TDOut> {
    public TDOut GenerateDisplay(TDIn dInput);
}
public class ProcessSteps<TDIn, TDOut>(IStartStep<TDIn, TDOut> Start) : Dictionary<string, IStepBaseOutput<TDIn, TDOut>>() {
    public IStartStep<TDIn, TDOut> Start = Start;
    public ProcessSteps<TDIn, TDOut> Add<TIn>(IStepBaseInput<TDIn, TDOut, TIn> stepInput) {
        //IStepBaseBase<TDIn, TDOut> stepBase = stepInput;
        //if (stepInput is IStepAdapter<TDIn, TDOut, TIn> stepInfo)
            //stepBase = stepInfo.Step;
        if (stepInput is not IStepBaseOutput<TDIn, TDOut> step)
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