using System;
using System.ComponentModel.DataAnnotations;

namespace EmpresaT3.ViewModels
{
    public class ProductoViewModel : EditImageViewModel 
    {
        [Required]
        [Display(Name = "Name")]
        public string Nombre { get; set; }

        [Required]
        [Display(Name = "Descripcion")]
        public string Descripcion { get; set; }

        [Required]
        public float Precio { get; set; }


    }
}
