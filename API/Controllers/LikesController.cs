using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Helpers.PaginationHelperParams;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class LikesController : BaseApiController
    {
        private readonly IAppUserRepository appUserRepository;
        private readonly ILikesRepository likesRepository;

        public LikesController(IAppUserRepository appUserRepository, ILikesRepository likesRepository)
        {
            this.appUserRepository = appUserRepository;
            this.likesRepository = likesRepository;
        }

        [HttpPost("{loginname}")]
        public async Task<ActionResult> AddLike(string loginname)
        {
            var sourceUserId = User.GetUserId();
            var likedUser = await appUserRepository.GetUserByLoginNameAsync(loginname);
            var sourceUser = await likesRepository.GetUserWithLikes(sourceUserId);

            if (sourceUser.UserName == loginname)
                return BadRequest("Magadat nem kedvelheted!");

            if (likedUser == null)
                return NotFound();

            var userLike = await likesRepository.GetUserLike(sourceUserId, likedUser.Id);
            if (userLike != null)
                return BadRequest("Már kedvelted ezt a felhasználót!");

            userLike = new UserLike
            {
                SourceUserId = sourceUserId,
                TargetUserId = likedUser.Id
            };

            sourceUser.LikedUsers.Add(userLike);

            //reuse save context from previous repo, refactor later
            if (await appUserRepository.SaveAllAsync())
                return Ok();

            return BadRequest("Nem tudta kedvelni a felhasználót.");
        }

        [HttpGet]
        public async Task<ActionResult<PagedList<LikeDTO>>> GetUserLikes([FromQuery]LikesParams likesParams)
        {
            likesParams.UserId = User.GetUserId();
            var returnUsers = await likesRepository.GetUserLikes(likesParams);

            Response.AddPaginationHeader(
                new PaginationHeader(returnUsers.CurrentPage, returnUsers.PageSize
                                , returnUsers.TotalCount, returnUsers.TotalPages));

            return Ok(returnUsers);

        }
    }
}
