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

        public ChatHub(IUserService users,
            IChatService chats)
        {
            this.users = users;
            this.chats = chats;
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

            await Clients.Group(chatName).SendAsync("ReceiveMessage", userFullName, message);
        }

        public async Task RetrieveChatHistory(string receiverId)
        {
            var claimsIdentity = (ClaimsIdentity)this.Context.User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            var idOfChatters = new List<string> { userId, receiverId };
            idOfChatters.Sort();

            var chatName = idOfChatters[0] + "-" + idOfChatters[1];

            var messages = this.chats.GetMessagesHistory(chatName);

            //var test = this.chats.GetUsersAndMessages(messages);

            await Clients.Client(Context.ConnectionId).SendAsync("ShowHistory", messages);

            //await Clients.Group(chatName).SendAsync("ShowHistory", messages);
        }

        public void CreateGroup(string receiverId)
        {
            var claimsIdentity = (ClaimsIdentity)this.Context.User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            var idOfChatters = new List<string> { userId, receiverId};
            idOfChatters.Sort();

            var chatName = idOfChatters[0] + "-" + idOfChatters[1];
            

            Groups.AddToGroupAsync(Context.ConnectionId, chatName);

            if (!this.chats.ChatExists(chatName))
            {
                this.chats.AddChat(chatName);
            }

            if (!this.chats.UserIsInChat(userId, chatName))
            {
                this.chats.AddUserToChat(userId, chatName);
            }
        }
    }
}
