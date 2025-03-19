using System.Data;
using System.Text;
using DataTable.SpecificParsers;

namespace DataTable.Parsers;
public class CharParser : Parser<char>, IDataReaderParser<char> {
    private CharParser() : base(1) { }
    public override void AppendJson(StringBuilder sb, char item)
        => sb.Append('"').Append(item).Append('"');
    public char WithDataReader(IDataReader r, int i)
        => r.GetChar(i);
}

public class NullCharParser : Parser<char?>, IDataReaderParser<char?> {
    private NullCharParser() : base(1) { }
    public override void AppendJson(StringBuilder sb, char? item) {
        if (!AppendIfNull(sb, item))
            sb.Append('"').Append(item.Value).Append('"');
    }
    public char? WithDataReader(IDataReader r, int i)
        => r.IsDBNull(i) ? null : r.GetChar(i);
}
