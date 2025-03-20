using System.Data;
using System.Text;
using Data.ParserInterfaces;

namespace Data.Parsers;
public struct RefItem<TID, TV> {
    public required TID? ID;
    public required TV? Value;
}
public class RefParser<TID, TV> : IDataReaderParserWithNull<RefItem<TID, TV>> {
    public int NbColUsed => 2;
    public void AppendJson(StringBuilder sb, RefItem<TID, TV> item) {
        sb.Append("{\"ID\":");
        IDataReaderParser<TID>.Instance!.AppendJson(sb, item.ID);
        sb.Append(",\"Value\":");
        IDataReaderParser<TV>.Instance!.AppendJson(sb, item.Value);
        sb.Append('}');
    }
    public RefItem<TID, TV> WithDataReader(IDataReader r, int i)
        => new() {
            ID = IDataReaderParser<TID>.Instance!.WithDataReader(r, i),
            Value = IDataReaderParser<TV>.Instance!.WithDataReader(r, i + 1)
        };
}