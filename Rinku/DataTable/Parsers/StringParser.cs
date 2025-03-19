using System.Data;
using System.Text;
using DataTable.SpecificParsers;

namespace DataTable.Parsers;
public class StringParser : Parser<string>, IDataReaderParser<string> {
    private StringParser() : base(1) { }
    public override void AppendJson(StringBuilder sb, string? item) {
        if (!AppendIfNull(sb, item))
            sb.Append('"').Append(item).Append('"');
    }
    public string? WithDataReader(IDataReader r, int i)
        => r.IsDBNull(i) ? null : r.GetString(i);
}
public class NullStringParser : Parser<string?>, IDataReaderParser<string?> {
    private NullStringParser() : base(1) { }
    public override void AppendJson(StringBuilder sb, string? item) {
        if (!AppendIfNull(sb, item))
            sb.Append('"').Append(item).Append('"');
    }
    public string? WithDataReader(IDataReader r, int i)
        => r.IsDBNull(i) ? null : r.GetString(i);
}
