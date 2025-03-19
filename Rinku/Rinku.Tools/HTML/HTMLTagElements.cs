using HTMLTemplating;

namespace Rinku.Tools.HTML;
public static class Tag {
    public const string DIV = "div";
    public const string TBL = "table";
    public const string TR = "tr";
    public const string TD = "td";
    public const string FORM = "form";
    public const string INPT = "input";
    public const string BTN = "button";
}
public class HTMLDiv(string? Content = null, Attributes? Attributes = null)
    : HTMLElement<Attributes>(Tag.DIV, Content, Attributes);
public class HTMLDiv<T>(string? Content = null, T? Attributes = default)
    : HTMLElement<T>(Tag.DIV, Content, Attributes) where T : IAttributes;
public class HTMLTable(string? Content = null, Attributes? Attributes = null)
    : HTMLElement<Attributes>(Tag.TBL, Content, Attributes);
public class HTMLTable<T>(string? Content = null, T? Attributes = default)
    : HTMLElement<T>(Tag.TBL, Content, Attributes) where T : IAttributes;
public class HTMLTr(string? Content = null, Attributes? Attributes = null)
    : HTMLElement<Attributes>(Tag.TR, Content, Attributes);
public class HTMLTr<T>(string? Content = null, T? Attributes = default)
    : HTMLElement<T>(Tag.TR, Content, Attributes) where T : IAttributes;
public class HTMLTd(string? Content = null, Attributes? Attributes = null)
    : HTMLElement<Attributes>(Tag.TD, Content, Attributes);
public class HTMLTd<T>(string? Content = null, T? Attributes = default)
    : HTMLElement<T>(Tag.TD, Content, Attributes) where T : IAttributes;
public class HTMLForm(string? Content = null, Attributes? Attributes = null)
    : HTMLElement<Attributes>(Tag.FORM, Content, Attributes);
public class HTMLForm<T>(string? Content = null, T? Attributes = default)
    : HTMLElement<T>(Tag.FORM, Content, Attributes) where T : IAttributes;
public class HTMLInput(string? Content = null, Attributes? Attributes = null)
    : HTMLElement<Attributes>(Tag.INPT, Content, Attributes);
public class HTMLInput<T>(string? Content = null, T? Attributes = default)
    : HTMLElement<T>(Tag.INPT, Content, Attributes) where T : IAttributes;
public class HTMLButton(string? Content = null, Attributes? Attributes = null)
    : HTMLElement<Attributes>(Tag.BTN, Content, Attributes);
public class HTMLButton<T>(string? Content = null, T? Attributes = default)
    : HTMLElement<T>(Tag.BTN, Content, Attributes) where T : IAttributes;
