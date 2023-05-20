using API.Data.Repositories;
using API.Interfaces;
using AutoMapper;

namespace API.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DataContext context;
        private readonly IMapper mapper;

        public UnitOfWork(DataContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }
        public IAppUserRepository appUserRepository => new AppUserRepository(context, mapper);

        public IMessageRepository messageRepository => new MessageRepository(context, mapper);

        public ILikesRepository likesRepository => new LikesRepository(context);

        public async Task<bool> Complete()
        {
            return await context.SaveChangesAsync() > 0;
        }

        public bool HasChanges()
        {
            return context.ChangeTracker.HasChanges();
        }
    }
}
