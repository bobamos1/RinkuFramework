using Data;
using Data.ParserInterfaces;
using Data.Parsers;
using Rinku.Http;
using TestWeb;
IParser<WithDataReader, string>.Instance = new StringParser();
IParser<WithDataReader, int>.Instance = new IntParser();
IParser<WithDataReader, decimal>.Instance = new DecimalParser();
IParser<WithDataReader, bool>.Instance = new BoolParser();
IParser<WithDataReader, RefItem<int, string>>.Instance = new RefParser<int, string>();
App.SetDefaultValues();
var rinkuApp = App.MakeApp();

var builder = WebApplication.CreateBuilder(args);
builder.AddRinkuApp(rinkuApp);
var app = builder.Build();

app.UseRinkuApp(rinkuApp, ["/favicon.ico"]);
app.UseStaticFiles();
app.UseHttpsRedirection();
app.Run();