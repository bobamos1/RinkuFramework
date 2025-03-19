using System.Collections;
using System.Data;

namespace Rinku.Tools.Data.Parsers;
public delegate T GetValDelegate<T>(IDataReader reader, int index);
public delegate int WithDataReaderDelegate(IList list, IDataReader reader);
public class DataReaderParsersInfo : ParsersInfo<WithDataReaderDelegate> {
    public DataReaderParsersInfo SetParser<T>(GetValDelegate<T> parser) {
        SettingParser<T, GetValDelegate<T>>(parser);
        return this;
    }
    protected override WithDataReaderDelegate GetDelegate<T>(object del, int i) {
        var dele = (GetValDelegate<T>)del;
        return (l, r) => i;//((List<T?>)l).Add(dele(r, i));
    }
}
