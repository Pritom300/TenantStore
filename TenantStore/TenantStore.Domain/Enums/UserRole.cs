namespace TenantStore.Domain.Enums;

public enum UserRole
{
    SuperAdmin = 0,  // NEW: Cross-tenant access
    Admin = 1,
    User = 2
}