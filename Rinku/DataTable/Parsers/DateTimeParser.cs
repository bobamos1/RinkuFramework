using System.Data;
using System.Text;
using DataTable.SpecificParsers;

namespace DataTable.Parsers;
public class DateTimeParser : Parser<DateTime>, IDataReaderParser<DateTime> {
    private DateTimeParser() : base(1) { }
    public override void AppendJson(StringBuilder sb, DateTime item)
        => sb.Append('"').Append(item).Append('"');
    public DateTime WithDataReader(IDataReader r, int i)
        => r.GetDateTime(i);
}

public class NullDateTimeParser : Parser<DateTime?>, IDataReaderParser<DateTime?> {
    private NullDateTimeParser() : base(1) { }
    public override void AppendJson(StringBuilder sb, DateTime? item) {
        if (!AppendIfNull(sb, item))
            sb.Append('"').Append(item.Value).Append('"');
    }
    public DateTime? WithDataReader(IDataReader r, int i)
        => r.IsDBNull(i) ? null : r.GetDateTime(i);
}
