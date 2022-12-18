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
            var user = this.data
                .Users
                .Include(u => u.Chats)
                .Where(u => u.Id == userId)
                .FirstOrDefault();

            var userIsInChat = user.Chats.Any(c => c.Name == chatName);

            return userIsInChat;
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
                    CreatedAt = DateTime.UtcNow
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

            var test = this.data
                .Messages
                .ToList();


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

        public Message GetLastMessage(string chatName)
        {
            var chatId = this.data
                .Chats
                .Where(c => c.Name == chatName)
                .Select(c => c.Id)
                .FirstOrDefault();

            var message = this.data
                .Messages
                .Where(m => m.ChatId == chatId)
                .OrderBy(m => m.Id)
                .LastOrDefault();

            if (message != null)
            {
                return message;
            }

            return null;
        }

        public ICollection<Chat> RetrieveRecentChats(string userId)
        {
            var userChats = this.data
                .Chats
                .Include(x => x.Users)
                .Include(x => x.Messages)
                .Where(c => c.Users.Any(u => u.Id == userId))
                .ToList();

            var listOfMessages = new List<Message>();

            foreach (var chat in userChats)
            {
                if (chat.Messages.Count > 0)
                {
                    var lastMessage = chat.Messages.LastOrDefault();
                    listOfMessages.Add(lastMessage);
                }
            }

            var ordered = listOfMessages.OrderByDescending(x => x.CreatedAt.Ticks).ToList();

            var orderedUserChats = new List<Chat>();

            foreach (var message in ordered)
            {
                foreach (var chat in userChats)
                {
                    if (chat.Messages.Contains(message))
                    {
                        orderedUserChats.Add(chat);
                    }
                }
            }



            return orderedUserChats;
        }

        public int GetChatId(string chatName)
        {
            return this.data
                .Chats
                .Where(c => c.Name == chatName)
                .Select(c => c.Id)
                .FirstOrDefault();
        }

        public DateTime GetDateTimeOfLastMessage(int chatId)
        {

            return this.data
                .Messages
                .Where(c => c.ChatId == chatId)
                .OrderByDescending(c => c.Id)
                .Select(m => m.CreatedAt)
                .FirstOrDefault();
        }

        public string GetMessageSentTimeAgo(Chat chat)
        {
            const int SECOND = 1;
            const int MINUTE = 60 * SECOND;
            const int HOUR = 60 * MINUTE;
            const int DAY = 24 * HOUR;
            const int MONTH = 30 * DAY;

            var datetimeOfLastMessage = this.GetDateTimeOfLastMessage(chat.Id);

            var ts = new TimeSpan(DateTime.UtcNow.Ticks - datetimeOfLastMessage.Ticks);
            
            double delta = Math.Abs(ts.TotalSeconds);

            if (delta < 1 * MINUTE)
                return ts.Seconds == 1 ? "one second ago" : ts.Seconds + " seconds ago";

            if (delta < 2 * MINUTE)
                return "a minute ago";

            if (delta < 45 * MINUTE)
                return ts.Minutes + " minutes ago";

            if (delta < 90 * MINUTE)
                return "an hour ago";

            if (delta < 24 * HOUR)
                return ts.Hours + " hours ago";

            if (delta < 48 * HOUR)
                return "yesterday";

            if (delta < 30 * DAY)
                return ts.Days + " days ago";

            if (delta < 12 * MONTH)
            {
                int months = Convert.ToInt32(Math.Floor((double)ts.Days / 30));
                return months <= 1 ? "one month ago" : months + " months ago";
            }
            else
            {
                int years = Convert.ToInt32(Math.Floor((double)ts.Days / 365));
                return years <= 1 ? "one year ago" : years + " years ago";
            }
        }

        public void OrderChats(List<Chat> recentChats)
        {
            var lastMessages = new List<Message>();

            foreach (var chat in recentChats)
            {
                if(chat.Messages.Count > 0)
                {
                    var lastMessage = chat.Messages.LastOrDefault();
                    lastMessages.Add(lastMessage);
                }
            }
        }
    }
}
