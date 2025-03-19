using System.Data;
using System.Text;
using DataTable.SpecificParsers;

namespace DataTable.Parsers;
public class IntParser : Parser<int>, IDataReaderParser<int> {
    private IntParser() : base(1) { }
    public override void AppendJson(StringBuilder sb, int item)
        => sb.Append(item);
    public int WithDataReader(IDataReader r, int i)
        => r.GetInt32(i);
}

public class NullIntParser : Parser<int?>, IDataReaderParser<int?> {
    private NullIntParser() : base(1) { }
    public override void AppendJson(StringBuilder sb, int? item) {
        if (!AppendIfNull(sb, item))
            sb.Append(item.Value);
    }
    public int? WithDataReader(IDataReader r, int i)
        => r.IsDBNull(i) ? null : r.GetInt32(i);
}
