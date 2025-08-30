namespace ServiceClients.Models;

public class LoginResponse(
    TokenWithExpiry AccessToken,
    TokenWithExpiry RefreshToken);

public class TokenWithExpiry(string Token, DateTime Expiry);