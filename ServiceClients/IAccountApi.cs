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
    public const string RefreshAccessTokenPath = $"/{Base}/refresh-access-token";

    [Get(GetMyUserInfoPath)]
    Task<User> GetMyUserInfo();

    [Post(LoginPath)]
    Task<LoginResponse> Login([Body] LoginModel loginModel);
    
    [Post(RefreshAccessTokenPath)]
    Task<string> RefreshAccessToken([Body] string refreshToken);
    
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