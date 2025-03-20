using System.Data;
using ConditionalQueries;
using HTMLTemplating;
using Microsoft.Data.SqlClient;
using Rinku;
using Rinku.ConditionalSelect;
using Rinku.Context;
using Rinku.Controllers;
using Rinku.Http.Connection;
using Rinku.Http.ConnectionClasses;
using Rinku.Http.Controllers;
using Rinku.Http.Middlewares;
using Rinku.Middlewares;
using Rinku.Tools.Globalization;
using Rinku.Tools.Processes;
using Rinku.Tools.Queries;

namespace TestWeb;
public static class App {
    public const string CnnStr = "Data Source=localhost;Initial Catalog=Miliem;Persist Security Info=True;User ID=sa;Password=allo123; TrustServerCertificate=true;";
    public static void SetDefaultValues() {
        Query.QueryValueParsers = [
            QueryValueWord.TryParse,
            Query.QueryValueSubquery,
            Query.QueryValueSimple
        ];
        Word.DefaultCulture = C.EN;
    }
    public static IDataReader Getreader(string query) {
        var cnn = new SqlConnection(CnnStr);
        using var command = cnn.CreateCommand();
        command.CommandText = query;
        cnn.Open();
        return command.ExecuteReader(CommandBehavior.CloseConnection);
    }
    public static RinkuApp MakeApp() {
        var middlewares = GetMiddlewares().ToArray();
        var controller = GetRootController();
        var rinkuApp = new RinkuApp(controller, middlewares);
        return rinkuApp;
    }
    public static IEnumerable<IEndpointHandler<NoContext>> GetMiddlewares() {
        yield return new AuthenticationMiddleware<User>() {
            AuthScheme = "fewfa",
            LoginPath = "Login",
            LogoutPath = "Logout"
        };
        yield return new CultureMiddleware([
            C.EN,
            C.JA,
            C.FR,
            C.FR_CA,
            C.TR
        ]);
    }
    public static IEndpointHandler<NoContext> GetRootController() {
        var cred = new CredentialsPage("Main");
        var rootController = new HTMLRootController2<User>("App");
        rootController.Head.PrependChild((HTMLRaw)@"
            <meta charset=""utf-8"" />
            <link rel=""stylesheet"" href=""/test.css"">
            <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"" />
            <script src=""https://unpkg.com/htmx.org@2.0.4""></script>");
        var processController = new ProcessController("Login", "body",
            Seg.From(cred)
            .Then(new Token2FAPage())
            .Then(
                Seg.From(new RedirectAfterSuccess("/potato", "body")),
                Seg.CloseWith(cred)
            )
            .Build()
        );
        var contentController = new ContentController<User>("Potato", new() {
            [C.FR] = "Francais",
            [C.EN] = "English",
            [C.JA] = "nihongo",
            [C.FR_CA] = "Franquais",
            [C.INV] = "Lang"
        });
        var query = new ConditionalSelectQuery<JSONContext>(@"SELECT TOP(2) * FROM Boxes b 
INNER JOIN vwSources sr ON sr.BoxTypeID = b.BoxTypeID AND sr.SourceID = b.SourceID 
LEFT JOIN Stations st ON st.ID = b.StationID 
INNER JOIN BoxTypes bt ON bt.ID = b.BoxTypeID 
INNER JOIN Products pr ON pr.ID = b.ProductID 
LEFT JOIN Statuses s ON s.ID = b.StatusID 
LEFT JOIN StatusGroups sg ON sg.ID = s.StatusGroupID 
LEFT JOIN Users u ON u.ID = b.UserID ORDER BY b.ID", new CondColList<JSONContext>()
    .Add<int>("ID", "b.ID").WhenHasRoles([1])
.Add<bool>("Quarantine", "sr.Quarantine")
.Add<int>("SourceID", "b.SourceID")
.Add<int>("BoxTypeID", "b.BoxTypeID")
.Add<string>("BatchNB", "sr.Name")
.Add<int, string>("Product", "b.ProductID", "CONCAT(pr.Code, ':', pr.Name)")
.Add<string>("Type", "bt.Name")
//.Add<DateTime>("Date", "b.Date")
.Add<decimal>("Gross", "b.Gross")
.Add<decimal>("Tare", "b.Tare")
.Add<decimal>("Net", "(b.Gross - b.Tare)")
.AddNull<int, string>("User", "b.UserID", "CASE WHEN b.UserID IS NULL THEN NULL ELSE CONCAT(u.Firstname, ' ', u.Lastname) END")
.AddNull<int, string>("Status", "b.StatusID", "CASE WHEN b.StatusID IS NULL THEN NULL ELSE CONCAT(s.Name, ' (', sg.Name, ')') END")
.Add<string>("StatusColor", "sg.Color")
.Add<int, string>("Station", "b.StationID", "st.Name")


);
        var dynaController = new DynamicTable<JSONContext>("Test", query, Getreader);
        rootController
            .AddController(processController)
            .AddController(contentController);
        rootController.JSONControllers
            .Add(dynaController);
        return rootController;
    }
}
