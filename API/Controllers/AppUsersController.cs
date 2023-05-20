using API.Data;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Helpers.PaginationHelperParams;
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
        private readonly IUnitOfWork uow;
        private readonly IMapper mapper;
        private readonly IPhotoService photoService;

        public AppUsersController(IUnitOfWork uow, IMapper mapper, IPhotoService photoService)
        {
            this.uow = uow;
            this.mapper = mapper;
            this.photoService = photoService;
        }
        //[Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDTO>>> GetAppUsers([FromQuery]UserParams userParams)
        {
            var currentUser = await uow.appUserRepository.GetUserByLoginNameAsync(User.GetLoginName());
            userParams.CurrentLogin=currentUser.LoginName;

            if (string.IsNullOrEmpty(userParams.Gender))
            {
                userParams.Gender = currentUser.Gender == "male" ? "female" : "male";
            }
            //var users = await AppUserRepository.GetUsersAsync();
            //return Ok(mapper.Map<IEnumerable<MemberDTO>>(users));
            var returnUsers = await uow.appUserRepository.GetMembersAsync(userParams);
            Response.AddPaginationHeader(
                new PaginationHeader(returnUsers.CurrentPage, returnUsers.PageSize
                                , returnUsers.TotalCount, returnUsers.TotalPages));
            return Ok(returnUsers);
        }

        //[Authorize(Roles = "Member")]
        [HttpGet("{loginname}")]
        public async Task<ActionResult<MemberDTO>> GetAppUser(string loginname)
        {
            //var user= await AppUserRepository.GetUserByLoginNameAsync(loginname);
            //return mapper.Map<MemberDTO>(user);
            return await uow.appUserRepository.GetMemberByLoginNameAsync(loginname);

        }

        [HttpPut]
        public async Task<ActionResult> UpdateUser(MemberUpdateDTO memberUpdateDto)
        {
            var username = User.GetLoginName(); //User.Identity.Name
            var appUser = await uow.appUserRepository.GetUserByLoginNameAsync(username);
            if (appUser == null)
                return NotFound();

            mapper.Map(memberUpdateDto, appUser);
            if (await uow.Complete())
                return NoContent();

            return BadRequest("Felhasználó frissítése sikertelen");

        }

        [HttpPost("add-photo")]
        public async Task<ActionResult<PhotoDTO>> AddPhoto(IFormFile file)
        {
            var appUser = await uow.appUserRepository.GetUserByLoginNameAsync(User.GetLoginName());
            if (appUser == null)
                return NotFound();
            var result = await photoService.AddPhotoAsync(file);

            if (result.Error != null)
                return BadRequest(result.Error);

            var photo = new Photo
            {
                url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId
            };

            if (appUser.Photos.Count == 0)
                photo.isMain = true;

            appUser.Photos.Add(photo);

            if (await uow.Complete())
                return CreatedAtAction(
                    nameof(GetAppUser)
                    , new { loginName = appUser.LoginName }
                    , mapper.Map<PhotoDTO>(photo)
                    );

            return BadRequest("Fotó hozzáadása sikertelen");
        }

        [HttpPut("set-main-photo/{photoId}")]
        public async Task<ActionResult> SetMainPhoto(int photoId)
        {
            var user = await uow.appUserRepository.GetUserByLoginNameAsync(User.GetLoginName());
            if (user == null)
                return NotFound();

            var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);
            if (photo == null)
                return NotFound();

            if (photo.isMain)
                return BadRequest("Ez már a profilképed!");

            var mainPhoto = user.Photos.FirstOrDefault(x => x.isMain);
            if (mainPhoto != null)
            {
                mainPhoto.isMain = false;
            }
            photo.isMain = true;

            if (await uow.Complete())
                return NoContent();

            return BadRequest("Profilkép beállítása sikertelen!");
        }

        [HttpDelete("delete-photo/{photoId}")]
        public async Task<ActionResult> DeletePhoto(int photoId)
        {
            var user = await uow.appUserRepository.GetUserByLoginNameAsync(User.GetLoginName());
            if (user == null)
                return NotFound();

            var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);
            if (photo == null)
                return NotFound();

            if (photo.isMain)
                return BadRequest("A profilképét nem törölheti!");

            if (photo.PublicId != null)
            {
                var result = await photoService.DeletePhotoAsync(photo.PublicId);
                if (result.Error != null)
                {
                    return BadRequest(result.Error.Message);
                }
            }
            user.Photos.Remove(photo);

            if (await uow.Complete())
                return Ok();

            return BadRequest("Kép törlése sikertelen!");
        }
    }
}
