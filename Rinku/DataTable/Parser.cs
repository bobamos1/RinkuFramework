using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace DataTable;
public interface IParser<TDel, TType> {
    public static IParser<TDel, TType>? Instance = Parser<TType>.Instance as IParser<TDel, TType>;
    public TDel GetDelegate(List<TType?> list, int indexCol);
}
public abstract class Parser<T>(int NbColUsed) {
    public static Parser<T>? Instance = Registerer.GetParser<T, Parser<T>>();
    public int NbColUsed = NbColUsed;
    public abstract void AppendJson(StringBuilder sb, T? item);
    public static bool AppendIfNull(StringBuilder sb, [NotNullWhen(false)] T? item) {
        if (item is not null)
            return false;
        sb.Append("null");
        return true;
    }
}