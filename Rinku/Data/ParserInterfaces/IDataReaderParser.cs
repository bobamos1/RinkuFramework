using System.Data;

namespace Data.ParserInterfaces;
public delegate void WithDataReader(IDataReader reader);
public interface IDataReaderParser<T> : IParser<WithDataReader, T> {
    public static new IDataReaderParser<T>? Instance = IParser<WithDataReader, T>.Instance as IDataReaderParser<T>;
    public int NbColUsed { get; }
    public T? WithDataReader(IDataReader r, int i);
}
public interface IDataReaderParserWithNull<T> 
    : IDataReaderParser<T>, IDataReaderParser<T?>, IParserWithNull<WithDataReader, T> where T : struct {
    T? IDataReaderParser<T?>.WithDataReader(IDataReader r, int i) 
        => r.IsDBNull(i) ? default : ((IDataReaderParser<T>)this).WithDataReader(r, i);
}