namespace Rinku.Tools.Data.Parsers;
public class Parser<TPopulateDel> {
    public required object ParserDelegate;
    public required Func<object, int, TPopulateDel> DelegateGetter;
    public required TPopulateDel[] CachedParserDelegates;
    public void UpdateCaching(int nbCached) {
        var len = CachedParserDelegates.Length;
        if (len == nbCached)
            return;
        var temp = new TPopulateDel[nbCached];
        var top = len > nbCached ? nbCached : len;
        if (top > 0)
            for (int i = 0; i < top; i++)
                temp[i] = CachedParserDelegates[i];
        if (len < nbCached)
            for (int i = 0; i < nbCached; i++)
                temp[i] = DelegateGetter(ParserDelegate, i);
        CachedParserDelegates = temp;
    }
    public void ForceUpdateCaching(int nbCached) {
        CachedParserDelegates = new TPopulateDel[nbCached];
        for (int i = 0; i < nbCached; i++)
            CachedParserDelegates[i] = DelegateGetter(ParserDelegate, i);
    }
    internal TPopulateDel GetDelegate(int i) => DelegateGetter(ParserDelegate, i);
    internal TPopulateDel GetCachedDelegate(int i) {
        if (i < CachedParserDelegates.Length)
            return CachedParserDelegates[i];
        return DelegateGetter(ParserDelegate, i);
    }
}
public abstract class ParsersInfo<TPopulateDel> : Dictionary<Type, Parser<TPopulateDel>> {
    public static int NbToCache { get; set; } = 5;
    protected void SettingParser<T, TGetValDel>(TGetValDel parser, int nbCached = -1) where TGetValDel : class {
        if (nbCached == -1) 
            nbCached = NbToCache;
        if (!ParserBaseInfos.Bases.ContainsKey(typeof(T)))
            throw new Exception();
        Parser<TPopulateDel> parserInfo = new() {
            ParserDelegate = parser,
            DelegateGetter = GetDelegate<T>,
            CachedParserDelegates = []
        };
        parserInfo.UpdateCaching(nbCached);
        this[typeof(T)] = parserInfo;
    }
    public void UpdateCaching(int nbCached) {
        foreach (var info in Values)
            info.UpdateCaching(nbCached);
    }
    protected abstract TPopulateDel GetDelegate<T>(object del, int i);

    public TPopulateDel GetDelegate(Type type, int i) => this[type].GetDelegate(i);
    public TPopulateDel GetCachedDelegate(Type type, int i) => this[type].GetCachedDelegate(i);
}
