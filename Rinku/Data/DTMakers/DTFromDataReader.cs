using System.Data;
using Data.ParserInterfaces;

namespace Data.DTMakers; 
public class DTFromDataReader : DTMaker<WithDataReader> {
    public int CurrentCol = 0;
    public DataTable GetDataTable(IDataReader data) {
        try {
            while (data.Read())
                foreach (var parser in this)
                    parser(data);
        }
        finally {
            data.Dispose();
        }
        return DT;
    }
    public override WithDataReader? GetDel<T>(IParser<WithDataReader, T> parser, List<T?> list) where T : default {
        if (parser is not IDataReaderParser<T> dtrParser)
            return null;
        var i = CurrentCol;
        CurrentCol += dtrParser.NbColUsed;
        return r => list.Add(dtrParser.WithDataReader(r, i));
    }
}
