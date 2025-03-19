using System.Data;
using System.Text;
using DataTable.SpecificParsers;

namespace DataTable.Parsers;
public class BoolParser : Parser<bool>, IDataReaderParser<bool> {
    private BoolParser() : base(1) { }
    public override void AppendJson(StringBuilder sb, bool item)
        => sb.Append(item ? "true" : "false");
    public bool WithDataReader(IDataReader r, int i)
        => r.GetBoolean(i);
}

public class NullBoolParser : Parser<bool?>, IDataReaderParser<bool?> {
    private NullBoolParser() : base(1) { }
    public override void AppendJson(StringBuilder sb, bool? item) {
        if (!AppendIfNull(sb, item))
            sb.Append(item.Value ? "true" : "false");
    }
    public bool? WithDataReader(IDataReader r, int i)
        => r.IsDBNull(i) ? null : r.GetBoolean(i);
}
