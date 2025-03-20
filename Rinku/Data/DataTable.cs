using System.Collections;
using System.Text;

namespace Data;
public delegate void JSONAppendDelegate<T>(StringBuilder sb, T? item);
public delegate void JSONAppendDelegate(StringBuilder sb, int index);
public class DataTable : IEnumerable<DataRow> {
    public static readonly Type NullableType = typeof(Nullable<>);
    protected readonly Dictionary<string, IList> Columns = [];
    protected readonly List<JSONAppendDelegate> Appenders = [];
    public List<T?> AddColumn<T>(string name, JSONAppendDelegate<T> appender) {
        var list = new List<T?>();
        Columns.Add(name, list);
        Appenders.Add((sb, i) => {
            sb.Append('"').Append(name).Append("\":");
            appender(sb, list[i]);
        });
        return list;
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
            foreach (var appender in Appenders) {
                appender(sb, i);
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