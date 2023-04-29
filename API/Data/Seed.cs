using API.DTOs;
using API.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace API.Data
{
    public class Seed
    {
        public static async Task SeedUsers(DataContext context)
        {
            if (await context.AppUsers.AnyAsync())
                return;

            var appUserData = await File.ReadAllTextAsync("Data/UserSeedData.json");

            //this way we don't have to make sure that the casing in the seed json is 1-to-1 with our attribute names in our classes
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            var users = JsonSerializer.Deserialize<List<AppUser>>(appUserData);

            foreach (var user in users)
            {
                using var hmac = new HMACSHA512();

                user.LoginName = user.LoginName.ToLower();
                user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("Pa$$w0rd"));
                user.PasswordSalt = hmac.Key;

                context.AppUsers.Add(user);
            }

            //save resources by saving context after adding all users 
            await context.SaveChangesAsync();
        }
    }
}

//[
//  '{{repeat(5)}}',
//  {
//    LoginName: '{{firstName("male")}}',
//    Gender: 'male',
//    DateOfBirth: '{{date(new Date(1980,0,1), new Date(2003, 11, 31), "YYYY-MM-dd")}}',
//    UserName: function(){ return this.UserName; },
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
