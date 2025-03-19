using HTMLTemplating;
using Rinku.Controllers;

namespace Rinku.Http.Controllers;
public class HTMLRootController2<T> : RootControllerCred<T> where T : class {
    public HTMLRootController2(string AppName) : base(AppName) {
        Root.AppendChild(new HTMLElement("a", "click", [new Attr("href", "/potato")]));
    }
}