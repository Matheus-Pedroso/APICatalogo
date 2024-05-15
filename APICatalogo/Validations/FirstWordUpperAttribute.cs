using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System.ComponentModel.DataAnnotations;

namespace APICatalogo.Validations
{
    public class FirstWordUpperAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null || string.IsNullOrEmpty(value.ToString())) return ValidationResult.Success;

            var firstWord = value.ToString()[0].ToString();
            if (firstWord != firstWord.ToUpper()) return new ValidationResult("The first word isn't to upper");

            return ValidationResult.Success;
        }
    }
}
