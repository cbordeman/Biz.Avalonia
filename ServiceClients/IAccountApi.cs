using Biz.Models;
using Biz.Models.Account;
using Refit;

namespace ServiceClients;

public interface IAccountApi
{
    const string Base = "Account";
    public const string GetMyUserInfoPath = $"/{Base}/users/myinfo";
    public const string LoginPath = $"/{Base}/login";

    [Get(GetMyUserInfoPath)]
    Task<User> GetMyUserInfo();

    [Post(LoginPath)]
    Task<string> Login([Body] LoginModel loginModel);
    
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