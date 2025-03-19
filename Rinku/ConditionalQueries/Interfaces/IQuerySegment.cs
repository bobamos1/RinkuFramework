using System.Text;

namespace ConditionalQueries.Interfaces; 
public interface IQuerySegment {
    public bool Parse(StringBuilder sb, HashSet<string> conds);
}
