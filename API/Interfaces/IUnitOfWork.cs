namespace API.Interfaces
{
    public interface IUnitOfWork
    {
        IAppUserRepository appUserRepository { get; }
        IMessageRepository messageRepository { get; }
        ILikesRepository likesRepository { get; }
        Task<bool> Complete();
        bool HasChanges();
    }
}
