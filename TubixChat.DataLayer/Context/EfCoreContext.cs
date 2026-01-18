using Microsoft.EntityFrameworkCore;
using TubixChat.DataLayer.Entities;

namespace TubixChat.DataLayer.Context
{
    public class EfCoreContext : DbContext
    {
        public EfCoreContext(DbContextOptions<EfCoreContext> options)
            : base(options)
        {
        }

        public DbSet<State> States { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Message> Messages { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<State>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedNever();
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.UserName).IsUnique();

                entity.HasOne(e => e.State)
                    .WithMany()
                    .HasForeignKey(e => e.StateId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Message>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasIndex(e => new { e.SenderUserId, e.RecieverUserId, e.CreatedAt })
                    .HasDatabaseName("idx_message_pair");

                entity.HasIndex(e => new { e.RecieverUserId, e.IsRead })
                    .HasDatabaseName("idx_message_unread");

                entity.HasOne(e => e.Sender)
                    .WithMany()
                    .HasForeignKey(e => e.SenderUserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Reciever)
                    .WithMany()
                    .HasForeignKey(e => e.RecieverUserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}