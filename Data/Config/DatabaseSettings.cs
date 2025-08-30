namespace Data.Config;

public class DatabaseSettings
{
    public string Backend { get; set; } = string.Empty;
    public string MsSql { get; set; } = string.Empty;
    public string PostgreSql { get; set; } = string.Empty;
}

public class JwtIssuerSettings
{
    public string SecurityKey { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int AccessTokenExpirationMinutes { get; set; }
    public int RefreshTokenExpirationDays { get; set; }
}