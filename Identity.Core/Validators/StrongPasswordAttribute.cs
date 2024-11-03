using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Core.Validators;
public class StrongPasswordAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value == null || !(value is string password))
        {
            return new ValidationResult("Password is required.");
        }

        if (IsPasswordStrong(password))
        {
            return ValidationResult.Success;
        }

        return new ValidationResult("Password must be at least 8 characters long and include an uppercase letter, a lowercase letter, a number, and a special character.");
    }
    private bool IsPasswordStrong(string password)
    {
        return password.Length >= 8 &&
               password.Any(char.IsUpper) &&
               password.Any(char.IsLower) &&
               password.Any(char.IsDigit) &&
               password.Any(ch => "!@#$%^&*()_+[]{}|;:,.<>?".Contains(ch));
    }
}
