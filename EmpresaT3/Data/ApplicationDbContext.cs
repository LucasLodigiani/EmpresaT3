using EmpresaT3.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Data.SqlClient;

namespace EmpresaT3.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) :
            base(options)
        {

        }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<EmpresaT3.Models.Category> Category { get; set; }
        public DbSet<EmpresaT3.Models.Contacto> Contacto { get; set; }

    }
}
