using BenchmarkDotNet.Attributes;
using Rinku.Tools.Data;
using Rinku.Tools.Data.DTMakers;
using Rinku.Tools.Data.Parsers;
namespace Test;
[MemoryDiagnoser]
[ShortRunJob]
public class LookupBenchmark {
    private string[] _keys = [];
    private Type[] _types = [
        typeof(string),
        typeof(int),
        typeof(DateTime),
        typeof(float),
        typeof(decimal),
    ];
    private List<WithDataReaderDelegate> List;
    private DataReaderDTMaker Maker;
    // Various collection sizes to test
    //[Params(5, 10, 20)]
    public int _size = 5;
    //[Params(10, 25, 100)]
    public int _count = 10;
    public int pp;
    // Setup method to initialize both List and Dictionary with the same data
    [GlobalSetup]
    public void Setup() {
        _keys = new string[_size];
        List = new(_size);
        Maker = new(_size);
        for (int i = 0; i < _size; i++)
            _keys[i] = "itemtosearchfor" + i;
       pp = ParsersInfos.DataReaderParsersInfo.Count;
    }
    [Benchmark]
    public bool Delegate() {
        List.Clear();
        for (int i = 0; i < _size; i++)
            List.Add(ParsersInfos.DataReaderParsersInfo.GetDelegate(_types[i % _types.Length], i));
        var res = 0;
        for (int j = 0; j < _count; j++)
            foreach (var parser in List)
                res += parser(null!, null!);
        return res >= 0;
    }
    [Benchmark]
    public bool DelegateCached() {
        List.Clear();
        for (int i = 0; i < _size; i++)
            List.Add(ParsersInfos.DataReaderParsersInfo.GetCachedDelegate(_types[i % _types.Length], i));
        var res = 0;
        for (int j = 0; j < _count; j++)
            foreach (var parser in List)
                res += parser(null!, null!);
        return res >= 0;
    }
    [Benchmark(Baseline = true)]
    public bool DelegateNew() {
        Maker.Clear();
        for (int i = 0; i < _size; i++)
            Maker.Add(_types[i % _types.Length], _keys[i]);
        var res = 0;
        for (int j = 0; j < _count; j++)
            foreach (var parser in Maker)
                res += parser.Parser(null!, null!);
        return res >= 0;
    }
    [Benchmark]
    public bool DelegateNewCached() {
        Maker.Clear();
        for (int i = 0; i < _size; i++)
            Maker.AddCached(_types[i % _types.Length], _keys[i]);
        var res = 0;
        for (int j = 0; j < _count; j++)
            foreach (var parser in Maker)
                res += parser.Parser(null!, null!);
        return res >= 0;
    }
}