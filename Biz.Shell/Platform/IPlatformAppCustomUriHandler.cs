namespace Biz.Shell.Platform;

public interface IPlatformAppCustomUriHandler
{
    const string ShowUiForForgotPassword = "show-ui-for-forgot-password";
    const string ShowUiForRegisterUser = "show-ui-for-register-user";
    
    Task HandleUri(string uriString);
}
