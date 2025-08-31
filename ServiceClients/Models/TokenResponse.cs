namespace ServiceClients.Models;

public class TokenResponse(
    TokenWithExpiry AccessToken,
    TokenWithExpiry RefreshToken);