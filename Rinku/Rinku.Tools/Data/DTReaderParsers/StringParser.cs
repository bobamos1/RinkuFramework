using System.Data;
using System.Text;

namespace Rinku.Tools.Data.DTReaderParsers;
/*
public class StringParser(string Name) : DTDataReaderProperty<string?>(Name) {
    public override void AppendJSON(StringBuilder sb, string? item) {
        if (item is null) {
            sb.Append("null");
            return;
        }
        sb.Append('"').Append(item).Append('"');
    }
    public override string? GetValue(IDataReader reader, Dictionary<string, int> template) {
        int index = template[Name];
        return reader.IsDBNull(index) ? null : reader.GetString(index);
    }
}
*/