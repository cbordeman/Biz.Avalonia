namespace Biz.Core;

public static class AppConstants
{
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
    
    // A custom Uri like "biz:path" used to invoke the
    // app's functionality when it's not running,
    // such as from an email link.  Allows the app
    // to be invoked from other apps, like a browser.
    const string WindowsRegistryCustomUriScheme = 
        AppConstants.AppInternalName;
    
    // This should not change, either, though the rules for what
    // it contains is not so strict.
    const string WindowsRegistryCustomUriProtocolFriendlyName = 
        $"{AppConstants.AppShortName} Protocol";
    
    public const string AppVersion = "0.0.1";
}
