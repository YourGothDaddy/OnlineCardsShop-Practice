namespace OnlineCardShop.Services.Users
{
    using OnlineCardShop.Data.Models;
    using System.Collections.Generic;

    public interface IUserService
    {
        public User GetUser(string id);

        public string GetUserFullName(string userId);

        public UserDetailsServiceViewModel GetUserDetails(string id);

        public void SaveConnectionId(string userId, string connectionId);

        public bool UserHasConnectionIdSaved(string userId);

        public string GetUserConnectionId(string userId);

        ProfileImage CreateProfileImage(string imageName, string imagePathForDb, string originalImageName);

        public void AddProfileImageToDB(ProfileImage profileImage);

        public List<string> GetRecentChatsSendersProfileImages(ICollection<Chat> recentChats, string userId);

        public string IdOfReceiverOfMostRecentChat(string userId);

        public void SaveReport(string reportReason, string reportedUserId);

        public void ChangeProfileImage(User user, int id);

        public string GetProfileImagePath(string userId);

        public string GetAboutMe(string userId);

        public void SetAboutMe(string userId, string content);
    }
}
