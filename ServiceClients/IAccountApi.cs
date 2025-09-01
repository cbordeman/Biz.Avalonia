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
    public const string RegisterAccountPath = $"/{Base}/register--account";
    public const string ChangeUserEmailPath = $"/{Base}/change--email";
    public const string ConfirmEmailChangePath = $"/{Base}/confirm--email-change";
    public const string ChangePasswordPath = $"/{Base}/change--password";
    public const string ChangeNamePath = $"/{Base}/change--name";
    public const string ChangePhonePath = $"/{Base}/change--phone";
    public const string ForgotPasswordPath = $"/{Base}/forgot-password";
    public const string ResetPasswordPath = $"/{Base}/reset-password";
    
    [Get(GetMyUserInfoPath)]
    Task<User> GetMyUserInfo();

    [Post(LoginPath)]
    Task<TokenResponse> Login([Body] LoginModel loginModel);
    
    [Post(RefreshTokensPath)]
    Task<TokenResponse> RefreshTokens([Body] string refreshToken);

    [Post(RegisterAccountPath)]
    Task RegisterUser([Body] RegisterRequest model);
    
    [Post(ChangeUserEmailPath)]
    Task ChangeUserEmail([Body] ChangeEmailRequest model);

    [Post(ConfirmEmailChangePath)]
    Task ConfirmEmailChange(string userId, string email, string token);
    
    [Put(ChangePasswordPath)]
    Task ChangeUserPassword([Body] ChangePasswordRequest model);

    [Put(ChangeNamePath)]
    Task ChangeName([Body] ChangeNameRequest model);

    [Put(ChangePhonePath)]
    Task ChangePhone([Body] ChangePhoneRequest model);

    [Put(ForgotPasswordPath)]
    Task ForgotPassword(ForgotPasswordRequest model);

    [Put(ResetPasswordPath)]
    Task ResetPassword([Body] ResetPasswordRequest model);

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