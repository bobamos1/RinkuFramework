namespace Processes;
public struct Segment<TFirst, TAddingDelegate> {
    public TFirst First;
    public TAddingDelegate Then;
}