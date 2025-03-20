using System.Data;
using System.Text;
using Data.ParserInterfaces;

namespace Data.Parsers;
public class BoolParser : IDataReaderParserWithNull<bool> {
    public int NbColUsed => 1;
    public void AppendJson(StringBuilder sb, bool item)
        => sb.Append(item ? "true" : "false");
    public bool WithDataReader(IDataReader r, int i)
        => r.GetBoolean(i);
}