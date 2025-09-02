using System.Text;
using Biz.Core.Extensions;
using Biz.Models;
using Biz.Models.Account;
using Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.WebUtilities;
using ServiceClients.Models;
using Services.Auth.Jwt;
using Services.Converters;
using Services.Services;
using Shouldly;

// ReSharper disable UnusedParameter.Global

namespace Services.Controllers;

// TODO: Restrict most of these endpoints to the BizAdmin,
// TenantAdmin, or the authenticated user.

[Route("[controller]")]
[ApiController]
[Authorize]
public class AccountController(UserManager<AppUser> userManager,
    JwtTokenIssuer jwtTokService,
    IDbContextFactory<AppDbContext> dbContextFactory,
    IEmailService emailService,
    ILogger<AccountController> logger)
    : ControllerBase, IAccountApi
{
    [HttpPost(IAccountApi.LoginPath)]
    [AllowAnonymous]
    public async Task<TokenResponse> Login([FromBody] LoginModel model)
    {
        var user = await userManager.FindByNameAsync(model.Username);
        if (user == null || !await userManager.CheckPasswordAsync(user, model.Password))
            throw new UnauthorizedAccessException(
                "User or password is incorrect.");
        var accessToken = jwtTokService.GenerateAccessToken(user);
        var dbContext = dbContextFactory.CreateDbContext();
        var refreshToken = await jwtTokService.GenerateAndSwapRefreshToken(
            user, dbContext);

        return new TokenResponse(accessToken, refreshToken);
    }

    [HttpPost(IAccountApi.RefreshTokensPath)]
    [AllowAnonymous]
    public async Task<TokenResponse> RefreshTokens(
        [FromBody] string requestRefreshToken)
    {
        var user = await userManager.GetUserAsync(HttpContext.User);
        user.ShouldNotBeNull();

        // Find the refresh token record
        var dbContext = dbContextFactory.CreateDbContext();
        var storedRefreshToken = await dbContext.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == requestRefreshToken &&
                                       rt.UserId == user.Id);

        if (storedRefreshToken == null || //storedToken.IsRevoked || 
            storedRefreshToken.ExpiryDate < DateTime.UtcNow)
            throw new UnauthorizedAccessException("Invalid or expired refresh token");

        if (storedRefreshToken.UserId != user.Id)
            throw new UnauthorizedAccessException("Refresh token belongs to another user.");

        // Generate new tokens while removing old refresh token.
        var newAccessToken = jwtTokService.GenerateAccessToken(user);
        var newRefreshToken = await jwtTokService.GenerateAndSwapRefreshToken(
            user, dbContext, storedRefreshToken);

        return new TokenResponse(newAccessToken, newRefreshToken);
    }

    [HttpPost(IAccountApi.RegisterAccountPath)]
    [AllowAnonymous]
    public async Task RegisterUser([FromBody] RegisterRequest model)
    {
        if (!ModelState.IsValid)
            throw new HttpNotFoundObjectException("Invalid model state.");
        if (await userManager.FindByNameAsync(model.Email!) != null)
            return;

        var user = new AppUser
        {
            Id = Guid.NewGuid().ToString(),
            Email = model.Email,
            // Username and Email are the same,
            // but could easily add UserName to the RegisterModel.
            Name = model.Name!,
            PhoneNumber = model.PhoneNumber,
            Extension = model.Extension,
            UserName = model.Email
        };

        var result = await userManager.CreateAsync(user, model.Password!);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
                ModelState.AddModelError(error.Code, error.Description);
            throw new HttpNotFoundObjectException(
                "Failed to register user.  Errors: " +
                string.Join(", ", result.Errors));
        }

        var emailToken = await userManager.GenerateChangeEmailTokenAsync(
            user, model.Email!);

        await emailService.SendConfirmationEmailAsync(user, emailToken);
    }

    [HttpPost(IAccountApi.ConfirmRegisteredEmailPath)]
    [AllowAnonymous]
    public async Task ConfirmRegisteredEmail(string email, string token)
    {
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(token))
            throw new BadHttpRequestException(ModelState.Serialize());

        var user = await userManager.FindByEmailAsync(email);
        if (user == null)
            return;

        // The token from the query string is usually Base64 URL encoded, so decode it
        var tokenBytes = WebEncoders.Base64UrlDecode(token);
        var decodedToken = Encoding.UTF8.GetString(tokenBytes);

        var result = await userManager.ChangeEmailAsync(user, email, decodedToken);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);
            logger.LogWarning(
                "1 Failed email confirmation {Email}: {Model}.", 
                email, ModelState.Serialize());
            return;
        }

        // Optionally update username to match new email (if your app uses
        // email as username).  Can comment this code (and fix the Register
        // endpoint to set username from the model) if you don't want
        // this behavior.  Also, the model would have to be extended
        // to include the Username.
        user.UserName = email;
        var updateResult = await userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
        {
            foreach (var error in updateResult.Errors)
                ModelState.AddModelError(string.Empty, error.Description);
            logger.LogWarning(
                "2 Failed email confirmation {Email}: {Model}.", 
                email, ModelState.Serialize());

        }
    }
    
    [HttpPost(IAccountApi.ChangeUserEmailPath)]
    public async Task ChangeUserEmail([FromBody] ChangeEmailRequest model)
    {
        if (!ModelState.IsValid)
            throw new BadHttpRequestException(ModelState.Serialize());

        var user = await userManager.FindByIdAsync(model.UserId!);
        if (user == null)
            throw new HttpNotFoundObjectException("User not found.");

        // ChangeEmailAsync sets the new email but does not update
        // user.EmailConfirmed automatically.
        var token = await userManager.GenerateChangeEmailTokenAsync(user, model.NewEmail!);
        var result = await userManager.ChangeEmailAsync(user, model.NewEmail!, token);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);
            logger.LogWarning(
                "3 Failed change email {ModelUserId} {ModelNewEmail}: {Model}.", 
                model.UserId, model.NewEmail, ModelState.Serialize());
        }
    }

    [HttpPost(IAccountApi.ConfirmEmailChangePath)]
    [AllowAnonymous]
    public async Task ConfirmEmailChange(string userId, string email, string token)
    {
        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(token))
            throw new BadHttpRequestException(
                $"Invalid email confirmation request.  " +
                $"userId: {userId}, email: {email}, token: {token}.");

        var user = await userManager.FindByIdAsync(userId);
        if (user == null)
            throw new HttpNotFoundObjectException("User not found.");

        // The token from the query string is usually Base64 URL encoded, so decode it
        var tokenBytes = WebEncoders.Base64UrlDecode(token);
        var decodedToken = Encoding.UTF8.GetString(tokenBytes);

        var result = await userManager.ChangeEmailAsync(user, email, decodedToken);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);
            logger.LogWarning(
                "4 Failed change email {UserId} {NewEmail}: {Model}.", 
                userId, email, ModelState.Serialize());
            return;
        }

        // Optionally update username to match new email (if your app uses
        // email as username).  Can comment this code and similar code
        // elsewhere if you don't want this behavior.  Also, some
        // models would have to be extended to include the UserName.
        user.UserName = email;
        var updateResult = await userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
        {
            foreach (var error in updateResult.Errors)
                ModelState.AddModelError(string.Empty, error.Description);
            logger.LogWarning(
                "5 Failed change email {UserId} {Email}: {Model}.", 
                userId, email, ModelState.Serialize());
        }
    }

    // For when the user knows their password, called
    // from an Account Profile page.
    [HttpPut(IAccountApi.ChangePasswordPath)]
    public async Task ChangeUserPassword([FromBody] ChangePasswordRequest model)
    {
        if (!ModelState.IsValid)
            throw new BadHttpRequestException(ModelState.Serialize());

        var user = await userManager.FindByIdAsync(model.UserId!);
        if (user == null)
            throw new HttpNotFoundObjectException("User not found.");

        var result = await userManager.ChangePasswordAsync(
            user, model.CurrentPassword!, model.NewPassword!);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);
            throw new BadHttpRequestException(ModelState.Serialize());
        } 
    }

    [HttpPut(IAccountApi.ChangeNamePath)]
    public async Task ChangeName([FromBody] ChangeNameRequest model)
    {
        if (!ModelState.IsValid)
            throw new BadHttpRequestException(ModelState.Serialize());

        var user = await userManager.FindByIdAsync(model.UserId!);
        if (user == null)
            throw new HttpNotFoundObjectException("User not found.");

        user.Name = model.NewName!;
        await userManager.UpdateAsync(user);
    }

    [HttpPut(IAccountApi.ChangePhonePath)]
    public async Task ChangePhone([FromBody] ChangePhoneRequest model)
    {
        if (!ModelState.IsValid)
            throw new BadHttpRequestException(ModelState.Serialize());

        var user = await userManager.FindByIdAsync(model.UserId!);
        if (user == null)
            throw new HttpNotFoundObjectException("User not found.");

        user.PhoneNumber = model.NewPhoneNumber!;
        user.Email = model.NewExtension!;
        await userManager.UpdateAsync(user);
    }

    // Called to send as password reset email when the
    // user has forgotten their password.
    [HttpPut(IAccountApi.ForgotPasswordPath)]
    [AllowAnonymous]
    public async Task ForgotPassword([FromBody] ForgotPasswordRequest model)
    {
        if (!ModelState.IsValid)
            throw new BadHttpRequestException(ModelState.Serialize());

        var user = await userManager.FindByEmailAsync(model.Email!);
        if (user == null || !await userManager.IsEmailConfirmedAsync(user))
        {
            // To prevent email enumeration, treat non-existent user same as valid
            // and return success.
            return;
        }

        var token = await userManager.GeneratePasswordResetTokenAsync(user);
        var encodedToken = WebEncoders.Base64UrlEncode(
            Encoding.UTF8.GetBytes(token));

        // Send the password reset email
        await emailService.SendPasswordResetEmailAsync(
            user, encodedToken);
    }

    // This one is linked to in the password reset email.
    [HttpPut(IAccountApi.ResetPasswordPath)]
    [AllowAnonymous]
    public async Task ResetPassword([FromBody] ResetPasswordRequest model)
    {
        if (!ModelState.IsValid)
            throw new BadHttpRequestException(ModelState.Serialize());

        var user = await userManager.FindByEmailAsync(model.Email!);
        // To prevent email enumeration, treat non-existent user same as valid
        // and return success.
        if (user == null)
            return;

        var tokenBytes = WebEncoders.Base64UrlDecode(model.Token!);
        var decodedToken = Encoding.UTF8.GetString(tokenBytes);

        var result = await userManager.ResetPasswordAsync(user, decodedToken, model.Password!);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);
            throw new BadHttpRequestException(ModelState.Serialize());
        }
    }

    [HttpGet(IAccountApi.GetMyUserInfoPath)]
    public Task<User> GetMyUserInfo()
    {
        var ctx = dbContextFactory.CreateDbContext();
        var user = User.VerifyTenantUserIsActive(ctx);

        return Task.FromResult(user.ConvertToExternalUser());
    }

    // [HttpGet("{id}")]
    // public string Get(int id)
    // {
    //     return "value";
    // }
    //
    // [HttpPost]
    // public void Post([FromBody] string value)
    // {
    // }
    //
    // [HttpPut("{id}")]
    // public void Put(int id, [FromBody] string value)
    // {
    // }
    //
    // [HttpDelete("{id}")]
    // public void Delete(int id)
    // {
    // }
    //
    // [HttpGet("claims")]
    // public IActionResult GetUserClaims()
    // {
    //     var claims = User.Claims.Select(c => new { c.Type, c.Value });
    //     return Ok(claims);
    // }
}