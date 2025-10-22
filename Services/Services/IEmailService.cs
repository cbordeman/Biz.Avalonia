using Azure;
using Azure.Communication.Email;
using Biz.Core;
using Data.Models;
using Services.Config;
using Shouldly;

namespace Services.Services;

public interface IEmailService
{
    Task SendConfirmationEmailAsync(AppUser user, string emailToken);
    Task SendPasswordResetEmailAsync(AppUser user, string emailToken);
}

public class AzureEmailService(AzureSettings settings,
    ILogger<AzureEmailService> logger) : IEmailService
{
    public Task SendConfirmationEmailAsync(AppUser user, string emailToken)
    {
        // Construct reset link to send via email
        var link = string.Format(
            format: AppConstants.ConfirmEmailLinkFormat,
            user.Email!, user.Email);
        
        // TODO: insert links for email confirmation
        return SendEmailAsync(user.Email!,
            "Confirm your email address",
            $"Please click <a href\"{link}\">here</a> to confirm your email " +
            $"address and activate your {AppConstants.AppShortName} account!",
            $"Please go to {link} to confirm your email address and activate your Biz account!");
    }
    
    public Task SendPasswordResetEmailAsync(AppUser user, string emailToken)
    {
        // Construct reset link to send via email
        var link = string.Format(
            AppConstants.ResetPasswordLinkFormat,
            user.Email!, user.Email);

        return SendEmailAsync(user.Email!,
            "Password reset confirmation",
            $"Please click <a href=\"{link}\">here</a> to reset " +
            $"your {AppConstants.AppShortName} password.",
            $"Please go to {link} to reset your password.");
    }
    
    private async Task SendEmailAsync(string recipient,
        string subject, string htmlBody, string plainTextBody)
    {
        htmlBody.ShouldNotBeNull();
        plainTextBody.ShouldNotBeNull();

        var emailClient = new EmailClient(settings.ConnectionString);

        var emailMessage = new EmailMessage(
            senderAddress: settings.FromEmailAddress,
            content: new EmailContent(subject)
            {
                // For email clients that are configured to display
                // text only.
                PlainText = $"Biz\n\n{plainTextBody}",
                
                Html = $"""
<html>
    <body style="font-family: 'Arial'">
        <h1>Biz</h1>
        <p>{htmlBody}</p>
    </body>
</html>
"""
            },
            recipients: new EmailRecipients(new List<EmailAddress> { new(recipient) }));

        var emailSendOperation = await emailClient.SendAsync(WaitUntil.Completed, emailMessage);

        if (emailSendOperation.Value.Status == EmailSendStatus.Succeeded)
            logger.LogInformation("Email Sent: {Recipient}", recipient);
        else
        {
            logger.LogWarning("Failed to send email (Status = {Status}).  Recipient: {Recipient}", emailSendOperation.Value.Status, recipient);
            throw new Exception(@$"Failed to send email to recipient ""{recipient}.""");
        }
    }
}
