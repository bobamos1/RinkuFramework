using System.Text;

namespace Rinku.Http.ConditionalColumns;
public class SingleValCondCol2(string Name, string SQL) : SimpleCondCol2<string>(Name) {
    public string SQL = SQL;
    public override void AppendSQL(StringBuilder sb) => sb.Append(SQL);
}
