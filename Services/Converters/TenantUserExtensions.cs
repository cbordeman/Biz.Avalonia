namespace Services.Converters;

public static class TenantUserExtensions
{
    /// <summary>
    /// Converts a TenantUser to a User model.
    /// </summary>
    /// <param name="tu">The TenantUser to convert.</param>
    /// <returns>An External User model with the Tenant and AppUser information.</returns>
    /// <exception cref="ArgumentNullException">Thrown if tu is null.</exception>
    public static User ConvertToExternalUser(this Data.Models.TenantUser tu)
    {
        return new User(
            tu.AppUserId, tu.AppUser.Name, tu.AppUser.Email!,
            tu.IsActive,
            tu.AppUser.LoginProvider,
            new Tenant(
                tu.TenantId,
                tu.Tenant.Name));
    }
}