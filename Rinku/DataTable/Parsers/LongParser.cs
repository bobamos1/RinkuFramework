using System.Data;
using System.Text;
using DataTable.SpecificParsers;

namespace DataTable.Parsers;
public class LongParser : Parser<long>, IDataReaderParser<long> {
    private LongParser() : base(1) { }
    public override void AppendJson(StringBuilder sb, long item)
        => sb.Append(item);
    public long WithDataReader(IDataReader r, int i)
        => r.GetInt64(i);
}

public class NullLongParser : Parser<long?>, IDataReaderParser<long?> {
    private NullLongParser() : base(1) { }
    public override void AppendJson(StringBuilder sb, long? item) {
        if (!AppendIfNull(sb, item))
            sb.Append(item.Value);
    }
    public long? WithDataReader(IDataReader r, int i)
        => r.IsDBNull(i) ? null : r.GetInt64(i);
}