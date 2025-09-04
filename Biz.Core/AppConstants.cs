namespace Biz.Core;

// ReSharper disable MemberCanBePrivate.Global

public static class AppConstants
{
    public const string AppVersion = "0.0.1";

    // This is what is used 99% of the time to refer to the product.
    // The shorter, the better.  You can change this as you want.
    // Mixed case, since the user will see this.
    public const string AppShortName = "Biz";
    
    // These can be the same as the short name, but are
    // sometimes slightly longer.  Used in legal contexts,
    // the About screen, and elsewhere.
    public const string AppLongName = "Biz";
    public const string AppCompany = "My Company, Inc.";
    
    // IMPORTANT NOTE: all "Internal" names must be very
    // short and lowercase, no spaces or punctuation.
    // They should NOT change after first deployment
    // or settings will be lost and things break.
    // They are used programatically for registrations,
    // manifests, and Windows Registry keys.
    
    public const string CompanyInternalName = "mycompany";
    public const string AppInternalName = "biz";
    
    // A custom Uri like "biz:path" allows the app
    // to be invoked from other apps, like a browser.
    // This is the prefix part before the colon.
    public const string CustomUriSchemeName = AppInternalName;
    
    // This should not change, either, though the rules for what
    // it contains is not as strict.
    public const string CustomUriProtocolFriendlyName = 
        $"{AppShortName} Protocol";

    public const string ConfirmEmailLinkFormat =
        $"{CustomUriSchemeName}:/confirm-email?email={{email}}&token={{token}}";
    
    public const string ResetPasswordLinkFormat =
        $"{CustomUriSchemeName}:/reset-password?email={{email}}&token={{token}}";
    
    public const string EmailRegex = @"^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$";
}
