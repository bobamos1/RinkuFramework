using ConditionalQueries;
using HTMLTemplating;
using Rinku.ConditionalSelect;
using Rinku.Context;
using Rinku.Controllers;
using Rinku.Tools.Globalization;

namespace Rinku.Http.Controllers;
public class ContentController<T> : IEndpointHandler<HTMLCredContext<T>>, IEndpointHandler<JSONContext> where T : class, IHasRoles<int> {
    public string Name;
    public readonly Query item;
    public readonly HTMLElement Div = new("div");
    public readonly HTMLElement Inner = new("div");
    public readonly HTMLElement Inner2 = new("div");
    public ContentController(string Name, Word word) {
        this.Name = Name;
        item = @"SELECT DISTINCT CustomerID, /** ja|JaCol & fr|FrCol */[{EnCol }], (SELECT MAX(Amount) FROM Orders WHERE CustomerID = Customers.CustomerID) AS MaxOrderAmount FROM Customers 
UNION ALL SELECT 123 FROM potato INNER JOIN o LEFT JOIN u WHERE EXISTS (SELECT 1 FROM Orders WHERE Orders.CustomerID = Customers.CustomerID AND Amount > @Amount)
UNION /*potato*/SELECT 432 FROM test INNER JOIN i
GROUP BY 1";
        Inner2.Content = "NoUser";
        Div.Content = word;
        Div.AppendChild(Inner);
        Div.AppendChild(Inner2);
    }
    public Task<bool> TryHandleEndpoint(HTMLCredContext<T> ctx) {
        if (!ctx.Nav.SameSegmentAs(Name))
            return Task.FromResult(false);
        ctx.SetRoot("body", Div);
        ctx.ChangeInner(Inner, item.Parse() ?? "");
        if (ctx.Credential is not null)
            ctx.ChangeInner(Inner2, ctx.Credential.ID.ToString());
        return ctx.SetHTML();
    }

    public async Task<bool> TryHandleEndpoint(JSONContext ctx) {
        if (!ctx.Nav.SameSegmentAs(Name))
            return false;
        await ctx.Ctx.WriteResponse("application/json", "1");
        return true;
    }
}
