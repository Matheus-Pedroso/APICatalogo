using APICatalogo.Validations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace APICatalogo.Models;

[Table("Product")] // Redundante, porque ao definir o contexto DBSet no context já está mapeando a tabela
public class Product : IValidatableObject

{
    [Key]
    public int Id { get; set; }

    [Required]
    //[StringLength(20, ErrorMessage="O nome deve ter entre 5 e 20 caracteres", MinimumLength = 5)]
    //[FirstWordUpperAttribute]
    public string? Name { get; set; }

    [Required]
    //[StringLength(10, ErrorMessage ="A descrição deve conter no máximo {1} caracteres")]
    public string? Description { get; set; }

    [Required]
    [Column(TypeName ="decimal(10,2)")]
    [Range(1,10000, ErrorMessage = "O preço deve estar entre {1} e {2}")]
    public decimal Price { get; set; }

    [Required]
    //[StringLength(300, MinimumLength = 10)]
    public string? ImageUrl { get; set; }

    public float Balance { get; set; }
    public DateTime DateRegister { get; set; }
    public int CategoryId { get; set; }

    [JsonIgnore]
    public Category? Category { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (!string.IsNullOrEmpty(Name))
        {
            var firstWord = Name[0].ToString();
            if (firstWord != firstWord.ToUpper()) 
            {
                yield return new ValidationResult("The first word isn't to upper", new[] { nameof(Name)}) ;
            }
        }

        if (Balance <= 0)
        {
            yield return new ValidationResult("Balance must be greater than zero", new[] { nameof(Balance)});
        }
    }
}
