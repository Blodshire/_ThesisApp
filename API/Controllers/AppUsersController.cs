using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace API.Controllers
{
    //AllowAnonymous
    [Authorize]
    public class AppUsersController : BaseApiController
    {
        private readonly IAppUserRepository AppUserRepository;
        private readonly IMapper mapper;

        public AppUsersController(IAppUserRepository appUserRepository, IMapper mapper)
        {
            AppUserRepository = appUserRepository;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDTO>>> GetAppUsers()
        {
            //var users = await AppUserRepository.GetUsersAsync();
            //return Ok(mapper.Map<IEnumerable<MemberDTO>>(users));
            return Ok(await AppUserRepository.GetMembersAsync());
        }

        
        [HttpGet("{loginname}")]
        public async Task<ActionResult<MemberDTO>> GetAppUser(string loginname)
        {
            //var user= await AppUserRepository.GetUserByLoginNameAsync(loginname);
            //return mapper.Map<MemberDTO>(user);
            return await AppUserRepository.GetMemberByLoginNameAsync(loginname);

        }

        [HttpPut]
        public async Task<ActionResult> UpdateUser(MemberUpdateDTO memberUpdateDto)
        {
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value; //User.Identity.Name
            var appUser = await AppUserRepository.GetUserByLoginNameAsync(username);
            if (appUser == null)
                return NotFound();

            mapper.Map(memberUpdateDto, appUser);
            if(await AppUserRepository.SaveAllAsync())
                return NoContent();

            return BadRequest("Felhasználó frissítése sikertelen");
            
        }
    }
}
