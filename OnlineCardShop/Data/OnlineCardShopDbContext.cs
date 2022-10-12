using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OnlineCardShop.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineCardShop.Data
{
    public class OnlineCardShopDbContext : IdentityDbContext<User>
    {
        public OnlineCardShopDbContext(DbContextOptions<OnlineCardShopDbContext> options)
            : base(options)
        {
        }

        public DbSet<Card> Cards { get; set; }

        public DbSet<Category> Categories { get; set; }

        public DbSet<Condition> Conditions { get; set; }

        public DbSet<Dealer> Dealers { get; init; }

        public DbSet<Image> Images { get; set; }

        public DbSet<ProfileImage> ProfileImages { get; set; }

        public DbSet<Review> Reviews { get; set; }

        public DbSet<Chat> Chats { get; set; }

        public DbSet<Message> Messages { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder
                .Entity<Card>()
                .HasOne(c => c.Category)
                .WithMany(c => c.Cards)
                .HasForeignKey(c => c.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .Entity<Card>()
                .HasOne(c => c.Dealer)
                .WithMany(d => d.Cards)
                .HasForeignKey(c => c.DealerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .Entity<Dealer>()
                .HasOne<User>()
                .WithOne()
                .HasForeignKey<Dealer>(d => d.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .Entity<Image>()
                .HasOne(i => i.Card)
                .WithOne(c => c.Image)
                .HasForeignKey<Card>(i => i.ImageId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .Entity<ProfileImage>()
                .HasOne(pi => pi.User)
                .WithOne(u => u.ProfileImage)
                .HasForeignKey<User>(pi => pi.ProfileImageId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .Entity<Review>()
                .HasOne(r => r.User)
                .WithMany(u => u.Reviews)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .Entity<Message>()
                .HasOne(m => m.Chat)
                .WithMany(c => c.Messages)
                .HasForeignKey(m => m.ChatId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .Entity<Message>()
                .HasOne(m => m.User)
                .WithMany(u => u.Messages)
                .HasForeignKey(m => m.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .Entity<UserChat>()
                .HasKey(uc => new { uc.UserId, uc.ChatId });

            builder
                .Entity<UserChat>()
                .HasOne<User>(u => u.User)
                .WithMany(c => c.UserChats)
                .HasForeignKey(uc => uc.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .Entity<UserChat>()
                .HasOne<Chat>(uc => uc.Chat)
                .WithMany(c => c.UserChats)
                .HasForeignKey(uc => uc.ChatId)
                .OnDelete(DeleteBehavior.Restrict);

            base.OnModelCreating(builder);
        }
    }
}
