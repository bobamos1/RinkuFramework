namespace DataTable;
public interface IDTMaker {
    public void Add<T>(string name);
}
public abstract class DTMaker<TDel> : List<TDel>, IDTMaker {
    public int CurrentCol = 0;
    public DataTable DT = new();
    public DTMaker() { }
    public void Add<T>(string name) {
        var parserBase = Parser<T>.Instance ?? throw new Exception($"Type {typeof(T)} is not supported");
        var parser = IParser<TDel, T>.Instance ?? throw new Exception("");
        var list = DT.AddColumn<T>(name, parserBase.AppendJson);
        Add(parser.GetDelegate(list, CurrentCol));
        CurrentCol += parserBase.NbColUsed;
    }
}