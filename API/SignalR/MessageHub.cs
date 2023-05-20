using API.Data.Repositories;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalR
{
    [Authorize]
    public class MessageHub : Hub
    {
        private readonly IMessageRepository messageRepository;
        private readonly IAppUserRepository appUserRepository;
        private readonly IMapper mapper;
        private readonly IHubContext<PresenceHub> presenceHub;

        public MessageHub(IMessageRepository messageRepository, IAppUserRepository appUserRepository, IMapper mapper, IHubContext<PresenceHub> presenceHub)
        {
            this.messageRepository = messageRepository;
            this.appUserRepository = appUserRepository;
            this.mapper = mapper;
            this.presenceHub = presenceHub;
        }

        public override async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            var otherLoginName = httpContext.Request.Query["user"];
            var groupName = GetGroupName(Context.User.GetLoginName(), otherLoginName);

            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            await AddToGroupAsync(groupName);

            var messages = await messageRepository.GetMessageThreadAsync(Context.User.GetLoginName(), otherLoginName);

            await Clients.Group(groupName).SendAsync("ReceiveMessageThread", messages);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await RemvoeFromGroupAsync();
            await base.OnDisconnectedAsync(exception);
        }
        private string GetGroupName(string caller, string other)
        {
            return string.CompareOrdinal(caller, other) < 0 ? $"{caller}-{other}" : $"{other}-{caller}";
        }

        public async Task SendMessage(CreateMessageDTO msg)
        {
            var loginName = Context.User.GetLoginName();

            if (loginName == msg.RecipientLoginName.ToLower())
                throw new HubException("Magadnak nem küldhetsz üzenetet!");

            var sender = await appUserRepository.GetUserByLoginNameAsync(loginName);
            var recipient = await appUserRepository.GetUserByLoginNameAsync(msg.RecipientLoginName.ToLower());

            if (recipient == null)
                throw new HubException("Ilyen felhasználó nincs!");

            var insertMessage = new Message
            {
                Sender = sender,
                Recipient = recipient,
                SenderLoginName = sender.LoginName,
                RecipientLoginName = recipient.LoginName,
                Content = msg.Content
            };

            var groupName = GetGroupName(sender.LoginName, recipient.LoginName);
            var group = await messageRepository.GetMessageGroupAsync(groupName);

            if(group.Connections.Any(x=> x.LoginName == recipient.LoginName))
                insertMessage.DateRead= DateTime.UtcNow;
            else
            {
                var connections = await PresenceTracker.GetConnectionsForUserAsync(recipient.LoginName);
                if (connections != null)
                {
                    await presenceHub.Clients.Clients(connections).SendAsync("NewMessageReceived"
                        , new { loginName = sender.LoginName, displayName = sender.DisplayName });
                }
            }
            messageRepository.AddMessage(insertMessage);

            if (await messageRepository.SaveAllAsync())
            {
                //var group = GetGroupName(sender.LoginName, recipient.LoginName);
                await Clients.Group(groupName).SendAsync("NewMessage", mapper.Map<MessageDTO>(insertMessage));
            }
            else
            {
                throw new HubException("Az üzenetet nem sikerült elküldeni!");
            }

           
        }

        private async Task<bool> AddToGroupAsync(string groupName)
        {
            var group = await messageRepository.GetMessageGroupAsync(groupName);
            var connection = new Connection(Context.ConnectionId, Context.User.GetLoginName());

            if(group == null)
            {
                group = new Group(groupName);
                messageRepository.AddGroup(group);
            }
            group.Connections.Add(connection);

            return await messageRepository.SaveAllAsync();
        }

        private async Task RemvoeFromGroupAsync()
        {
            messageRepository.RemoveConnection(
                await messageRepository.GetConnectionAsync(Context.ConnectionId)
                );

            await messageRepository.SaveAllAsync();
        }
    }
}
