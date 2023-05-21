using API.DTOs;
using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace API.Data
{
    public class Seed
    {
        public static async Task ClearConnections(DataContext context)
        {
            context.Connections.RemoveRange(context.Connections);
            await context.SaveChangesAsync();
        }
        public static async Task SeedUsers(UserManager<AppUser> appUserManager, RoleManager<AppRole> roleManager)
        {
            if (await appUserManager.Users.AnyAsync())
                return;

            var appUserData = await File.ReadAllTextAsync("Data/UserSeedData.json");

            //this way we don't have to make sure that the casing in the seed json is 1-to-1 with our attribute names in our classes
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            var users = JsonSerializer.Deserialize<List<AppUser>>(appUserData);

            var roles = new List<AppRole>
            {
                new AppRole {Name ="Member"},
                new AppRole {Name ="Admin"},
                new AppRole {Name ="Moderator"}

            };

            foreach(var role in roles)
            {
                await roleManager.CreateAsync(role);
            }

            foreach (var user in users)
            {

                user.LoginName = user.LoginName.ToLower();

                user.Created = DateTime.SpecifyKind(user.Created, DateTimeKind.Utc);
                user.LastActive = DateTime.SpecifyKind(user.Created, DateTimeKind.Utc);

                await appUserManager.CreateAsync(user, "Pa$$w0rd");
                await appUserManager.AddToRoleAsync(user, "Member");
            }

            var admin = new AppUser
            {
                LoginName = "admin"
            };

            await appUserManager.CreateAsync(admin, "Pa$$w0rd");
            await appUserManager.AddToRolesAsync(admin, new[] { "Admin", "Moderator" });

            //save resources by saving context after adding all users 
            //await context.SaveChangesAsync(); --before identity
        }
    }
}

//[
//  '{{repeat(5)}}',
//  {
//    LoginName: '{{firstName("male")}}',
//    Gender: 'male',
//    DateOfBirth: '{{date(new Date(1980,0,1), new Date(2003, 11, 31), "YYYY-MM-dd")}}',
//    UserName: function(){ return this.LoginName; },
//    Created: '{{date(new Date(2022, 0, 1), new Date(2023,4,30), "YYYY-MM-dd")}}',
//    LastActive: '{{date(new Date(2023, 4, 1), new Date(2020,4,30), "YYYY-MM-dd")}}',
//    Introduction: '{{lorem(1, "paragraphs")}}',
//    LookingFor: '{{lorem(1, "paragraphs")}}',
//    Interests: '{{lorem(1, "sentences")}}',
//    City: '{{city()}}',
//    Country: '{{country()}}',
//    Photos:
//[
//        {
//url: function(num) {
//        return 'https://randomuser.me/api/portraits/men/' + num.integer(1, 99) + '.jpg';
//    },
//        isMain: true
//      }
//    ]
//  }
//]
