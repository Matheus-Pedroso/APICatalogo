using System.ComponentModel.DataAnnotations;

namespace APICatalogo.DTOS;

public class ProductDTOUpdateRequest : IValidatableObject
{
    [Range(1,999, ErrorMessage = "Value greater than allowed")]
    public float Balance { get; set; }

    public DateTime DateRegister { get; set; }
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (DateRegister.Date <= DateTime.Now.Date)
        {
            yield return new ValidationResult("The date must be greater than the current date",
                new[] { nameof(this.DateRegister) }) ;
        }
    }
}
