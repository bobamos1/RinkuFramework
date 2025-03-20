using System.Data;
using System.Text;
using Data.ParserInterfaces;

namespace Data.Parsers;
public class IntParser : IDataReaderParserWithNull<int> {
    public int NbColUsed => 1;
    public void AppendJson(StringBuilder sb, int item)
        => sb.Append(item);
    public int WithDataReader(IDataReader r, int i)
        => r.GetInt32(i);
}