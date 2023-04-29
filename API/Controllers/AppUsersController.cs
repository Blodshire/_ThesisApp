using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

    }
}
