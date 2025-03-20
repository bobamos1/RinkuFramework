using System.Data;
using System.Text;
using Data.ParserInterfaces;

namespace Data.Parsers;
public class GuidParser : IDataReaderParserWithNull<Guid> {
    public int NbColUsed => 1;
    public void AppendJson(StringBuilder sb, Guid item)
        => sb.Append('"').Append(item).Append('"');
    public Guid WithDataReader(IDataReader r, int i)
        => r.GetGuid(i);
}