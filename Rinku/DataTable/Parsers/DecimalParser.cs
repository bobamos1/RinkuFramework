using System.Data;
using System.Text;
using DataTable.SpecificParsers;

namespace DataTable.Parsers;
public class DecimalParser : Parser<decimal>, IDataReaderParser<decimal> {
    private DecimalParser() : base(1) { }
    public override void AppendJson(StringBuilder sb, decimal item)
        => sb.Append(item);
    public decimal WithDataReader(IDataReader r, int i)
        => r.GetDecimal(i);
}

public class NullDecimalParser : Parser<decimal?>, IDataReaderParser<decimal?> {
    private NullDecimalParser() : base(1) { }
    public override void AppendJson(StringBuilder sb, decimal? item) {
        if (!AppendIfNull(sb, item))
            sb.Append(item.Value);
    }
    public decimal? WithDataReader(IDataReader r, int i)
        => r.IsDBNull(i) ? null : r.GetDecimal(i);
}
