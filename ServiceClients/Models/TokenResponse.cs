namespace ServiceClients.Models;

public record TokenResponse(
    TokenWithExpiry AccessToken,
    TokenWithExpiry RefreshToken);