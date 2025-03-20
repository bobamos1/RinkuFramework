namespace Processes;
public abstract class StepBase<TDIn, TDOut>
    : IStepBaseOutput<TDIn, TDOut> {
    public string StepName { get; init; }
    public string Endpoint = null!;
    protected string _processName = null!;
    public string ProcessName {
        set {
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
    //public abstract TSegment GetSegment<TFirst>();
}