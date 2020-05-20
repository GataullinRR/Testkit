using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using UserServiceDb;
using Utilities.Extensions;
using Utilities.Types;

namespace UserService
{
    [Service(ServiceLifetime.Scoped, RegisterAsPolicy.Self)]
    class DbInitializer
    {
        public DbInitializer(RoleManager<IdentityRole> roleManager)
        {
            if (roleManager.Roles.Count() == 0)
            {
                foreach (var role in Roles.AllRoles)
                {
                    roleManager.CreateAsync(new IdentityRole(role)).Wait();
                }
            }
        }
    }
}
