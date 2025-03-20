using System.Data;
using System.Text;
using Data.ParserInterfaces;

namespace Data.Parsers;
public class ShortParser : IDataReaderParserWithNull<short> {
    public int NbColUsed => 1;
    public void AppendJson(StringBuilder sb, short item)
        => sb.Append(item);
    public short WithDataReader(IDataReader r, int i)
        => r.GetInt16(i);
}