namespace OnlineCardShop.Services.Chats
{
    using OnlineCardShop.Data.Models;
    using System.Collections.Generic;

    public interface IChatService
    {
        public bool ChatExists(string chatName);

        public bool UserIsInChat(string userId, string chatName);

        public void AddChat(string chatName);

        public void AddUserToChat(string userId, string chatName);

        public void SaveMessage(string chatName, string userId, string content);

        public IEnumerable<Message> GetMessagesHistory(string chatName);

        public IDictionary<string, string> GetUsersAndMessages(IEnumerable<Message> messages);
    }
}
