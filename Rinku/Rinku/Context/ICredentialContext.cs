namespace Rinku.Context;
public interface ICredentialContext: IContext {
    public object? Credential { get; set; }
}