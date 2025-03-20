using System.Data;
using System.Text;
using Data.ParserInterfaces;

namespace Data.Parsers;
public class LongParser : IDataReaderParserWithNull<long> {
    public int NbColUsed => 1;
    public void AppendJson(StringBuilder sb, long item)
        => sb.Append(item);
    public long WithDataReader(IDataReader r, int i)
        => r.GetInt64(i);
}