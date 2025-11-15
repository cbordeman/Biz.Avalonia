using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text.RegularExpressions;
using Core.Extensions;

namespace Core.DataAnnotations;

public class EmailAddressCustomAttribute : ValidationAttribute
{
    public EmailAddressCustomAttribute()
        : base(() => "Please enter a valid email address.")
    { }

    public override bool IsValid(object? value)
    {
        // Convert the value to a string
        string? val = Convert.ToString(value, CultureInfo.CurrentCulture);

        // Automatically pass if value is null or empty.
        // RequiredAttribute should be used to assert a value is not empty.
        if (string.IsNullOrEmpty(val))
            return true;

        return val.IsValidEmail();
    }
}


