using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LibraryMS.Repositories.Interfaces
{
    public interface IRoleRepository
    {
        IEnumerable<IdentityRole> GetAllRoles();
        Task<IdentityResult> CreateRoleAsync(string roleName);
        Task<bool> RoleExistsAsync(string roleName);
    }
}