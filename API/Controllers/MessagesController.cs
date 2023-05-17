using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Helpers.PaginationHelperParams;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class MessagesController : BaseApiController
    {
        private readonly IAppUserRepository appUserRepository;
        private readonly IMessageRepository messageRepository;
        private readonly IMapper mapper;

        public MessagesController(IAppUserRepository appUserRepository, IMessageRepository messageRepository, IMapper mapper)
        {
            this.appUserRepository = appUserRepository;
            this.messageRepository = messageRepository;
            this.mapper = mapper;
        }

        [HttpPost]
        public async Task<ActionResult<MessageDTO>> CreateMessage(CreateMessageDTO msg)
        {
            var loginName = User.GetLoginName();

            if (loginName == msg.RecipientLoginName.ToLower())
                return BadRequest("Magadnak nem küldhetsz üzenetet!");

            var sender = await appUserRepository.GetUserByLoginNameAsync(loginName);
            var recipient = await appUserRepository.GetUserByLoginNameAsync(msg.RecipientLoginName.ToLower());

            if (recipient == null)
                return NotFound();

            var insertMessage = new Message
            {
                Sender = sender,
                Recipient = recipient,
                SenderLoginName = sender.LoginName,
                RecipientLoginName = recipient.LoginName,
                Content = msg.Content
            };

            messageRepository.AddMessage(insertMessage);

            if (await messageRepository.SaveAllAsync())
                return Ok(mapper.Map<MessageDTO>(insertMessage));

            return BadRequest("Az üzenetet nem sikerült elküldeni!");
        }

        [HttpGet]
        public async Task<ActionResult<PagedList<MessageDTO>>> GetMessagesForUser([FromQuery] MessageParams messageParams)
        {
            messageParams.LoginName = User.GetLoginName().ToLower();

            var returnMessages = await messageRepository.GetMessagesForUserAsync(messageParams);

            Response.AddPaginationHeader(
                new PaginationHeader(
                returnMessages.CurrentPage,
                returnMessages.PageSize,
                returnMessages.TotalCount,
                returnMessages.TotalPages));

            return returnMessages;
        }

        [HttpGet("thread/{loginname}")]
        public async Task<ActionResult<IEnumerable<MessageDTO>>> GetMessageThread(string loginname)
        {
            var currentLoginName = User.GetLoginName().ToLower();

            return Ok(await messageRepository.GetMessageThreadAsync(currentLoginName, loginname));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteMessage(int id)
        {
            var loginName = User.GetLoginName().ToLower();

            var message = await messageRepository.GetMessageAsync(id);

            if(message.SenderLoginName!= loginName &&message.RecipientLoginName!=loginName) {
                return Unauthorized();
            }

            if (message.SenderLoginName == loginName)
                message.SenderDeleted = true;
            else
                message.RecipientDeleted = true;

            if (message.SenderDeleted && message.RecipientDeleted)
                messageRepository.DeleteMessage(message);

            if (await messageRepository.SaveAllAsync())
                return Ok();
            return BadRequest("Üzenetet nem lehet törölni");
        }
    }
}