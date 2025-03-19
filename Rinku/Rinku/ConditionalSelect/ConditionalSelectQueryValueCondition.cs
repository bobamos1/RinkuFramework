using System.Data;
using Rinku.Context;
using Rinku.Tools.Data;

namespace Rinku.ConditionalSelect;
public interface IHasRoles<T> {
    public int ID { get; }
    public bool IsAuthorized(T[] rolesAuthorized);
}
public interface IConditionalSelectColumnCondition<in T> {
    public bool Valid(T ctx);
}

public class HasRoleCondition<T, TUnit>(TUnit[]? AuthorizedRoles) : IConditionalSelectColumnCondition<T> where T : IContext {
    public TUnit[]? AuthorizedRoles = AuthorizedRoles;
    public bool Valid(T ctx) {
        if (AuthorizedRoles is null)
            return true;
        var cred = ctx.GetCredential();
        if (cred is not IHasRoles<TUnit> credRoles)
            return false;
        return credRoles.IsAuthorized(AuthorizedRoles);
    }
}
