using System.ComponentModel.DataAnnotations;

namespace APICatalogo.DTOS
{
    public class CategoryDTO
    {
        public int Id { get; set; }

        [Required]
        [StringLength(80)]
        public string? Name { get; set; }

        [Required]
        [StringLength(300)]
        public string? ImageUrl { get; set; }
    }
}
