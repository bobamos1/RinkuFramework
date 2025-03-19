using Microsoft.AspNetCore.Builder;

namespace Rinku.Http.Middlewares;
public interface INeedToUseWebApp {
    public void AddWebBuilder(WebApplicationBuilder builder);
    public void UseWebApp(WebApplication app);
}
