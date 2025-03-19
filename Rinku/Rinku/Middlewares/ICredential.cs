namespace Rinku.Middlewares;
public interface ICredential;
public interface ICredentialClaim<T, TClaim> : ICredential where T : ICredential {
    public abstract static T? FromClaim(TClaim claim);
    public TClaim MakeClaim(string authScheme);
}
