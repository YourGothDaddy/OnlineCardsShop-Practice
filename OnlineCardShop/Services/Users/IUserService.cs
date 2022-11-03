namespace OnlineCardShop.Services.Users
{
    using OnlineCardShop.Data.Models;

    public interface IUserService
    {
        public User GetUser(string id);

        public string GetUserFullName(string userId);

        public UserDetailsServiceViewModel GetUserDetails(string id);

        public void SaveConnectionId(string userId, string connectionId);

        public bool UserHasConnectionIdSaved(string userId);

        public string GetUserConnectionId(string userId);
    }
}
