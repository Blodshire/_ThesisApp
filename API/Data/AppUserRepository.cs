using API.DTOs;
using API.Entities;
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

        public async Task<IEnumerable<MemberDTO>> GetMembersAsync()
        {
            return await context.AppUsers
                .ProjectTo<MemberDTO>(mapper.ConfigurationProvider)
                .ToListAsync();
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
