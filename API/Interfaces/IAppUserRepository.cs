using API.DTOs;
using API.Entities;

namespace API.Interfaces
{
    public interface IAppUserRepository
    {
        void Update(AppUser user);
        Task<bool> SaveAllAsync();
        Task<IEnumerable<AppUser>> GetUsersAsync();
        Task<AppUser> GetUserByIdAsync(int id);
        Task<AppUser> GetUserByLoginNameAsync(string loginName);
        Task<IEnumerable<MemberDTO>> GetMembersAsync();
        Task<MemberDTO> GetMemberByLoginNameAsync(string loginName);
    }
}
