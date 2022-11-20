namespace OnlineCardShop.Tests.Services
{
    using OnlineCardShop.Data.Models;
    using OnlineCardShop.Services.Chats;
    using OnlineCardShop.Services.Users;
    using OnlineCardShop.Tests.Mocks;
    using System;
    using System.Linq;
    using Xunit;
    using static TestConstants;

    public class UserServiceTests
    {
        [Theory]
        [InlineData(testUserId)]
        public void GetUserShouldReturnAUserWhenGivenAnId(string userId)
        {
            // Arrange
            using var data = DatabaseMock.Instance;

            data.Users.Add(new User { Id = userId });

            data.SaveChanges();

            var chatService = new ChatService(data);
            var userService = new UserService(data, chatService);

            // Act
            var result = userService.GetUser(userId);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Id == userId);
        }

        [Theory]
        [InlineData(testUserId)]
        public void GetUserFullNameShouldReturnCorrectUserFullNameWhenGivenAnId(string userId)
        {
            // Arrange
            using var data = DatabaseMock.Instance;
            data.Users.Add(new User { Id = userId, FullName = "TestFullName" });

            data.SaveChanges();

            var chatService = new ChatService(data);
            var userService = new UserService(data, chatService);

            // Act
            var result = userService.GetUserFullName(userId);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.True(result == "TestFullName");
        }

        [Theory]
        [InlineData(testUserId)]
        public void GetUserDetailsShouldReturnUserDetailsServiceViewModel(string userId)
        {
            // Arrange
            using var data = DatabaseMock.Instance;

            var user = new User();
            user.Id = userId;
            user.FullName = "TestFullName";
            user.ProfileImage = new ProfileImage { Path = "resTestPath" };
            user.AboutMe = "testAboutMe";

            data.Users.Add(user);

            data.SaveChanges();

            var chatService = new ChatService(data);
            var userService = new UserService(data, chatService);

            // Act
            var result = userService.GetUserDetails(userId);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<UserDetailsServiceViewModel>(result);
            Assert.True(result.Id == userId);
            Assert.True(result.FullName == "TestFullName");
            Assert.True(result.ProfileImage == "TestPath");
            Assert.True(result.AboutMe == "testAboutMe");
        }

        [Theory]
        [InlineData(testUserId)]
        public void GetUserDetailsShouldReturnNullWhenNoSuchUser(string userId)
        {
            // Arrange
            using var data = DatabaseMock.Instance;

            data.SaveChanges();

            var chatService = new ChatService(data);
            var userService = new UserService(data, chatService);

            // Act
            var result = userService.GetUserDetails(userId);

            // Assert
            Assert.Null(result);
        }

        [Theory]
        [InlineData(testUserId, "testConnectionId")]
        public void SaveConnectionIdShouldSaveTheGivenConnectionIdToTheGivenUser(string userId,
            string connectionId)
        {
            // Arrange
            using var data = DatabaseMock.Instance;

            data.Users.Add(new User { Id = userId });

            data.SaveChanges();

            var chatService = new ChatService(data);
            var userService = new UserService(data, chatService);

            // Act
            userService.SaveConnectionId(userId, connectionId);

            // Assert
            Assert.True(data.Users.Contains(new User { Id = userId, ConnectionId = connectionId }));
        }

        [Theory]
        [InlineData(testUserId)]
        public void UserHasConnectionIdSavedShouldReturnTrueIfThereIsConnectionId(string userId)
        {
            // Arrange
            using var data = DatabaseMock.Instance;

            data.Users.Add(new User { Id = userId, ConnectionId = "TestConnectionId" });

            data.SaveChanges();

            var chatService = new ChatService(data);
            var userService = new UserService(data, chatService);

            // Act
            var result = userService.UserHasConnectionIdSaved(userId);

            // Assert
            Assert.NotNull(result);
            Assert.True(result);
        }

        [Theory]
        [InlineData(testUserId)]
        public void UserHasConnectionIdSavedShouldReturnFalseIfThereIsNotConnectionId(string userId)
        {
            // Arrange
            using var data = DatabaseMock.Instance;

            data.Users.Add(new User { Id = userId});

            data.SaveChanges();

            var chatService = new ChatService(data);
            var userService = new UserService(data, chatService);

            // Act
            var result = userService.UserHasConnectionIdSaved(userId);

            // Assert
            Assert.True(!result);
        }

        [Theory]
        [InlineData(testUserId)]
        public void GetUserConnectionIdShouldReturnConnectionIdIfTheUserHasIt(string userId)
        {
            // Arrange
            using var data = DatabaseMock.Instance;

            data.Users.Add(new User { Id = userId, ConnectionId = "TestConnectionId" });

            data.SaveChanges();

            var chatService = new ChatService(data);
            var userService = new UserService(data, chatService);

            // Act
            var result = userService.GetUserConnectionId(userId);

            // Assert
            Assert.NotNull(result);
            Assert.True(result == "TestConnectionId");
        }

        [Theory]
        [InlineData(testUserId)]
        public void GetUserConnectionIdShouldNullTheUserDoesNotHaveConnectionId(string userId)
        {
            // Arrange
            using var data = DatabaseMock.Instance;

            data.Users.Add(new User { Id = userId });

            data.SaveChanges();

            var chatService = new ChatService(data);
            var userService = new UserService(data, chatService);

            // Act
            var result = userService.GetUserConnectionId(userId);

            // Assert
            Assert.Null(result);
        }

        [Theory]
        [InlineData("testImageName", "testImagePath", "testOriginalImageName")]
        public void CreateProfileImageShouldReturnProfileImage(string imageName,
            string imagePathForDb,
            string originalImageName)
        {
            // Arrange
            using var data = DatabaseMock.Instance;

            var chatService = new ChatService(data);
            var userService = new UserService(data, chatService);

            // Act
            var result = userService.CreateProfileImage(imageName, imagePathForDb, originalImageName);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<ProfileImage>(result);
        }

        [Fact]
        public void AddProfileImageToDBShouldAddTheProfileImageToDB()
        {
            // Arrange
            using var data = DatabaseMock.Instance;

            var profileImage = new ProfileImage { Id = 1 };

            data.SaveChanges();

            var chatService = new ChatService(data);
            var userService = new UserService(data, chatService);

            // Act
            userService.AddProfileImageToDB(profileImage);

            // Assert
            Assert.True(data.ProfileImages.Contains(new ProfileImage { Id = 1 }));
        }

        [Theory]
        [InlineData(testUserId)]
        public void IdOfReceiverOfMostRecentChat(string userId)
        {
            // Arrange
            using var data = DatabaseMock.Instance;

            var user = new User();
            user.Id = userId;

            var chat = new Chat();
            chat.Id = 1;
            chat.Users.Add(user);
            chat.Users.Add(new User { Id = "otherId" });
            chat.Messages.Add(new Message
            {
                Content = "testMessage",
                CreatedAt = DateTime.UtcNow.AddDays(1)
            });

            data.Users.Add(user);

            data.Chats.Add(chat);

            data.SaveChanges();

            var chatService = new ChatService(data);
            var userService = new UserService(data, chatService);

            // Act
            var result = userService.IdOfReceiverOfMostRecentChat(userId);

            // Assert
            Assert.NotNull(result);
        }
    }
}
