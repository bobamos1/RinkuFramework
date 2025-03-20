namespace Data;
public interface IDTMaker {
    public void Add<T>(string name);
}
public abstract class DTMaker<TDel> : List<TDel>, IDTMaker {
    public DataTable DT = new();
    public DTMaker() { }
    public abstract TDel? GetDel<T>(IParser<TDel, T> Parser, List<T?> list);
    public void Add<T>(string name) {
        var parser = IParser<TDel, T>.Instance ?? throw new Exception($"Type {typeof(T)} is not supported");
        var list = DT.AddColumn<T>(name, parser.AppendJson);
        Add(GetDel(parser, list) ?? throw new Exception($"Type {typeof(TDel)} is not supported"));
    }
}
