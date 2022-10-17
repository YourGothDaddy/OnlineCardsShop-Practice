namespace OnlineCardShop.Services.Chats
{
    using Microsoft.EntityFrameworkCore;
    using OnlineCardShop.Data;
    using OnlineCardShop.Data.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class ChatService : IChatService
    {
        private readonly OnlineCardShopDbContext data;

        public ChatService(OnlineCardShopDbContext data)
        {
            this.data = data;
        }

        public bool ChatExists(string chatName)
        {
            return this.data
                .Chats
                .Select(c => c.Name)
                .Contains(chatName);
        }

        public bool UserIsInChat(string userId, string chatName)
        {
            var chat = this.data
                .Chats
                .Where(c => c.Name == chatName)
                .Where(c => c.Users.Any(c => c.Id == userId))
                .ToList();

            return chat.Count() > 0;
        }

        public void AddChat(string chatName)
        {
            this.data
                .Chats
                .Add(new Chat
                {
                    Name = chatName
                });

            this.data.SaveChanges();
        }

        public void AddUserToChat(string userId, string chatName)
        {
            var chatId = this.data
                .Chats
                .Where(c => c.Name == chatName)
                .Select(c => c.Id)
                .FirstOrDefault();

            var user = this.data
                .Users
                .Where(c => c.Id == userId)
                .FirstOrDefault();

            var chat = this.data
                .Chats
                .Where(c => c.Name == chatName)
                .FirstOrDefault();

            chat.Users.Add(user);

            this.data.SaveChanges();
        }

        public void SaveMessage(string chatName, string userId, string content)
        {
            var chatId = this.data
                .Chats
                .Where(c => c.Name == chatName)
                .Select(c => c.Id)
                .FirstOrDefault();

            this.data
                .Messages
                .Add(new Message
                {
                    ChatId = chatId,
                    UserId = userId,
                    Content = content,
                    CreatedAt = DateTime.Now
                });

            this.data.SaveChanges();
        }

        public IEnumerable<Message> GetMessagesHistory(string chatName)
        {
            var chatId = this.data
                .Chats
                .Where(c => c.Name == chatName)
                .Select(c => c.Id)
                .FirstOrDefault();

            var messages = this.data
                .Messages
                .Include(x => x.User)
                .Where(m => m.ChatId == chatId)
                .ToList();

            if(messages.Count > 0)
            {
                return messages;
            }

            return null;
        }

        public IDictionary<string, string> GetUsersAndMessages(IEnumerable<Message> messages)
        {
            var users = messages
                .Select(m => m.User.FullName)
                .ToList();

            return null;
        }
    }
}
