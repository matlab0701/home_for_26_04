using Domain.Contains;
using Microsoft.AspNetCore.Identity;
namespace Infrastructure.Data.Seeder;

public class DefaultUser
{
       public static async Task SeedAsync(UserManager<IdentityUser> userManager)
    {
        var user = new IdentityUser
        {
            UserName = "abdunazarovmmatlabshoh@gmail.com",
            Email = "abdunazarovmmatlabshoh@gmail.com",
            EmailConfirmed = true,
            PhoneNumber = "400095619",
        };
        
        var existingUser = await userManager.FindByNameAsync(user.UserName);
        if (existingUser != null)
        {
            return;
        }

        var result = await userManager.CreateAsync(user, "12345");
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(user, Roles.Admin);
        }
    }
}
