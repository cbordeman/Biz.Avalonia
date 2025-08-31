using Biz.Models;
using Biz.Models.Account;
using Refit;
using ServiceClients.Models;

namespace ServiceClients;

public interface IAccountApi
{
    const string Base = "Account";
    public const string GetMyUserInfoPath = $"/{Base}/users/myinfo";
    public const string LoginPath = $"/{Base}/login";
    public const string RefreshTokensPath = $"/{Base}/refresh-tokens";
    public const string RegisterLocalAccountPath = $"/{Base}/register-local-account";
    public const string ChangeLocalUserEmailPath = $"/{Base}/change-local-email";
    public const string ConfirmLocalEmailChangePath = $"/{Base}/confirm-local-email-change";
    public const string ChangeLocalPasswordPath = $"/{Base}/change-local-password";
    public const string ChangeLocalNamePath = $"/{Base}/change-local-name";
    public const string ChangeLocalPhonePath = $"/{Base}/change-local-phone";
    
    [Get(GetMyUserInfoPath)]
    Task<User> GetMyUserInfo();

    [Post(LoginPath)]
    Task<TokenResponse> Login([Body] LoginModel loginModel);
    
    [Post(RefreshTokensPath)]
    Task<TokenResponse> RefreshTokens([Body] string refreshToken);

    [Post(RegisterLocalAccountPath)]
    Task RegisterLocalUser([Body] RegisterRequest model);
    
    [Post(ChangeLocalUserEmailPath)]
    Task ChangeLocalUserEmail([Body] ChangeEmailRequest model);

    [Post(ConfirmLocalEmailChangePath)]
    Task ConfirmLocalEmailChange(string userId, string email, string token);
    
    [Put(ChangeLocalPasswordPath)]
    Task ChangeLocalUserPassword([Body] ChangePasswordRequest model);

    [Put(ChangeLocalNamePath)]
    Task ChangeLocalName([Body] ChangeNameRequest model);

    [Put(ChangeLocalPhonePath)]
    Task ChangeLocalPhone([Body] ChangePhoneRequest model);
    
    //[Get("/posts/{id}")]
    //Task<Post> GetPostAsync(int id);

    //[Get("/posts")]
    //Task<List<Post>> GetPostsAsync();

    //[Post("/posts")]
    //Task<Post> CreatePostAsync([Body] Post post);

    //[Put("/posts/{id}")]
    //Task<Post> UpdatePostAsync(int id, [Body] Post post);

    //[Delete("/posts/{id}")]
    //Task DeletePostAsync(int id);
}