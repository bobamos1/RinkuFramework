using System.Collections;
using System.Text;
using Rinku.Tools.Data.Parsers;
using IDataReader = System.Data.IDataReader;

namespace Rinku.Tools.Data;
/*
public interface IDTDataReaderProperty : IDTProperty {
    public void Populate(IList column, IDataReader reader, Dictionary<string, int> template);
}

public abstract class DTDataReaderProperty<T>(string Name) : IDTDataReaderProperty {
    public Type Type => typeof(T);
    public string Name { get; } = Name;
    public void AppendJSON(StringBuilder sb, int index, IList list) {
        sb.Append('"').Append(Name).Append("\":");
        AppendJSON(sb, ((List<T?>)list)[index]);
    }
    public abstract void AppendJSON(StringBuilder sb, T? item);
    public abstract T? GetValue(IDataReader reader, Dictionary<string, int> template);
    public void Populate(IList column, IDataReader reader, Dictionary<string, int> template)
        => ((List<T?>)column).Add(GetValue(reader, template));
}

public static class DataReaderParser {
    public static DataTable GetDataTable<T>(DTInfo<T>[] parsers, IDataReader data, Dictionary<string, int> template) {
        var dt = DataTable.New(parsers);
        try {
            while (data.Read())
                foreach (var pair in dt.Properties)
                    pair.Key.Populate(pair.Value, data, template);
        }
        finally {
            data.Dispose();
        }
        return dt;
    }
}
*/