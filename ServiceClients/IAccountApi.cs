using Biz.Models;
using Refit;

namespace ServiceClients;

public interface IAccountApi
{
    const string Base = "Account";
    public const string GetMyUserInfoPath = $"/{Base}/users/myinfo";

    [Get(GetMyUserInfoPath)]
    Task<User> GetMyUserInfo();

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