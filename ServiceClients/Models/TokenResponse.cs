namespace ServiceClients.Models;

public class TokenResponse(
    TokenWithExpiry AccessToken,
    TokenWithExpiry RefreshToken);

public class TokenWithExpiry(string Token, DateTime Expiry);