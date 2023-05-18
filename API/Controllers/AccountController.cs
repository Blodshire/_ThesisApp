using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace API.Controllers
{

    public class AccountController : BaseApiController
    {
        private readonly UserManager<AppUser> appUserManager;
        private readonly ITokenService _tokenService;
        private readonly IMapper mapper;

        public AccountController(UserManager<AppUser> appUserManager, ITokenService tokenService, IMapper mapper)
        {
            this.appUserManager = appUserManager;
            _tokenService = tokenService;
            this.mapper = mapper;
        }

        [HttpPost("register")] // POST, api/account/register
        public async Task<ActionResult<UserDTO>> Register(RegisterDTO registerDto)
        {
            if (await AlreadyExists(registerDto.LoginName))
                return BadRequest("Felhasználónév használatban!");

            var appUser = mapper.Map<AppUser>(registerDto);
            

            appUser.LoginName = registerDto.LoginName.ToLower();
           

            var result = await appUserManager.CreateAsync(appUser, registerDto.Password);

            if (!result.Succeeded)
                return BadRequest("Hiba regisztráció során");

            var roleResult = await appUserManager.AddToRoleAsync(appUser, "Member");

            if (!roleResult.Succeeded)
                return BadRequest("Hiba regisztráció során");

            return new UserDTO
            {
                LoginName = registerDto.LoginName,
                Token = await _tokenService.CreateToken(appUser),
                DisplayName = appUser.DisplayName,
                Gender= appUser.Gender,
            };
        }

        private async Task<bool> AlreadyExists(string username)
        {
            return await appUserManager.Users.AnyAsync(x => x.LoginName == username.ToLower());
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDTO>> Login(LoginDTO loginDto)
        {
            var appUser = await appUserManager.Users.Include(i=> i.Photos).FirstOrDefaultAsync(x => x.LoginName == loginDto.UserName);
            //consider SingleOrDefault
            if(appUser == null)
                return Unauthorized("Rosszul adta meg a jelszót!");

            var result = await appUserManager.CheckPasswordAsync(appUser, loginDto.Password);

            if(!result)
                return Unauthorized("Rosszul adta meg a jelszót!");


            return new UserDTO
            {
                LoginName = loginDto.UserName,
                Token = await _tokenService.CreateToken(appUser),
                PhotoUrl = appUser.Photos.FirstOrDefault(x=> x.isMain)?.url,
                DisplayName = appUser.DisplayName,
                Gender = appUser.Gender
            };


        }
    }
}
