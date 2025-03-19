namespace Rinku.Context;
public static class ContextExtension {
    public static string CredentialItemName { get; set; } = "Credential";
    public static object? GetCredential(this IContext ctx) {
        if (ctx is ICredentialContext ctxCred)
            return ctxCred.Credential;
        if (ctx.Items.TryGetValue(CredentialItemName, out var cred))
            return cred;
        return null;
    }
    public static void SetCredential(this IContext ctx, object? cred) {
        if (ctx is ICredentialContext ctxCred)
            ctxCred.Credential = cred;
        ctx.Items[CredentialItemName] = cred;
    }
}