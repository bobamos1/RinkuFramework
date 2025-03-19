using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using Rinku.Tools.Data.Parsers;

namespace Rinku.Tools.Data;
public struct PropInfo {
    public required string Name;
    public required IList List;
    public required AppendJSONDelegate Appender;
}
public class DataTable : IEnumerable<DataRow> {
    protected readonly Dictionary<string, IList> Columns;
    public readonly PropInfo[] Properties;
    private DataTable(int len) {
        Columns = new(len);
        Properties = new PropInfo[len];
    }
    public static DataTable New<T>(List<DTInfo<T>> infos) {
        var len = infos.Count;
        var dt = new DataTable(len);
        for (int i = 0; i < len; i++) {
            var prop = infos[i];
            var list = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(prop.Type))!;
            dt.Properties[i] = new() {
                Name = prop.Name,
                List = list,
                Appender = null//prop.JSON.JSONAppender
            };
            dt.Columns[prop.Name] = list;
        }
        return dt;
    }
    public int Count => GetNbRows();
    public int NbCols => Columns.Count;
    public int GetNbRows() {
        using var e = Columns.GetEnumerator();
        return e.MoveNext() ? e.Current.Value.Count : 0;
    }
    public string ToJSON() {
        var sb = new StringBuilder();
        var count = Count;
        sb.Append('[');
        for (int i = 0; i < count; i++) {
            sb.Append('{');
            foreach (var item in Properties) {
                sb.Append('"').Append(item.Name).Append("\":");
                item.Appender(sb, i, item.List);
                sb.Append(',');
            }
            sb.Length--;
            sb.Append("},");
        }
        if (sb.Length > 1)
            sb.Length--;
        sb.Append(']');
        return sb.ToString();
    }
    public void Set<T>(string propertyName, int index, T value) => (GetColumn<T>(propertyName) ?? throw new Exception("property not present"))[index] = value;
    public string? Get(string propertyName, int index) => GetObj(propertyName, index)?.ToString();
    public object? GetObj(string propertyName, int index) => GetColumn(propertyName)?[index];
    public T? Get<T>(string propertyName, int index) {
        var col = GetColumn<T>(propertyName);
        if (col is null)
            return default;
        return col[index];
    }
    public IList? GetColumn(string propertyName) => Columns.TryGetValue(propertyName, out var col) ? col : null;
    public List<T?>? GetColumn<T>(string propertyName) => GetColumn(propertyName) as List<T?>;
    public IList this[string propertyName] {
        get => Columns[propertyName];
    }
    public DataRow this[int i] {
        get => new(i, this);
    }
    public IEnumerator<DataRow> GetEnumerator() {
        var count = Count;
        for (int i = 0; i < count; i++)
            yield return new DataRow(i, this);
    }
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
public readonly struct DataRow(int Index, DataTable DT) {
    public readonly T? Get<T>(string propertyName) => DT.Get<T>(propertyName, Index);
}