using System.Data;
using System.Text;
using Data.ParserInterfaces;

namespace Data.Parsers;
public class DateTimeParser : IDataReaderParserWithNull<DateTime> {
    public int NbColUsed => 1;
    public void AppendJson(StringBuilder sb, DateTime item)
        => sb.Append('"').Append(item).Append('"');
    public DateTime WithDataReader(IDataReader r, int i)
        => r.GetDateTime(i);
}