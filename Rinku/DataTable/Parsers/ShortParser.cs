using System.Data;
using System.Text;
using DataTable.SpecificParsers;

namespace DataTable.Parsers;
public class ShortParser : Parser<short>, IDataReaderParser<short> {
    private ShortParser() : base(1) { }
    public override void AppendJson(StringBuilder sb, short item)
        => sb.Append(item);
    public short WithDataReader(IDataReader r, int i)
        => r.GetInt16(i);
}

public class NullShortParser : Parser<short?>, IDataReaderParser<short?> {
    private NullShortParser() : base(1) { }
    public override void AppendJson(StringBuilder sb, short? item) {
        if (!AppendIfNull(sb, item))
            sb.Append(item.Value);
    }
    public short? WithDataReader(IDataReader r, int i)
        => r.IsDBNull(i) ? null : r.GetInt16(i);
}