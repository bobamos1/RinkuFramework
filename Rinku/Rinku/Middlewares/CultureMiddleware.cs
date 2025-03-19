using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Rinku.Context;
using Rinku.Controllers;
using Rinku.Tools.StringExtensions;

namespace Rinku.Middlewares;
public class CultureMiddleware : IEndpointHandler<NoContext> {
    public const string AcceptLanguage = "Accept-Language";
    public HashSet<CultureInfo> SupportedCulturesFull { get; private set; } = [];
    public HashSet<CultureInfo> SupportedCulturesNeutral { get; private set; } = [];
    public CultureInfo DefaultCulture = null!;
    public CookieOptions CookieOptions;
    public void UpdateSupportedCultures(IEnumerable<CultureInfo> supportedCultures) {
        SupportedCulturesFull.Clear();
        SupportedCulturesNeutral.Clear();
        DefaultCulture = null!;
        foreach (CultureInfo culture in supportedCultures) {
            DefaultCulture ??= culture;
            (culture.IsNeutralCulture
                ? SupportedCulturesNeutral
                : SupportedCulturesFull)
                .Add(culture);
        }
    }
    public string CultureCookieName;
    public string UpdateCultureQueryParamName;
    public CultureMiddleware(
        IEnumerable<CultureInfo>? SupportedCultures = null, 
        string CultureCookieName = "user-lang", 
        string UpdateCultureQueryParamName = "update-culture",
        CookieOptions? CookieOptions = null) {
        UpdateSupportedCultures(SupportedCultures ?? [CultureInfo.InvariantCulture]);
        this.CultureCookieName = CultureCookieName;
        this.UpdateCultureQueryParamName = UpdateCultureQueryParamName;
        this.CookieOptions = CookieOptions ?? new CookieOptions() {
            Expires = DateTime.UtcNow.AddYears(1)
        };
    }
    public Task<bool> TryHandleEndpoint(NoContext ctx) {
        var culture = GetCulture(ctx);
        CultureInfo.CurrentCulture = culture;
        CultureInfo.CurrentUICulture = culture;
        return Task.FromResult(true);
    }
    public virtual void SetCulture(IContext ctx, CultureInfo culture)
        => ctx.Ctx.SetCulture(CultureCookieName, culture.Name, CookieOptions);
    public CultureInfo GetCulture(IContext ctx) {
        var culture = GetCulture(ctx, out var needToSet);
        if (needToSet)
            SetCulture(ctx, culture);
        return culture;
    }
    public CultureInfo GetCulture(IContext ctx, out bool needToSet) {
        if (TryGetCultureFromSegment(ctx, out var culture))
            needToSet = false;
        else if (TryGetCultureFromQueryParam(ctx, out culture))
            needToSet = true;
        else if (TryGetCultureFromCookie(ctx, out culture))
            needToSet = false;
        else if (TryGetCultureFromAcceptLanguage(ctx, out culture))
            needToSet = true;
        else {
            needToSet = true;
            culture = DefaultCulture;
        }
        return culture;
    }
    private bool TryGetCultureFromQueryParam(IContext ctx, [MaybeNullWhen(false)] out CultureInfo result)
        => TryGetCultureFromSegment(ctx.Ctx.GetQueryParam(UpdateCultureQueryParamName), out result);
    private bool TryGetCultureFromSegment(IContext ctx, [MaybeNullWhen(false)] out CultureInfo result) {
        if (TryGetCultureFromSegment(ctx.Nav.CurrentSegment, out result)) {
            ctx.Nav.NextSegment();
            return true;
        }
        return false;
    }
    private bool TryGetCultureFromSegment(string? segment, [MaybeNullWhen(false)] out CultureInfo result) {
        result = null;
        if (segment is null)
            return false;
        if (segment.Length != 2) {
            if (segment.Length < 5 || segment[2] != '-')
                return false;
            if (TryGetCulture(segment, out result))
                return true;
        }
        if (TryGetCulture2(segment, out result))
            return true;
        return false;
    }
    private bool TryGetCulture(string str, [MaybeNullWhen(false)] out CultureInfo result) {
        foreach (var culture in SupportedCulturesFull)
            if (culture.Name.Same(str)) {
                result = culture;
                return true;
            }
        result = null;
        return false;
    }
    private bool TryGetCulture2(string str, [MaybeNullWhen(false)] out CultureInfo result) {
        foreach (var culture in SupportedCulturesNeutral)
            if (culture.Name[0] == (str[0] | 32)
             && culture.Name[1] == (str[1] | 32)) {
                result = culture;
                return true;
            }
        result = null;
        return false;
    }
    private bool TryGetCultureFromCookie(IContext ctx, [MaybeNullWhen(false)] out CultureInfo culture) {
        var lang = ctx.Ctx.GetCookie(CultureCookieName);
        if (string.IsNullOrEmpty(lang)) {
            culture = null;
            return false;
        }
        try {
            culture = new CultureInfo(lang);
            return true;
        }
        catch {
            culture = null;
            return false;
        }
    }
    private bool TryGetCultureFromAcceptLanguage(IContext ctx, [MaybeNullWhen(false)] out CultureInfo cultureInfo) {
        var acceptLangs = ctx.Ctx.GetHeader(AcceptLanguage);
        if (string.IsNullOrEmpty(acceptLangs)) {
            cultureInfo = null;
            return false;
        }
        var langs = GetOrderedAcceptLang(acceptLangs);
        foreach (var lang in langs) {
            if ((lang.Length == 2 && TryGetCulture2(lang, out var culture))
             || (lang.Length >= 5 && TryGetCulture(lang, out culture))) {
                cultureInfo = culture;
                return true;
            }
        }
        cultureInfo = null;
        return false;
    }
    public static string[] GetOrderedAcceptLang(string acceptLangs) {
        var nb = 0;
        foreach (var c in acceptLangs)
            if (c == ',')
                nb++;
        if (nb == 0) {
            var i = acceptLangs.IndexOf(';');
            if (i == -1)
                return [acceptLangs];
            return [acceptLangs[..i]];
        }
        var res = new string[nb + 1];
        Span<int> qualities = stackalloc int[nb + 1];
        int startIndex = 0;
        while (startIndex < acceptLangs.Length) {
            int commaIndex = acceptLangs.IndexOf(',', startIndex);
            if (commaIndex == -1)
                commaIndex = acceptLangs.Length;

            int semicolonIndex = acceptLangs.IndexOf(';', startIndex);
            if (semicolonIndex == -1 || semicolonIndex > commaIndex)
                semicolonIndex = commaIndex;

            var cultureString = acceptLangs[startIndex..semicolonIndex];
            if (semicolonIndex == commaIndex
            || !TryGetQuality(acceptLangs, semicolonIndex, out var quality))
                quality = 10;
            Insert(ref res, ref qualities, cultureString, quality);
            startIndex = commaIndex + 1;
        }
        return res;
    }
    public static bool TryGetQuality(string s, int semicolonIndex, out int result) {
        result = s[semicolonIndex + 5] - '0';
        return result > 0 && result <= 9;
    }
    public static void Insert(ref string[] res, ref Span<int> qualities, string newString, int newQuality) {
        int position = 0;
        while (position < res.Length - 1) {
            var quality = qualities[position];
            if (quality < newQuality || quality == 0)
                break;
            if (quality == newQuality && res[position].Length < newString.Length)
                break;
            position++;
        }
        for (int i = res.Length - 1; i > position; i--) {
            res[i] = res[i - 1];
            qualities[i] = qualities[i - 1];
        }
        res[position] = newString;
        qualities[position] = newQuality;
    }
}
