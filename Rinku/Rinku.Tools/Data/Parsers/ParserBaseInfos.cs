using System.Collections;
using System.Text;

namespace Rinku.Tools.Data.Parsers;
public delegate void AppendJSONDelegate<T>(StringBuilder sb, T? item);
public delegate void AppendJSONDelegate(StringBuilder sb, int index, IList list);
public class ParserBase {
    public required int NbColUsed;
    public required AppendJSONDelegate JSONAppender;
}
public class ParserBaseInfos : Dictionary<Type, ParserBase> {
    private ParserBaseInfos() {}
    public static readonly ParserBaseInfos Bases = [];
    public ParserBaseInfos Add<T>(AppendJSONDelegate<T> appender, int nbColUsed = 1) {
        this[typeof(T)] = new() { 
            JSONAppender = (sb, i, l) => appender(sb, ((List<T?>)l)[i]),
            NbColUsed = nbColUsed
        };
        return this;
    }
}
