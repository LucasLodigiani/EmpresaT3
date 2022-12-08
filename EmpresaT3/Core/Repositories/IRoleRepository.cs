using EmpresaT3.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;

namespace EmpresaT3.Core.Repositories
{
    public interface IRoleRepository
    {
        ICollection<IdentityRole> GetRoles();
    }
}
