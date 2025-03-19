using System.Collections;
using System.Text;

namespace Rinku.Tools.Data.Parsers;
public interface IParserBase {
    public void AppendJson(StringBuilder sb, int index, IList list);
}
public abstract class ParserBase2(int NbColUsed) : IParserBase {
    public int NbColUsed = NbColUsed;
    public abstract void AppendJson(StringBuilder sb, int index, IList list);
}
