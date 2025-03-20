using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Data;
public static class ParserHelper {
    public static bool AppendIfNull<T>(this StringBuilder sb, [NotNullWhen(false)] T? item) {
        if (item is not null)
            return false;
        sb.Append("null");
        return true;
    }
}
public interface IParser<TDel, TType> {
    public static new IParser<TDel, TType>? Instance = TryGetParser();
    private static IParser<TDel, TType>? TryGetParser() {
        var type = typeof(TType);
        return null;
    }
    public void AppendJson(StringBuilder sb, TType? item);
}
public interface IParserWithNull<TDel, TType> : IParser<TDel, TType>, IParser<TDel, TType?> where TType : struct {
    void IParser<TDel, TType?>.AppendJson(StringBuilder sb, TType? item) {
        if (!sb.AppendIfNull(item))
            AppendJson(sb, item.Value);
    }
}