using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace APICatalogo.Models;


[Table("Categories")] // Redundante, porque ao definir o contexto DBSet no context já está mapeando a tabela
public class Category
{

    public Category()
    {
        Products = new Collection<Product>();
    }
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(80)]
    public string? Name { get; set; }

    [Required]
    [StringLength(300)]
    public string? ImageUrl { get; set; }

    [JsonIgnore]
    public ICollection<Product>? Products { get; set; }
}
