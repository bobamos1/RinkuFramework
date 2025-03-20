namespace Processes;
public interface IStepBase<TDIn, TDOut>;
public interface IStepBase<TSegment, TDIn, TDOut> {
    public TSegment GetSegment<TFirst>();
}
public interface IStepBaseInput<TDIn, TDOut, in TIn> {
    public TDOut GenerateDisplay(TDIn dInput, TIn input);
}
public interface IStepBaseInput<TSegment, TDIn, TDOut, in TIn> 
    : IStepBaseInput<TDIn, TDOut, TIn>, IStepBase<TSegment, TDIn, TDOut>;
public interface IStepBaseOutput<TDIn, TDOut> {
    public string StepName { get; }
    public string ProcessName { set; }
    public TDOut ProcessAnswer(TDIn dInput);
    public void PopulateProcessSteps(ProcessSteps<TDIn, TDOut> processSteps);
}
public interface IStepBaseOutput<TSegment, TDIn, TDOut> 
    : IStepBaseOutput<TDIn, TDOut>, IStepBase<TSegment, TDIn, TDOut>;