using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AdminController : BaseApiController
    {
        private readonly UserManager<AppUser> appUserManager;
        private readonly RoleManager<AppRole> roleManager;

        public AdminController(UserManager<AppUser> appUserManager, RoleManager<AppRole> roleManager)
        {
            this.appUserManager = appUserManager;
            this.roleManager = roleManager;
        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpGet("users-with-roles")]
        public async Task<ActionResult> GetUsersWithRoles()
        {
            var appUsers = await appUserManager.Users.OrderBy(x => x.LoginName)
                .Select(x => new
                {
                    x.Id,
                    //alternatively LoginName = x.LoginName
                    x.LoginName,
                    Roles = x.UserRoles.Select(role => role.Role.Name).ToList()
                }).ToListAsync();


            return Ok(appUsers);
        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpPut("edit-roles/{loginName}")]
        public async Task<ActionResult> EditRoles(string loginName, [FromQuery] string roles)
        {
            if (string.IsNullOrEmpty(roles))
                return BadRequest("Egy jogosultsági kört legalább ki kell választanod!");

            var selectedRoles = roles.Split(",");

            var appUser = await appUserManager.FindByNameAsync(loginName);
            if (User == null)
                return NotFound();

            var userRoles = await appUserManager.GetRolesAsync(appUser);
            var result = await appUserManager.AddToRolesAsync(appUser, selectedRoles.Except(userRoles));

            var adminRole = roleManager.Roles.Where(x => x.Name == "Admin").FirstOrDefault();

            var admins = appUserManager.Users.Where(x => x.UserRoles.Any(role => role.RoleId == adminRole.Id)).ToList();

            if(admins.Count() == 1 && appUser == admins.FirstOrDefault() && !roles.Contains("Admin"))
                return BadRequest("Az utolsó admin admin jogát nem veheted el!");


            if (!result.Succeeded)
                return BadRequest("Jogkörök hozzáadása sikertelen!");

            result = await appUserManager.RemoveFromRolesAsync(appUser, userRoles.Except(selectedRoles));

            if (!result.Succeeded) 
                return BadRequest("Jogkörök visszavonása sikertelen!");

            return Ok(await appUserManager.GetRolesAsync(appUser));
        }


        [Authorize(Policy = "ModeratePhotoRole")]
        [HttpGet("photos-to-moderate")]
        public ActionResult GetPhotosForModeration()
        {
            return Ok("Admin or moderator");
        }
    }
}
