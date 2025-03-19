using System.Text;
using Rinku.Context;

namespace Rinku.ConditionalSelect;
public class SingleValCondCol<T>(string Name, string SQL) : SimpleCondCol<T>(Name) {
    public string SQL = SQL;
    public override void AppendSQL(StringBuilder sb, IContext _) => sb.Append(SQL);
}
