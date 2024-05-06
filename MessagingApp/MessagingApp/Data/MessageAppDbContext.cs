using MessagingApp.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MessagingApp.Data
{
    public class MessageAppDbContext : IdentityDbContext<User, Role, string>
    {
        public DbSet<Chat> Chats { get; set; }
        public DbSet<Message> Messages { get; set; }

        public MessageAppDbContext(DbContextOptions<MessageAppDbContext> options) : base(options)
        {
        }

        public MessageAppDbContext()
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            //sets tha one to many relationships for the DB
            builder.Entity<Chat>().HasOne(x => x.Receiver).WithMany(x => x.ReceiverChats).HasForeignKey(x => x.ReceiverId).OnDelete(DeleteBehavior.NoAction);
            builder.Entity<Chat>().HasOne(x => x.Sender).WithMany(x => x.SenderChats).HasForeignKey(x => x.SenderId).OnDelete(DeleteBehavior.NoAction);
            builder.Entity<Chat>().HasMany(x => x.Messages).WithOne(x => x.Chat).HasForeignKey(x => x.ChatId).OnDelete(DeleteBehavior.NoAction);
            builder.Entity<User>().HasOne(x => x.Role).WithMany(x => x.Users).OnDelete(DeleteBehavior.NoAction);
        }
    }
}
