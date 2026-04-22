namespace Shared.Enums;

/// <summary>
/// Enum representing the approval status of a user account
/// </summary>
public enum UserStatus
{
    /// <summary>
    /// User account is pending approval by Admin
    /// </summary>
    Pending,

    /// <summary>
    /// User account has been approved and has full access
    /// </summary>
    Approved,

    /// <summary>
    /// User account has been rejected and access is denied
    /// </summary>
    Rejected
}
