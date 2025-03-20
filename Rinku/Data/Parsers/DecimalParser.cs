using System.Data;
using System.Text;
using Data.ParserInterfaces;

namespace Data.Parsers;
public class DecimalParser : IDataReaderParserWithNull<decimal> {
    public int NbColUsed => 1;
    public void AppendJson(StringBuilder sb, decimal item)
        => sb.Append(item);
    public decimal WithDataReader(IDataReader r, int i)
        => r.GetDecimal(i);
}