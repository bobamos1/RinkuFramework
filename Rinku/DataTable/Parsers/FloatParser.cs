using System.Data;
using System.Text;
using DataTable.SpecificParsers;

namespace DataTable.Parsers;
public class FloatParser : Parser<float>, IDataReaderParser<float> {
    private FloatParser() : base(1) { }
    public override void AppendJson(StringBuilder sb, float item)
        => sb.Append(item);
    public float WithDataReader(IDataReader r, int i)
        => r.GetFloat(i);
}

public class NullFloatParser : Parser<float?>, IDataReaderParser<float?> {
    private NullFloatParser() : base(1) { }
    public override void AppendJson(StringBuilder sb, float? item) {
        if (!AppendIfNull(sb, item))
            sb.Append(item.Value);
    }
    public float? WithDataReader(IDataReader r, int i)
        => r.IsDBNull(i) ? null : r.GetFloat(i);
}
