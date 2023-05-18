using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class DataContext : IdentityDbContext
        <AppUser, AppRole, int, 
        IdentityUserClaim<int>, AppUserRole,
        IdentityUserLogin<int>, IdentityRoleClaim<int>,
        IdentityUserToken<int>
        >
    {
        public DataContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<AppUser> AppUsers { get => base.Users; set => base.Users = value; }
        public DbSet<UserLike> Likes { get; set; }
        public DbSet<Message> Messages { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<AppUser>()
                .HasMany(xr => xr.UserRoles)
                .WithOne(x => x.User)
                .HasForeignKey(xr => xr.UserId)
                .IsRequired();


            builder.Entity<AppRole>()
                .HasMany(xr => xr.UserRoles)
                .WithOne(x => x.Role)
                .HasForeignKey(xr => xr.RoleId)
                .IsRequired();

            builder.Entity<UserLike>()
                .HasKey(k => new { k.SourceUserId, k.TargetUserId });

            builder.Entity<UserLike>()
                .HasOne(s => s.SourceUser)
                .WithMany(l => l.LikedUsers)
                .HasForeignKey(s=>s.SourceUserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<UserLike>()
               .HasOne(s => s.TargetUser)
               .WithMany(l => l.LikedByUsers)
               .HasForeignKey(s => s.TargetUserId)
               .OnDelete(DeleteBehavior.Cascade);
            //.OnDelete(DeleteBehavior.NoAction); on non-sqlite dbs

            builder.Entity<Message>()
               .HasOne(s => s.Recipient)
               .WithMany(l => l.MessagesReceived)
               .OnDelete(DeleteBehavior.Restrict);
            //keep messages after profile deleted

            builder.Entity<Message>()
              .HasOne(s => s.Sender)
              .WithMany(l => l.MessagesSent)
              .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
