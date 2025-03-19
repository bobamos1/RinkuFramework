using System.Data;
using System.Text;
using DataTable.SpecificParsers;

namespace DataTable.Parsers;
public class DoubleParser : Parser<double>, IDataReaderParser<double> {
    private DoubleParser() : base(1) { }
    public override void AppendJson(StringBuilder sb, double item)
        => sb.Append(item);
    public double WithDataReader(IDataReader r, int i)
        => r.GetDouble(i);
}

public class NullDoubleParser : Parser<double?>, IDataReaderParser<double?> {
    private NullDoubleParser() : base(1) { }
    public override void AppendJson(StringBuilder sb, double? item) {
        if (!AppendIfNull(sb, item))
            sb.Append(item.Value);
    }
    public double? WithDataReader(IDataReader r, int i)
        => r.IsDBNull(i) ? null : r.GetDouble(i);
}
