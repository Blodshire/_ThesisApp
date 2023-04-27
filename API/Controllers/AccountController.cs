using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace API.Controllers
{

    public class AccountController : BaseApiController
    {
        private readonly DataContext _context;
        private readonly ITokenService _tokenService;

        public AccountController(DataContext context, ITokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }

        [HttpPost("register")] // POST, api/account/register
        public async Task<ActionResult<UserDTO>> Register(RegisterDTO registerDto)
        {
            if (await AlreadyExists(registerDto.UserName))
                return BadRequest("Felhasználónév használatban!");

            using var hmac = new HMACSHA512();

            var appUser = new AppUser
            {
                LoginName = registerDto.UserName.ToLower(),
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
                PasswordSalt = hmac.Key
            };

            _context.AppUsers.Add(appUser);

            await _context.SaveChangesAsync();
            return new UserDTO
            {
                LoginName = registerDto.UserName,
                Token = _tokenService.CreateToken(appUser)
            };
        }

        private async Task<bool> AlreadyExists(string username)
        {
            return await _context.AppUsers.AnyAsync(x => x.LoginName == username.ToLower());
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDTO>> Login(LoginDTO loginDto)
        {
            var appUser = await _context.AppUsers.FirstOrDefaultAsync(x => x.LoginName == loginDto.UserName);
            //consider SingleOrDefault
            if(appUser == null)
                return Unauthorized("Rosszul adta meg a jelszót!");

            using var hmac = new HMACSHA512(appUser.PasswordSalt);

            var reversedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

            for(int i = 0; i < reversedHash.Length; i++)
            {
                if (reversedHash[i] != appUser.PasswordHash[i])
                    return Unauthorized("Rosszul adta meg a jelszót!");
            }
            return new UserDTO
            {
                LoginName = loginDto.UserName,
                Token = _tokenService.CreateToken(appUser)
            };


        }
    }
}
