using System;
using System.Text.Json;
using Biz.Core.Models;

namespace Biz.Core.Extensions;

public static class StringExtensions
{
    /// <summary>
    /// Deserializes from Json.
    /// </summary>
    /// <param name="str"></param>
    public static T? Deserialize<T>(this string str)
    {
        return JsonSerializer.Deserialize<T>(str);
    }
    
    public static bool IsValidEmail(this string email)
    {
        if (string.IsNullOrEmpty(email))
            return false;

        string[] parts = email.Split('@');
        if (parts.Length != 2)
            return false; // must have exactly one '@' symbol

        string localPart = parts[0];
        string domainPart = parts[1];

        if (string.IsNullOrWhiteSpace(localPart) || string.IsNullOrWhiteSpace(domainPart))
            return false; // local and domain parts cannot be empty

        // Check local part for valid characters: letters, digits, '.', '_', '-'
        foreach (char c in localPart)
            if (!char.IsLetterOrDigit(c) && c != '.' && c != '_' && c != '-')
                return false; // invalid character found

        // Basic domain checks: must contain '.', and not start or end with '.'
        if (domainPart.Length < 2 || !domainPart.Contains(".") ||
            domainPart.StartsWith(".") || domainPart.EndsWith("."))
            return false;

        // Optional: Check domain parts separated by '.' are not empty
        string[] domainParts = domainPart.Split('.');
        if (domainParts.Length < 2)
            return false;

        foreach (var dp in domainParts)
        {
            if (string.IsNullOrWhiteSpace(dp))
                return false; // empty domain label not allowed
        }

        return true;
    }
}