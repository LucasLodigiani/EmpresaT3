using EmpresaT3.Areas.Identity.Data;
using EmpresaT3.Core.Repositories;
using Microsoft.AspNetCore.Identity;

namespace EmpresaT3.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly ApplicationDbContext _context;

        public RoleRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public ICollection<IdentityRole> GetRoles()
        {
            return _context.Roles.ToList();
        }
    }
}
