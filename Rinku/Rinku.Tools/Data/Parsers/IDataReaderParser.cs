using System.Collections;
using System.Data;

namespace Rinku.Tools.Data.Parsers; 
public interface IDataReaderParser : IParserBase {
    public void Populate(IList list, IDataReader reader);
}
