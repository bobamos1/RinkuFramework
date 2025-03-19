using HTMLTemplating;
using Rinku.Context;
using Rinku.Http.ConnectionClasses;

namespace Rinku.Http.Connection;
public class Token2FAPage(string? StepName = null, string idName = "ID", string tokenName = "Token")
    : HTMXStep2<Token2FAPage, int, int, Token2FACredentials>(StepName) {
    public override Token2FACredentials ExtractAnswer(IContext ctx) {
        var form = ctx.Ctx.Request.Form;
        if (!form.TryGetValue(idName, out var idTemp) || !int.TryParse(idTemp, out var id))
            return default;
        if (!form.TryGetValue(tokenName, out var tokenTemp))
            return default;
        return new Token2FACredentials(id!, tokenTemp!);
    }
    public override Task<HTMLItem> GenerateDisplay(IContext ctx, int id) => GenerateDisplay(ctx, id, true);
    public Task<HTMLItem> GenerateDisplay(IContext _, int id, bool needWriteToken) {
        if (needWriteToken)
            Token2FACredentials.WriteToken(id);
        HTMLRaw html = $@"
        <form hx-post=""{Endpoint}"" hx-target=""body"" hx-swap=""innerHTML"">
            {(needWriteToken ? null : "Error")}
            <input type=""hidden"" name=""{idName}"" value=""{id}"" />
            <input type=""password"" name=""{tokenName}"" placeholder=""Token"" />
            <button type=""submit"">Submit</button>
        </form>
        <a href=""{_processName}"">Cancel</a>";
        return Task.FromResult<HTMLItem>(html);
    }
    public override async Task<HTMLItem> ProcessAnswer(IContext displayInput, Token2FACredentials token) {
        if (token.Token == "10")
            return await GenerateDisplay(displayInput, 100);
        if (token.Token == "c")
            return await NextStep2.GenerateDisplay(displayInput, 100);
        if (!token.Validate())
            return await GenerateDisplay(displayInput, token.ID, false);
        return await NextStep1.GenerateDisplay(displayInput, token.ID);
    }
}