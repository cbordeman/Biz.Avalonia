using Biz.Core.Models;

namespace ServiceClients;

public record TokenAndProvider(string Token, LoginProvider LoginProvider);