namespace OnlineCardShop.Hubs
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.SignalR;
    using OnlineCardShop.Data.Models;
    using System.Threading.Tasks;
    using System.Security.Claims;
    using OnlineCardShop.Services.Users;

    [Authorize]
    public class ChatHub : Hub
    {
        private readonly IUserService users;

        public ChatHub(IUserService users)
        {
            this.users = users;
        }
        public async Task SendMessage(string message)
        {

            var claimsIdentity = (ClaimsIdentity)this.Context.User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            var userFullName = this.users.GetUserFullName(userId);

            await Clients.All.SendAsync("ReceiveMessage", userFullName, message);
        }
    }
}
