using System.Data;
using Rinku.Tools.Data.Parsers;

namespace Rinku.Tools.Data.DTMakers;
public class DataReaderDTMaker : DTMaker<WithDataReaderDelegate, DataReaderParsersInfo> {
    public DataReaderDTMaker() {}
    public DataReaderDTMaker(int length) : base(length) {}
    public override DataReaderParsersInfo Parser => ParsersInfos.DataReaderParsersInfo;
    public DataTable GetDataTable(IDataReader data) {
        var dt = DataTable.New(this);
        try {
            while (data.Read())
                for (int i = 0; i < dt.Properties.Length; i++) 
                    this[i].Parser(dt.Properties[i].List, data);
        }
        finally {
            data.Dispose();
        }
        return dt;
    }
}
