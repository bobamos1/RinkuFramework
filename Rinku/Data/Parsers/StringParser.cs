using System.Data;
using System.Text;
using Data.ParserInterfaces;

namespace Data.Parsers;
public class StringParser : IDataReaderParser<string> {
    public int NbColUsed => 1;
    public void AppendJson(StringBuilder sb, string? item) {
        if (!sb.AppendIfNull(item))
            sb.Append('"').Append(item).Append('"');
    }
    public string? WithDataReader(IDataReader r, int i)
        => r.IsDBNull(i) ? null : r.GetString(i);
}