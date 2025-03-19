using System.Data;

namespace DataTable.SpecificParsers;
public delegate void WithDataReader(IDataReader reader);
public interface IDataReaderParser<T> : IParser<WithDataReader, T> {
    public new static IDataReaderParser<T>? Instance => IParser<WithDataReader, T>.Instance as IDataReaderParser<T>;
    public T? WithDataReader(IDataReader r, int i);
    WithDataReader IParser<WithDataReader, T>.GetDelegate(List<T?> list, int indexCol)
        => r => list.Add(WithDataReader(r, indexCol));
}
