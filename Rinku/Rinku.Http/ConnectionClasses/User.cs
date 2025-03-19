using System.Security.Claims;
using Rinku.ConditionalSelect;
using Rinku.Middlewares;
using Rinku.Tools.StringExtensions;

namespace Rinku.Http.ConnectionClasses;
public record class User(int ID, int[]? Roles) : ICredentialClaim<User, ClaimsIdentity>, IHasRoles<int> {
    public virtual bool IsValid() => ID != default && Roles != default;
    public Claim UserIDClaim => new(nameof(ID), ID.ToString());
    public Claim RolesClaim => new(nameof(Roles), Roles?.JoinRoles() ?? string.Empty);
    public static User? FromClaim(ClaimsIdentity claim) {
        var id = 0;
        int[]? roles = null;
        foreach (var item in claim.Claims)
            switch (item.Type) {
                case nameof(ID):
                    id = int.Parse(item.Value);
                    break;
                case nameof(Roles):
                    roles = item.Value.SplitRoles();
                    break;
            }
        if (id == 0)
            return null;
        return new User(id, roles);
    }
    public ClaimsIdentity MakeClaim(string authScheme) => new([
        UserIDClaim, RolesClaim
    ], authScheme);

    public bool IsAuthorized(int[] rolesAuthorized) => Roles is not null && rolesAuthorized.Any(r => Roles.Contains(r));
}
