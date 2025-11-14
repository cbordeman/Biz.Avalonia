using Biz.Core;
using Biz.Models;

namespace ServiceClients;

public record TokenAndProvider(string Token, LoginProvider LoginProvider);