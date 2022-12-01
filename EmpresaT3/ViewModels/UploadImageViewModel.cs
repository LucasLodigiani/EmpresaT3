using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace EmpresaT3.ViewModels
{
    public class UploadImageViewModel
    {
        [Display(Name = "Picture")]
        public IFormFile ProductPicture { get; set; }
    }
}
