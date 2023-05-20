using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Helpers.PaginationHelperParams;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data.Repositories
{
    public class MessageRepository : IMessageRepository
    {
        private readonly DataContext context;
        private readonly IMapper mapper;

        public MessageRepository(DataContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public void AddGroup(Group group)
        {
           context.Groups.Add(group);
        }

        public void AddMessage(Message message)
        {
            context.Messages.Add(message);
        }

        public void DeleteMessage(Message message)
        {
            context.Messages.Remove(message);
        }

        public async Task<Connection> GetConnectionAsync(string connectionId)
        {
           return await context.Connections.FindAsync(connectionId);
        }

        public async Task<Message> GetMessageAsync(int id)
        {
            return await context.Messages.FindAsync(id);
        }

        public async Task<Group> GetMessageGroupAsync(string groupName)
        {
            return await context.Groups.Include(x=>x.Connections).FirstOrDefaultAsync(x=> x.Name== groupName);
        }

        public async Task<PagedList<MessageDTO>> GetMessagesForUserAsync(MessageParams messageParams)
        {
            var query = context.Messages
                .OrderByDescending(x => x.MessageSent)
                .AsQueryable();
            query = messageParams.Container switch
            {
                "Inbox" => query.Where(x => x.RecipientLoginName == messageParams.LoginName && x.RecipientDeleted==false),
                "Outbox" => query.Where(x => x.SenderLoginName == messageParams.LoginName && x.SenderDeleted == false),
                _ => query.Where(x => x.RecipientLoginName == messageParams.LoginName && x.DateRead == null && x.RecipientDeleted == false) 
            };

            return await PagedList<MessageDTO>.CreateAsync(
                query.ProjectTo<MessageDTO>(mapper.ConfigurationProvider),
                messageParams.PageNumber,
                messageParams.PageSize);
        }

        public async Task<IEnumerable<MessageDTO>> GetMessageThreadAsync(string currentLoginName, string recipientLoginName)
        {
            var messages = await context.Messages
                 .Include(x => x.Sender).ThenInclude(xx => xx.Photos)
                 .Include(x => x.Recipient).ThenInclude(xx => xx.Photos)
                 .Where(
                 x => x.RecipientLoginName == currentLoginName
                 && x.SenderLoginName == recipientLoginName
                 && x.RecipientDeleted == false
                 || x.RecipientLoginName == recipientLoginName
                 && x.SenderLoginName == currentLoginName
                 && x.SenderDeleted == false
                 )
                 .OrderBy(x => x.MessageSent)
                 .ToListAsync();

            var unreadMessages = messages.Where(x => x.DateRead == null && x.RecipientLoginName == currentLoginName).ToList();

            if (unreadMessages.Any())
            {
                foreach (var msg in unreadMessages)
                {
                    msg.DateRead = DateTime.UtcNow;
                }
                await context.SaveChangesAsync();
            }

            return mapper.Map<IEnumerable<MessageDTO>>(messages);
        }

        public void RemoveConnection(Connection connection)
        {
            context.Connections.Remove(connection);
        }

        public async Task<bool> SaveAllAsync()
        {
            return await context.SaveChangesAsync() > 0;
        }
    }
}
