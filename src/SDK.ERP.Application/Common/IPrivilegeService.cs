namespace SDK.ERP.Application.Common;

public interface IPrivilegeService
{
    /// <summary>
    /// Checks whether the user has been granted the specified privilege.
    /// SuperAdmin users bypass all checks automatically.
    /// </summary>
    Task<bool> HasPrivilegeAsync(long userId, string privilegeCode);

    /// <summary>
    /// Gets all privilege codes granted to the user.
    /// </summary>
    Task<HashSet<string>> GetUserPrivilegesAsync(long userId);

    /// <summary>
    /// Replaces the privileges for a user with the specified list.
    /// </summary>
    Task SaveUserPrivilegesAsync(long targetUserId, IEnumerable<string> grantedCodes, long grantedByUserId);
}
