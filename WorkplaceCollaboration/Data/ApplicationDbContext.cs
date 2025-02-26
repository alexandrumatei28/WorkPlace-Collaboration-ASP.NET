using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WorkplaceCollaboration.Models;

namespace WorkplaceCollaboration.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Channel> Channels { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<ChannelUser> ChannelUsers { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ChannelUser>()
            .HasKey(cu => new { cu.ChannelId, cu.UserId });

            builder.Entity<ChannelUser>()
                .HasOne(cu => cu.Channel)
                .WithMany(c => c.ChannelUsers)
                .HasForeignKey(cu => cu.ChannelId);

            builder.Entity<ChannelUser>()
                .HasOne(cu => cu.User)
                .WithMany(u => u.ChannelUsers)
                .HasForeignKey(cu => cu.UserId);
        }


    }
}