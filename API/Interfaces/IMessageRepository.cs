using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Helpers.PaginationHelperParams;

namespace API.Interfaces
{
    public interface IMessageRepository
    {
        void AddMessage(Message message);
        void DeleteMessage(Message message);
        Task<Message> GetMessageAsync(int id);
        Task<PagedList<MessageDTO>> GetMessagesForUserAsync(MessageParams messageParams);
        Task<IEnumerable<MessageDTO>> GetMessageThreadAsync(string currentLoginName, string recipientLoginName);
        //Task<bool> SaveAllAsync();

        void AddGroup(Group group);
        void RemoveConnection(Connection connection);
        Task<Connection> GetConnectionAsync(string connectionId);
        Task<Group> GetMessageGroupAsync(string groupName);
    }
}
