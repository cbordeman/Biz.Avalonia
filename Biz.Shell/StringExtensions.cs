namespace Biz.Shell
{
    public static class StringExtensions
    {
        public static string GetInternalId(this string socialId, LoginProvider provider)
        {
            switch (provider)
            {
                case LoginProvider.Google:
                    return $"G-{socialId}";
                case LoginProvider.Microsoft:
                    return $"M-{socialId}";
                case LoginProvider.Facebook:
                    return $"M-{socialId}";
                case LoginProvider.Apple:
                    return $"A-{socialId}";
                default:
                    throw new ArgumentOutOfRangeException(nameof(provider), provider, null);
            }
        }
    }
}
