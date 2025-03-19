using Rinku.Http;
using TestWeb;
App.SetDefaultValues();
var rinkuApp = App.MakeApp();

var builder = WebApplication.CreateBuilder(args);
builder.AddRinkuApp(rinkuApp);
var app = builder.Build();

app.UseRinkuApp(rinkuApp, ["/favicon.ico"]);
app.UseStaticFiles();
app.UseHttpsRedirection();
app.Run();