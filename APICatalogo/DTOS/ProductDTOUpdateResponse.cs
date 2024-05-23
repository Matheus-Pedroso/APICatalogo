using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace APICatalogo.DTOS;

public class ProductDTOUpdateResponse
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public string? ImageUrl { get; set; }
    public float Balance { get; set; }
    public DateTime DateRegister { get; set; }
    public int CategoryId { get; set; }
}
