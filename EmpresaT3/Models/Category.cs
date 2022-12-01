using System.ComponentModel.DataAnnotations;

namespace EmpresaT3.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Categorias { get; set; }
    }
}
