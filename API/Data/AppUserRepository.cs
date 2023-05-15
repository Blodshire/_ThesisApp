using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Helpers.PaginationHelperParams;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class AppUserRepository : IAppUserRepository
    {
        private readonly DataContext context;
        private readonly IMapper mapper;

        public AppUserRepository(DataContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public async Task<MemberDTO> GetMemberByLoginNameAsync(string loginName)
        {
            return await context.AppUsers.Where(x => x.LoginName == loginName)
                .ProjectTo<MemberDTO>(mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();
        }

        public async Task<PagedList<MemberDTO>> GetMembersAsync(UserParams userParams)
        {
            //return await context.AppUsers
            //    //.Take(4) -- painful pagination
            //    //.Skip(4)
            //    .ProjectTo<MemberDTO>(mapper.ConfigurationProvider)
            //    .ToListAsync();

            var query = context.AppUsers.AsQueryable();
            //.ProjectTo<MemberDTO>(mapper.ConfigurationProvider)
            //.AsNoTracking(); --not essential, slight optimization
            query = query.Where(x => x.LoginName != userParams.CurrentLogin);
            query = query.Where(x => x.Gender == userParams.Gender);


            query = userParams.OrderBy switch
            {
                "created" => query.OrderByDescending(x => x.Created),
                _=> query.OrderByDescending(x => x.LastActive)
            };
            var minDOB = DateOnly.FromDateTime(DateTime.Today.AddYears(-userParams.MaxAge - 1));
            var maxDOB = DateOnly.FromDateTime(DateTime.Today.AddYears(-userParams.MinAge));

            query = query.Where(x => x.DateOfBirth >= minDOB && x.DateOfBirth <= maxDOB);

            return await PagedList<MemberDTO>.CreateAsync(
                query.AsNoTracking().ProjectTo<MemberDTO>
                    (mapper.ConfigurationProvider),
                    userParams.PageNumber,
                    userParams.PageSize);
        }

        public async Task<AppUser> GetUserByIdAsync(int id)
        {
            return await context.AppUsers.Where(x => x.Id == id)
                .Include(i => i.Photos)
                .FirstOrDefaultAsync();

        }

        public async Task<AppUser> GetUserByLoginNameAsync(string loginName)
        {
            return await context.AppUsers.Where(x => x.LoginName.Equals(loginName))
                .Include(i => i.Photos)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<AppUser>> GetUsersAsync()
        {
            return await context.AppUsers
                .Include(i => i.Photos)
                .ToListAsync();
        }

        public async Task<bool> SaveAllAsync()
        {
            return await context.SaveChangesAsync() > 0;
        }

        public void Update(AppUser user)
        {
            context.Update(user);
        }
    }
}
