using EmpresaT3.Data;
using System.ComponentModel.DataAnnotations;

namespace EmpresaT3.Models
{
    public class Logs
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(60)]
        [Display(Name = "Usario")]
        public string? Usuario { get; set; }

        [Required]
        [Display(Name = "Operacion")]
        [StringLength(10)]
        public string? Operacion { get; set; }

        [Required]
        [Display(Name = "Accion")]
        [StringLength(10)]
        public string? Accion { get; set; }

        [Required]
        [Display(Name = "Producto/Categoria")]
        public int? Producto { get; set; }

        [Required]
        [Display(Name = "Fecha")]
        public string? Fecha { get; set; }

        [Required]
        public string? IpAddress { get; set; }

        

    }


}
