using System.ComponentModel.DataAnnotations;

namespace API.DTOs.Validation
{
    public class CustomDateOfBirthValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is DateTime dataNascimento)
            {
                var idade = DateTime.Now.Year - dataNascimento.Year;
                if (dataNascimento > DateTime.Now.AddYears(-idade)) idade--;

                if (idade < 18)
                {
                    return new ValidationResult(ErrorMessage ?? "O entregador deve ter pelo menos 18 anos.");
                }
            }
            return ValidationResult.Success;
        }
    }
}
