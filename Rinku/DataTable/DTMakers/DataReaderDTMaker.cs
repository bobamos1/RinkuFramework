using System.Data;
using DataTable.SpecificParsers;

namespace DataTable.DTMakers;
public class DataReaderDTMaker : DTMaker<WithDataReader> {
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
}
