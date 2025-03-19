using System.Collections;
using System.Data;
using System.Data.Common;
using System.Reflection.PortableExecutable;
using Rinku.Tools.Data.Parsers;
/*
namespace Rinku.Tools.Data.DTReaderParsers;
public class DataReaderParsersBuilder {
    public int CurrentCount = 0;
    public List<object> Parsers = [];
    public static Dictionary<Type, object> ValuesParsers = [];
    public static void AddTypeParser<T>(GetValDelegate<T> parser) => ValuesParsers[typeof(T)] = parser;
    public static WithDataReaderDelegate GetDelegate<T>(int index) {
        var del = (GetValDelegate<T>)ValuesParsers[typeof(T)];
        return (l, r) => index;//((List<T?>)l).Add(del(r, index));
    }
    static DataReaderParsersBuilder() {
        AddTypeParser((r, i) => r.IsDBNull(i) ? null : r.GetString(i));
        AddTypeParser((r, i) => r.GetBoolean(i));
        AddTypeParser((r, i) => r.GetByte(i));
        AddTypeParser((r, i) => r.GetInt16(i));
        AddTypeParser((r, i) => r.GetInt32(i));
        AddTypeParser((r, i) => r.GetInt64(i));
        AddTypeParser((r, i) => r.GetFloat(i));
        AddTypeParser((r, i) => r.GetDouble(i));
        AddTypeParser((r, i) => r.GetDecimal(i));
        AddTypeParser((r, i) => r.GetDateTime(i));
        AddTypeParser((r, i) => r.GetChar(i));
        AddTypeParser((r, i) => r.GetGuid(i));
        AddTypeParser<bool?>((r, i) => r.IsDBNull(i) ? null : r.GetBoolean(i));
        AddTypeParser<byte?>((r, i) => r.IsDBNull(i) ? null : r.GetByte(i));
        AddTypeParser<short?>((r, i) => r.IsDBNull(i) ? null : r.GetInt16(i));
        AddTypeParser<int?>((r, i) => r.IsDBNull(i) ? null : r.GetInt32(i));
        AddTypeParser<long?>((r, i) => r.IsDBNull(i) ? null : r.GetInt64(i));
        AddTypeParser<float?>((r, i) => r.IsDBNull(i) ? null : r.GetFloat(i));
        AddTypeParser<double?>((r, i) => r.IsDBNull(i) ? null :r.GetDouble(i));
        AddTypeParser<decimal?>((r, i) => r.IsDBNull(i) ? null :r.GetDecimal(i));
        AddTypeParser<DateTime?>((r, i) => r.IsDBNull(i) ? null :r.GetDateTime(i));
        AddTypeParser<char?>((r, i) => r.IsDBNull(i) ? null :r.GetChar(i));
        AddTypeParser<Guid?>((r, i) => r.IsDBNull(i) ? null : r.GetGuid(i));
    }
}
*/