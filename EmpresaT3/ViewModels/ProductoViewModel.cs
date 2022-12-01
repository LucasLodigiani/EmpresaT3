using System;
using System.ComponentModel.DataAnnotations;
using EmpresaT3.Models;

namespace EmpresaT3.ViewModels
{
    public class ProductoViewModel : EditImageViewModel 
    {
        [Required]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; }

        [Required]
        [Display(Name = "Descripcion")]
        public string Descripcion { get; set; }

        [Required]
        public string Categoria { get; set; }

        [Required]
        public float Precio { get; set; }


    }
}
