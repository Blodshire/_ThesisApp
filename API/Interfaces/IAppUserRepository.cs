using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Helpers.PaginationHelperParams;

namespace API.Interfaces
{
    public interface IAppUserRepository
    {
        void Update(AppUser user);
        //Task<bool> SaveAllAsync();
        Task<IEnumerable<AppUser>> GetUsersAsync();
        Task<AppUser> GetUserByIdAsync(int id);
        Task<AppUser> GetUserByLoginNameAsync(string loginName);
        Task<PagedList<MemberDTO>> GetMembersAsync(UserParams userParams);
        Task<MemberDTO> GetMemberByLoginNameAsync(string loginName);
    }
}
