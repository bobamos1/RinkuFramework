using System.Data;
using System.Text;
using Data.ParserInterfaces;

namespace Data.Parsers;
public class DoubleParser : IDataReaderParserWithNull<double> {
    public int NbColUsed => 1;
    public void AppendJson(StringBuilder sb, double item)
        => sb.Append(item);
    public double WithDataReader(IDataReader r, int i)
        => r.GetDouble(i);
}