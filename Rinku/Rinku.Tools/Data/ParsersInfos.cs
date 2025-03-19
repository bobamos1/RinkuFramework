using System.Diagnostics.CodeAnalysis;
using System.Text;
using Rinku.Tools.Data.Parsers;

namespace Rinku.Tools.Data;
public static class ParsersInfos {
    public static DataReaderParsersInfo DataReaderParsersInfo = [];
    static ParsersInfos() {
        SetJSON();
        SetDataReader();
    }
    public static bool AppendIfNull<T>(StringBuilder sb, [NotNullWhen(false)] T? item) {
        if (item is not null)
            return false;
        sb.Append("null");
        return true;
    }
    public static void SetJSON() {
        ParserBaseInfos.Bases
            .Add<string>((sb, item) => { if (!AppendIfNull(sb, item)) sb.Append('"').Append(item).Append('"'); })
            .Add<bool>((sb, item) => sb.Append(item ? "true" : "false"))
            .Add<bool?>((sb, item) => { if (!AppendIfNull(sb, item)) sb.Append(item.Value ? "true" : "false"); })
            .Add<byte>((sb, item) => sb.Append(item))
            .Add<byte?>((sb, item) => { if (!AppendIfNull(sb, item)) sb.Append(item.Value); })
            .Add<short>((sb, item) => sb.Append(item))
            .Add<short?>((sb, item) => { if (!AppendIfNull(sb, item)) sb.Append(item.Value); })
            .Add<int>((sb, item) => sb.Append(item))
            .Add<int?>((sb, item) => { if (!AppendIfNull(sb, item)) sb.Append(item.Value); })
            .Add<long>((sb, item) => sb.Append(item))
            .Add<long?>((sb, item) => { if (!AppendIfNull(sb, item)) sb.Append(item.Value); })
            .Add<float>((sb, item) => sb.Append(item))
            .Add<float?>((sb, item) => { if (!AppendIfNull(sb, item)) sb.Append(item.Value); })
            .Add<double>((sb, item) => sb.Append(item))
            .Add<double?>((sb, item) => { if (!AppendIfNull(sb, item)) sb.Append(item.Value); })
            .Add<decimal>((sb, item) => sb.Append(item))
            .Add<decimal?>((sb, item) => { if (!AppendIfNull(sb, item)) sb.Append(item.Value); })
            .Add<DateTime>((sb, item) => sb.Append('"').Append(item).Append('"'))
            .Add<DateTime?>((sb, item) => { if (!AppendIfNull(sb, item)) sb.Append('"').Append(item.Value).Append('"'); })
            .Add<char>((sb, item) => sb.Append('"').Append(item).Append('"'))
            .Add<char?>((sb, item) => { if (!AppendIfNull(sb, item)) sb.Append('"').Append(item.Value).Append('"'); })
            .Add<Guid>((sb, item) => sb.Append('"').Append(item).Append('"'))
            .Add<Guid?>((sb, item) => { if (!AppendIfNull(sb, item)) sb.Append('"').Append(item.Value).Append('"'); });
    }
    public static void SetDataReader() {
        DataReaderParsersInfo
            .SetParser((r, i) => r.IsDBNull(i) ? null : r.GetString(i))
            .SetParser((r, i) => r.GetBoolean(i))
            .SetParser((r, i) => r.GetByte(i))
            .SetParser((r, i) => r.GetInt16(i))
            .SetParser((r, i) => r.GetInt32(i))
            .SetParser((r, i) => r.GetInt64(i))
            .SetParser((r, i) => r.GetFloat(i))
            .SetParser((r, i) => r.GetDouble(i))
            .SetParser((r, i) => r.GetDecimal(i))
            .SetParser((r, i) => r.GetDateTime(i))
            .SetParser((r, i) => r.GetChar(i))
            .SetParser((r, i) => r.GetGuid(i))
            .SetParser<bool?>((r, i) => r.IsDBNull(i) ? null : r.GetBoolean(i))
            .SetParser<byte?>((r, i) => r.IsDBNull(i) ? null : r.GetByte(i))
            .SetParser<short?>((r, i) => r.IsDBNull(i) ? null : r.GetInt16(i))
            .SetParser<int?>((r, i) => r.IsDBNull(i) ? null : r.GetInt32(i))
            .SetParser<long?>((r, i) => r.IsDBNull(i) ? null : r.GetInt64(i))
            .SetParser<float?>((r, i) => r.IsDBNull(i) ? null : r.GetFloat(i))
            .SetParser<double?>((r, i) => r.IsDBNull(i) ? null : r.GetDouble(i))
            .SetParser<decimal?>((r, i) => r.IsDBNull(i) ? null : r.GetDecimal(i))
            .SetParser<DateTime?>((r, i) => r.IsDBNull(i) ? null : r.GetDateTime(i))
            .SetParser<char?>((r, i) => r.IsDBNull(i) ? null : r.GetChar(i))
            .SetParser<Guid?>((r, i) => r.IsDBNull(i) ? null : r.GetGuid(i));
    }
}
