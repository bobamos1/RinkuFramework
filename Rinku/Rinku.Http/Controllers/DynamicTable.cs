using System.Data;
using DataTable.DTMakers;
using Rinku.ConditionalSelect;
using Rinku.Context;
using Rinku.Controllers;

namespace Rinku.Http.Controllers;
public class DynamicTable<T>(string Name, ConditionalSelectQuery<T> Query, Func<string, IDataReader> Reader) : IEndpointHandler<T> 
    where T : IContext {
    public string Name = Name;
    public ConditionalSelectQuery<T> Query = Query;
    public Func<string, IDataReader> Reader { get; } = Reader;

    public async Task<bool> TryHandleEndpoint(T ctx) {
        if (!ctx.Nav.SameSegmentAs(Name))
            return false;
        var dtMaker = new DataReaderDTMaker();
        var res = Query.Parse(ctx, [], dtMaker);
        if (res is null)
            return false;
        using var reader = Reader(res);
        var dt = dtMaker.GetDataTable(reader);
        await ctx.Ctx.WriteResponse("application/json", dt.ToJSON());
        return true;
    }
}
