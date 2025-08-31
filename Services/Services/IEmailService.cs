using Azure;
using Azure.Communication.Email;
using Data.Models;
using Services.Config;
using Shouldly;

namespace Services.Services;

public interface IEmailService
{
    Task SendConfirmationEmailAsync(AppUser user, string returnUrl);
}

public class AzureEmailService(AzureSettings settings,
    ILogger<AzureEmailService> logger) : IEmailService
{
    public Task SendConfirmationEmailAsync(AppUser user, string returnUrl)
    {
        return SendEmailAsync(user.Email!,
            "Confirm your email address",
            "Please click <a>here</a> to confirm your email address and activate your Biz account!",
            "Please go to confirm your email address and activate your Biz account!");
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
