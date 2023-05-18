using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Helpers.PaginationHelperParams;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Data.Repositories
{
    public class LikesRepository : ILikesRepository
    {
        private readonly DataContext context;

        public LikesRepository(DataContext context)
        {
            this.context = context;
        }
        public async Task<UserLike> GetUserLike(int sourceUserId, int targetUserId)
        {
            return await context.Likes.FindAsync(sourceUserId, targetUserId);
        }

        public async Task<PagedList<LikeDTO>> GetUserLikes(LikesParams likesParams)
        {
            var predicate = likesParams.Predicate;
            var userId = likesParams.UserId;

            var users = context.AppUsers.OrderBy(x => x.LoginName).AsQueryable();
            var likes = context.Likes.AsQueryable();

            if (predicate == "liked")
            {
                likes = likes.Where(like => like.SourceUserId == userId);
                users = likes.Select(like => like.TargetUser);
            }
            if (predicate == "likedBy")
            {
                likes = likes.Where(like => like.TargetUserId == userId);
                users = likes.Select(like => like.SourceUser);
            }

            var likedUsers = users.Select(user => new LikeDTO
            {
                LoginName = user.LoginName,
                DisplayName = user.DisplayName,
                Age = user.DateOfBirth.CalculateAge(),
                PhotoUrl = user.Photos.Where(x => x.isMain).FirstOrDefault().url,
                City = user.City,
                Id = user.Id,
            });
            return await PagedList<LikeDTO>.CreateAsync(likedUsers, likesParams.PageNumber, likesParams.PageSize);
        }

        public async Task<AppUser> GetUserWithLikes(int userId)
        {
            return await context.AppUsers
                 .Include(x => x.LikedUsers)
                 .FirstOrDefaultAsync(x => x.Id == userId);
        }
    }
}
