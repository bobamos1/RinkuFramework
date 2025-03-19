using HTMLTemplating;
using Rinku.Context;
using Rinku.Http.ConnectionClasses;

namespace Rinku.Http.Connection;
public class CredentialsPage(string? StepName = null, string identifierName = "Username", string passwordName = "Password")
    : HTMXStep<CredentialsPage, string?, int, ConnectionCredentials>(StepName),
    IHTMXStartStep, IHTMXFrom<int> {
    public override ConnectionCredentials ExtractAnswer(IContext ctx) {
        var form = ctx.Ctx.Request.Form;
        if (!form.TryGetValue(identifierName, out var identifierTemp))
            return default;
        if (!form.TryGetValue(passwordName, out var passwordTemp))
            return default;
        return new ConnectionCredentials(identifierTemp!, passwordTemp!);
    }
    public Task<HTMLItem> GenerateDisplay(IContext displayInput) => GenerateDisplay(displayInput, null);
    public Task<HTMLItem> GenerateDisplay(IContext displayInput, int input) => GenerateDisplay(displayInput, input.ToString());
    public override Task<HTMLItem> GenerateDisplay(IContext ctx, string? identifier) {
        HTMLRaw html = $@"
        <form hx-post=""{Endpoint}"" hx-target=""body"" hx-swap=""innerHTML"">
            {(identifier is null ? null : "Error")}
            <input type=""text"" name=""{identifierName}"" value=""{identifier}"" placeholder=""Name"" />
            <input type=""password"" name=""{passwordName}"" placeholder=""Password"" />
            <button type=""submit"">Submit</button>
        </form>
        ";
        return Task.FromResult<HTMLItem>(html);
    }


    public override async Task<HTMLItem> ProcessAnswer(IContext displayInput, ConnectionCredentials creds) {
        if (!creds.Validate(out var id))
            return await GenerateDisplay(displayInput, creds.Username);
        return await NextStep.GenerateDisplay(displayInput, id);
    }
}