using System.Data;
using System.Text;
/*
namespace Rinku.Tools.Data.DTReaderParsers;
public abstract class RefItemParser<TID, TVal>(string Name) : DTDataReaderProperty<RefItem<TID, TVal>>(Name) {
    public string IDName = Name + "ID";
    public override void AppendJSON(StringBuilder sb, RefItem<TID, TVal> item) {
        sb.Append("{\"").Append(nameof(item.ID)).Append("\":");
        AppendJSONID(sb, item.ID);
        sb.Append(",\"").Append(nameof(item.Value)).Append("\":");
        AppendJSONVal(sb, item.Value);
        sb.Append('}');
    }
    public abstract void AppendJSONID(StringBuilder sb, TID item);
    public abstract void AppendJSONVal(StringBuilder sb, TVal? item);
    public override RefItem<TID, TVal> GetValue(IDataReader reader, Dictionary<string, int> template) {
        return new RefItem<TID, TVal> {
            ID = GetItemID(reader, template),
            Value = GetItemValue(reader, template)
        };
    }
    public abstract TID GetItemID(IDataReader reader, Dictionary<string, int> template);
    public abstract TVal? GetItemValue(IDataReader reader, Dictionary<string, int> template);

}
public class RefItemParserIntString(string Name) : RefItemParser<int, string>(Name) {
    public override void AppendJSONID(StringBuilder sb, int item) => sb.Append(item);
    public override void AppendJSONVal(StringBuilder sb, string? item) {
        if (item is null) {
            sb.Append("null");
            return;
        }
        sb.Append('"').Append(item).Append('"');
    }
    public override int GetItemID(IDataReader reader, Dictionary<string, int> template)
        => reader.GetInt32(template[IDName]);
    public override string? GetItemValue(IDataReader reader, Dictionary<string, int> template)
        => reader.GetString(template[Name]);
}
public class RefItemParserNullIntString(string Name) : RefItemParser<int?, string>(Name) {
    public override void AppendJSONID(StringBuilder sb, int? item) {
        if (item is null) {
            sb.Append("null");
            return;
        }
        sb.Append(item);
    }
    public override void AppendJSONVal(StringBuilder sb, string? item) {
        if (item is null) {
            sb.Append("null");
            return;
        }
        sb.Append('"').Append(item).Append('"');
    }
    public override int? GetItemID(IDataReader reader, Dictionary<string, int> template) {
        int index = template[IDName];
        return reader.IsDBNull(index) ? null : reader.GetInt32(index);
    }
    public override string? GetItemValue(IDataReader reader, Dictionary<string, int> template) {
        int index = template[Name];
        return reader.IsDBNull(index) ? null : reader.GetString(index);
    }
}
*/