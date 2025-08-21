using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace RastroApi.Controllers;

// Use this as the base for all your controllers
public abstract class RastroControllerBase : ControllerBase
{
    /// <summary>
    /// Returns the user's email from claims, trying common claim types.
    /// </summary>
    protected string? UserEmail =>
        User.GetFirstClaimValue(ClaimTypes.Email) ??
        User.GetFirstClaimValue("email"); // JWT standard

    /// <summary>
    /// Returns the user's id (subject) from claims, trying common claim types.
    /// </summary>
    protected string? UserId =>
        User.GetFirstClaimValue(ClaimTypes.NameIdentifier) ??
        User.GetFirstClaimValue("sub"); // JWT standard

    /// <summary>
    /// Returns true if the token says the email is verified.
    /// </summary>
    protected bool EmailVerified =>
        (User.GetFirstClaimValue("email_verified") ?? "false")
            .Equals("true", StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Returns true when the user is authenticated AND we could resolve an email.
    /// </summary>
    protected bool IsUserAuthenticated =>
        (User?.Identity?.IsAuthenticated ?? false) &&
        !string.IsNullOrWhiteSpace(UserEmail);
}

// Small helper extensions to keep code tidy
public static class ClaimsPrincipalExtensions
{
    /// <summary>
    /// Gets the first claim value for a given type or null.
    /// </summary>
    public static string? GetFirstClaimValue(this ClaimsPrincipal principal, string claimType)
        => principal?.FindFirst(claimType)?.Value;
}
