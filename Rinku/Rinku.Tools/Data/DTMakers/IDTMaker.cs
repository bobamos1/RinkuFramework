using Rinku.Tools.Data.Parsers;

namespace Rinku.Tools.Data.DTMakers;
public interface IDTMaker {
    public void Add(Type type, string name);
    public void AddCached(Type type, string name);
    public void Add(Type type, string name, int i);
    public void AddCached(Type type, string name, int i);
}

public abstract class DTMaker<TDel, TParser> : List<DTInfo<TDel>>, IDTMaker where TParser : ParsersInfo<TDel> {
    public DTMaker() {}
    public DTMaker(int length) : base(length) {}
    public int CurrentCol = 0;
    public abstract TParser Parser { get; }
    public void Add(Type type, string name) {
        var parserBase = ParserBaseInfos.Bases[type];
        Add(new() {
            JSON = parserBase,
            Parser = Parser[type].GetDelegate(CurrentCol),
            Name = name,
            Type = type
        });
        CurrentCol += parserBase.NbColUsed;
    }
    public void AddCached(Type type, string name) {
        var parserBase = ParserBaseInfos.Bases[type];
        Add(new() {
            JSON = parserBase,
            Parser = Parser[type].GetCachedDelegate(CurrentCol),
            Name = name,
            Type = type
        });
        CurrentCol += parserBase.NbColUsed;
    }
    public void Add(Type type, string name, int i) => Add(new() {
        JSON = ParserBaseInfos.Bases[type],
        Parser = Parser[type].GetDelegate(i),
        Name = name,
        Type = type
    });

    public void AddCached(Type type, string name, int i) => Add(new() {
        JSON = ParserBaseInfos.Bases[type],
        Parser = Parser[type].GetCachedDelegate(i),
        Name = name,
        Type = type
    }); 
}