using System.Data;
using System.Text;
using Data.ParserInterfaces;

namespace DataTable.Parsers;
public class CharParser : IDataReaderParserWithNull<char> {
    public int NbColUsed => 1;
    public void AppendJson(StringBuilder sb, char item)
        => sb.Append('"').Append(item).Append('"');
    public char WithDataReader(IDataReader r, int i)
        => r.GetChar(i);
}