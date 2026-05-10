using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace HealthZone.Data
{
    public static class SeedData
    {
        public static async Task SeedRolesAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider
                .GetRequiredService<RoleManager<IdentityRole>>();

            string[] roles = { "Administrator", "Doktor", "MedicinskaSestra", "Pacijent" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }
    }
}