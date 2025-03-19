using System.Data;
using System.Text;
using DataTable.SpecificParsers;

namespace DataTable.Parsers;
public class GuidParser : Parser<Guid>, IDataReaderParser<Guid> {
    private GuidParser() : base(1) { }
    public override void AppendJson(StringBuilder sb, Guid item)
        => sb.Append('"').Append(item).Append('"');
    public Guid WithDataReader(IDataReader r, int i)
        => r.GetGuid(i);
}

public class NullGuidParser : Parser<Guid?>, IDataReaderParser<Guid?> {
    private NullGuidParser() : base(1) { }
    public override void AppendJson(StringBuilder sb, Guid? item) {
        if (!AppendIfNull(sb, item))
            sb.Append('"').Append(item.Value).Append('"');
    }
    public Guid? WithDataReader(IDataReader r, int i)
        => r.IsDBNull(i) ? null : r.GetGuid(i);
}
