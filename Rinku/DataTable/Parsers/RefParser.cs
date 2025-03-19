using System.Data;
using System.Text;
using DataTable.SpecificParsers;

namespace DataTable.Parsers;
public struct RefItem<TID, TV> {
    public required TID? ID;
    public required TV? Value;
}
public class RefParser<TID, TV> : Parser<RefItem<TID, TV>>, IDataReaderParser<RefItem<TID, TV>> {
    private RefParser() : base(2) { }
    public override void AppendJson(StringBuilder sb, RefItem<TID, TV> item) {
        sb.Append("{\"ID\":");
        Parser<TID>.Instance!.AppendJson(sb, item.ID);
        sb.Append(",\"Value\":");
        Parser<TV>.Instance!.AppendJson(sb, item.Value);
        sb.Append('}');
    }
    public RefItem<TID, TV> WithDataReader(IDataReader r, int i)
        => new() {
            ID = IDataReaderParser<TID>.Instance!.WithDataReader(r, i),
            Value = IDataReaderParser<TV>.Instance!.WithDataReader(r, i + 1)
        };
}
public class NullRefParser<TID, TV> : Parser<RefItem<TID, TV>?>, IDataReaderParser<RefItem<TID, TV>?> {
    private NullRefParser() : base(2) { }
    public override void AppendJson(StringBuilder sb, RefItem<TID, TV>? item) {
        if (AppendIfNull(sb, item))
            return;
        sb.Append("{\"ID\":");
        Parser<TID>.Instance!.AppendJson(sb, item.Value.ID);
        sb.Append(",\"Value\":");
        Parser<TV>.Instance!.AppendJson(sb, item.Value.Value);
        sb.Append('}');
    }
    public RefItem<TID, TV>? WithDataReader(IDataReader r, int i) {
        if (r.IsDBNull(i))
            return null;
        return new() {
            ID = IDataReaderParser<TID>.Instance!.WithDataReader(r, i),
            Value = IDataReaderParser<TV>.Instance!.WithDataReader(r, i + 1)
        };
    }
}
