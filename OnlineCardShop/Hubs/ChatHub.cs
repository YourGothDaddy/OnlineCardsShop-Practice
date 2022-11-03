namespace OnlineCardShop.Hubs
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.SignalR;
    using System.Threading.Tasks;
    using System.Security.Claims;
    using OnlineCardShop.Services.Users;
    using OnlineCardShop.Services.Chats;
    using System.Collections.Generic;

    [Authorize]
    public class ChatHub : Hub
    {
        private readonly IUserService users;
        private readonly IChatService chats;

        private static List<string> usersConnectionIds = new List<string>();

        public ChatHub(IUserService users,
            IChatService chats)
        {
            this.users = users;
            this.chats = chats;
        }

        public Task OnConnected()
        {
            usersConnectionIds.Add(Context.ConnectionId);

            var claimsIdentity = (ClaimsIdentity)this.Context.User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            this.users.SaveConnectionId(userId, Context.ConnectionId);

            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(System.Exception exception)
        {
            usersConnectionIds.Remove(Context.ConnectionId);
            return base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(string message, string receiverId)
        {
            var claimsIdentity = (ClaimsIdentity)this.Context.User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            var idOfChatters = new List<string> { userId, receiverId };
            idOfChatters.Sort();

            var chatName = idOfChatters[0] + "-" + idOfChatters[1];

            var userFullName = this.users.GetUserFullName(userId);

            this.chats.SaveMessage(chatName, userId, message);

            var chatId = this.chats.GetChatId(chatName);

            await Clients.Group(chatName).SendAsync("ReceiveMessage", userId, userFullName, message, receiverId);
        }

        public async Task RetrieveChatHistory(string receiverId)
        {
            var claimsIdentity = (ClaimsIdentity)this.Context.User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            var idOfChatters = new List<string> { userId, receiverId };
            idOfChatters.Sort();

            var chatName = idOfChatters[0] + "-" + idOfChatters[1];

            var messages = this.chats.GetMessagesHistory(chatName);

            await Clients.Client(Context.ConnectionId).SendAsync("ShowHistory", messages, receiverId);
        }

        public async Task RetrieveRecentChats()
        {
            var claimsIdentity = (ClaimsIdentity)this.Context.User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            var userFullName = this.users.GetUserFullName(userId);

            var recentChats = this.chats.RetrieveRecentChats(userId);

            var recentChatsTimesPassedSinceLastMessage = new List<string>();

            foreach (var chat in recentChats)
            {
                if(chat.Messages.Count > 0)
                {
                    var messageSentTimeAgo = this.chats.GetMessageSentTimeAgo(chat);
                    recentChatsTimesPassedSinceLastMessage.Add(messageSentTimeAgo);
                }
            }

            await Clients.Client(Context.ConnectionId).SendAsync("ShowRecentChats", recentChats, recentChatsTimesPassedSinceLastMessage, userFullName);
        }

        public async Task RetrieveRecentChatsOfUser(string receiverId)
        {
            var userFullName = this.users.GetUserFullName(receiverId);

            var recentChats = this.chats.RetrieveRecentChats(receiverId);

            var recentChatsTimesPassedSinceLastMessage = new List<string>();

            foreach (var chat in recentChats)
            {
                if (chat.Messages.Count > 0)
                {
                    var messageSentTimeAgo = this.chats.GetMessageSentTimeAgo(chat);
                    recentChatsTimesPassedSinceLastMessage.Add(messageSentTimeAgo);
                }
            }

            if (this.users.UserHasConnectionIdSaved(receiverId))
            {
                var receiverConnectionId = this.users.GetUserConnectionId(receiverId);

                if (usersConnectionIds.Contains(receiverConnectionId))
                {
                    await Clients.Client(receiverConnectionId).SendAsync("ShowRecentChats", recentChats, recentChatsTimesPassedSinceLastMessage, userFullName);
                }
            }
        }

        public void CreateGroup(string receiverId)
        {
            var claimsIdentity = (ClaimsIdentity)this.Context.User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            var idOfChatters = new List<string> { userId, receiverId};
            idOfChatters.Sort();

            var chatName = idOfChatters[0] + "-" + idOfChatters[1];

            this.users.SaveConnectionId(userId, Context.ConnectionId);

            Groups.AddToGroupAsync(Context.ConnectionId, chatName);

            if (!this.chats.ChatExists(chatName))
            {
                this.chats.AddChat(chatName);
            }

            if (!this.chats.UserIsInChat(userId, chatName))
            {
                this.chats.AddUserToChat(userId, chatName);
            }

            if (!this.chats.UserIsInChat(receiverId, chatName))
            {
                this.chats.AddUserToChat(receiverId, chatName);
            }
        }
    }
}
