using System.ComponentModel.DataAnnotations;

namespace EmpresaT3.Models
{
    public class Contacto
    {

        public int Id { get; set; }
        [Required]
        [StringLength(20)]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Mensaje")]
        public string Mensaje { get; set; }

        [Required]
        public string Fecha { get; set; }

        [Required]
        public string IpAddress  { get; set; }

    }
}
