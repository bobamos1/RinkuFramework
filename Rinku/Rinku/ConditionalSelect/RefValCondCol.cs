using System.Text;
using DataTable.Parsers;
using Rinku.Context;

namespace Rinku.ConditionalSelect;
public abstract class RefValCondColBase<TRefItem, TID, TVal>(string Name, string SQLID, string SQLValue) : SimpleCondCol<TRefItem>(Name) {
    public string SQLID = SQLID;
    public string SQLValue = SQLValue;
    public string SQLIDName = Name + "ID";
    public override void AddColumns(HashSet<string> columns) {
        columns.Add(SQLIDName);
        columns.Add(Name);
    }
    public override void AppendSQL(StringBuilder sb, IContext ctx) {
        sb.Append(SQLID).Append(" AS [").Append(SQLIDName).Append("], ").Append(SQLValue);
    }
}
public class RefValCondCol<TID, TVal>(string Name, string SQLID, string SQLValue)
    : RefValCondColBase<RefItem<TID, TVal>, TID, TVal>(Name, SQLID, SQLValue);
public class NullRefValCondCol<TID, TVal>(string Name, string SQLID, string SQLValue)
    : RefValCondColBase<RefItem<TID, TVal>?, TID, TVal>(Name, SQLID, SQLValue);
