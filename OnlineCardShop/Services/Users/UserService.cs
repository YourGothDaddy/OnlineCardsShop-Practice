namespace OnlineCardShop.Services.Users
{
    using Microsoft.EntityFrameworkCore;
    using OnlineCardShop.Data;
    using OnlineCardShop.Data.Models;
    using System.Collections.Generic;
    using System.Linq;

    public class UserService : IUserService
    {
        private readonly OnlineCardShopDbContext data;

        public UserService(OnlineCardShopDbContext data)
        {
            this.data = data;
        }

        public User GetUser(string id)
        {
            return this.data
                .Users
                .Where(u => u.Id == id)
                .FirstOrDefault();
        }

        public string GetUserFullName(string userId)
        {
            var userFullName = this.data
                .Users
                .Where(u => u.Id == userId)
                .Select(u => u.FullName)
                .FirstOrDefault();

            return userFullName;
        }

        public UserDetailsServiceViewModel GetUserDetails(string id)
        {
            return this.data
                .Users
                .Where(u => u.Id == id)
                .Select(u => new UserDetailsServiceViewModel
                {
                    Id = u.Id,
                    FullName = u.FullName,
                    ProfileImage = u.ProfileImage.Path.Replace("res", string.Empty),
                    AboutMe = u.AboutMe
                })
                .FirstOrDefault();
        }

        public void SaveConnectionId(string userId, string connectionId)
        {
            var user = this.data
                .Users
                .Where(u => u.Id == userId)
                .FirstOrDefault();

            user.ConnectionId = connectionId;

            this.data.SaveChanges();
        }

        public bool UserHasConnectionIdSaved(string userId)
        {
            var userConnectionId = this.data
                .Users
                .Where(u => u.Id == userId)
                .Select(u => u.ConnectionId)
                .FirstOrDefault();

            return userConnectionId != null;
        }

        public string GetUserConnectionId(string userId)
        {
            return this.data
                .Users
                .Where(u => u.Id == userId)
                .Select(u => u.ConnectionId)
                .FirstOrDefault();
        }

        public ProfileImage CreateProfileImage(
            string imageName,
            string imagePathForDb,
            string originalImageName)
        {
            return new ProfileImage
            {
                Name = imageName,
                Path = imagePathForDb,
                OriginalName = originalImageName
            };
        }

        public void AddProfileImageToDB(ProfileImage profileImage)
        {
            this.data.ProfileImages.Add(profileImage);
            this.data.SaveChanges();
        }

        public List<string> GetRecentChatsSendersProfileImages(ICollection<Chat> recentChats, string userId)
        {
            var senderProfilesImages = new List<string>();

            foreach (var chat in recentChats)
            {
                var currentSenderId = chat.Users
                    .Where(u => u.Id != userId)
                    .Select(u => u.Id)
                    .FirstOrDefault();

                if(currentSenderId != null)
                {
                    var currentSenderRawProfileImage = this.data
                    .Users
                    .Include(x => x.ProfileImage)
                    .Where(u => u.Id == currentSenderId)
                    .Select(u => u.ProfileImage.Path)
                    .FirstOrDefault();

                    var currentSenderProfileImage = currentSenderRawProfileImage.Replace("res", string.Empty);

                    senderProfilesImages.Add(currentSenderProfileImage);
                }
            }

            return senderProfilesImages;
        }
    }
}
