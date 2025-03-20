using System.Data;
using System.Text;
using Data.ParserInterfaces;

namespace Data.Parsers;
public class FloatParser : IDataReaderParserWithNull<float> {
    public int NbColUsed => 1;
    public void AppendJson(StringBuilder sb, float item)
        => sb.Append(item);
    public float WithDataReader(IDataReader r, int i)
        => r.GetFloat(i);
}