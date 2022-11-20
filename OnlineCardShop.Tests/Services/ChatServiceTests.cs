namespace OnlineCardShop.Tests.Services
{
    using OnlineCardShop.Data.Models;
    using OnlineCardShop.Services.Chats;
    using OnlineCardShop.Tests.Mocks;
    using System;
    using System.Linq;
    using Xunit;

    using static TestConstants;

    public class ChatServiceTests
    {
        [Theory]
        [InlineData(testChatName)]
        public void ChatExistsShouldReturnTrueWhenChatExists(string chatName)
        {
            // Arrange
            using var data = DatabaseMock.Instance;

            data.Chats.Add(new Chat { Name = chatName });

            data.SaveChanges();

            var chatService = new ChatService(data);

            // Act
            var result = chatService.ChatExists(chatName);

            // Assert
            Assert.NotNull(result);
            Assert.True(result);
        }

        [Theory]
        [InlineData(testChatName)]

        public void ChatExistsShouldReturnFalseWhenChatDoesNotExist(string chatName)
        {
            // Arrange
            using var data = DatabaseMock.Instance;

            data.Chats.Add(new Chat { Name = chatName });

            data.SaveChanges();

            var chatService = new ChatService(data);

            // Act
            var result = chatService.ChatExists("incorrectTestChat");

            // Assert
            Assert.NotNull(result);
            Assert.False(result);
        }

        [Theory]
        [InlineData(testUserId, testChatName)]
        public void UserIsInChatShouldReturnTrueWhenUserAndChatExist(string userId,
            string chatName)
        {
            // Arrange
            using var data = DatabaseMock.Instance;

            var chat = new Chat 
            { 
                Id = 1, 
                Name = chatName 
            };

            var user = new User();
            user.Id = userId;
            user.Chats.Add(chat);

            data.Chats.Add(chat);
            data.Users.Add(user);

            data.SaveChanges();

            var chatService = new ChatService(data);

            // Act
            var result = chatService.UserIsInChat(userId, chatName);

            // Assert
            Assert.True(result);
        }

        [Theory]
        [InlineData(testUserId, testChatName)]
        public void UserIsInChatShouldReturnFalseWhenUserIsNotInTheChat(string userId,
            string chatName)
        {
            // Arrange
            using var data = DatabaseMock.Instance;

            var chat = new Chat
            {
                Id = 1,
                Name = chatName
            };

            var user = new User();
            user.Id = userId;
            user.Chats.Add(chat);

            data.Chats.Add(chat);
            data.Users.Add(user);

            data.SaveChanges();

            var chatService = new ChatService(data);

            // Act
            var result = chatService.UserIsInChat(userId, "incorrectTestChat");

            // Assert
            Assert.False(result);
        }

        [Theory]
        [InlineData(testChatName)]
        public void AddChatShouldAddChat(string chatName)
        {
            // Arrange
            using var data = DatabaseMock.Instance;

            var chatService = new ChatService(data);

            // Act
            chatService.AddChat(chatName);

            var result = data.Chats.Any(c => c.Name == chatName);

            // Assert
            Assert.True(result);
        }

        [Theory]
        [InlineData(testUserId, testChatName)]
        public void AddUserToChatShouldAddUserToTheChat(string userId,
            string chatName)
        {
            // Arrange
            using var data = DatabaseMock.Instance;

            data.Chats.Add(new Chat { Id = 1, Name = chatName });
            data.Users.Add(new User { Id = userId });

            data.SaveChanges();

            var chatService = new ChatService(data);

            // Act
            chatService.AddUserToChat(userId, chatName);

            var result = data.Chats
                .Where(c => c.Name == chatName)
                .Select(c => c.Users)
                .Where(u => u.Contains(new User { Id = userId }))
                .FirstOrDefault();

            // Assert
            Assert.NotNull(result);
        }

        [Theory]
        [InlineData(testChatName, testUserId, "testMessageContent")]
        public void SaveMessageShouldSaveMessageToChatAndTheUser(string chatName,
            string userId,
            string content)
        {
            // Arrange
            using var data = DatabaseMock.Instance;

            data.Chats.Add(new Chat 
            { 
                Id = 1,
                Name = chatName
            });

            data.SaveChanges();

            var chatService = new ChatService(data);

            // Act
            chatService.SaveMessage(chatName, userId, content);

            var result = data.Chats
                .Where(c => c.Name == chatName)
                .Select(c => c.Messages)
                .Where(m => m.Any(m => m.ChatId == 1 && m.Content == content))
                .FirstOrDefault();

            // Assert
            Assert.NotNull(result);
        }

        [Theory]
        [InlineData(testChatName)]
        public void GetMessageHistoryShouldReturnMessages(string chatName)
        {
            // Arrange
            using var data = DatabaseMock.Instance;

            data.Chats.Add(new Chat { Id = 1, Name = chatName });

            data.Messages.Add(new Message { ChatId = 1 });

            data.SaveChanges();

            var chatService = new ChatService(data);

            // Act
            var result = chatService.GetMessagesHistory(chatName);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Count() > 0);
        }

        [Theory]
        [InlineData(testChatName)]
        public void GetMessageHistoryShouldReturnNull(string chatName)
        {
            // Arrange
            using var data = DatabaseMock.Instance;

            data.Chats.Add(new Chat { Id = 1, Name = chatName });

            data.SaveChanges();

            var chatService = new ChatService(data);

            // Act
            var result = chatService.GetMessagesHistory(chatName);

            // Assert
            Assert.Null(result);
        }

        [Theory]
        [InlineData(testChatName)]
        public void GetLastMessageShouldReturnLastMessage(string chatName)
        {
            // Arrange
            using var data = DatabaseMock.Instance;

            data.Chats.Add(new Chat { Id = 1, Name = chatName });

            data.Messages.Add(new Message { Id = 1, ChatId = 1 });

            data.Messages.Add(new Message { Id = 2, ChatId = 1 });

            data.SaveChanges();

            var chatService = new ChatService(data);

            // Act
            var result = chatService.GetLastMessage(chatName);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Id == 2);
        }

        [Theory]
        [InlineData(testChatName)]
        public void GetLastMessageShouldReturnNull(string chatName)
        {
            // Arrange
            using var data = DatabaseMock.Instance;

            data.Chats.Add(new Chat { Id = 1, Name = chatName });

            data.SaveChanges();

            var chatService = new ChatService(data);

            // Act
            var result = chatService.GetLastMessage(chatName);

            // Assert
            Assert.Null(result);
        }

        [Theory]
        [InlineData(testUserId)]
        public void RetrieveRecentChatsShouldRetrieveTheRecentChatsInDescendingOrder(string userId)
        {
            // Arrange
            using var data = DatabaseMock.Instance;

            var user = new User();
            user.Id = userId;

            var chat = new Chat();
            chat.Id = 1;
            chat.Users.Add(user);
            chat.Messages.Add(new Message 
            { 
                Content = "testMessage",
                CreatedAt = DateTime.UtcNow.AddDays(1)
            });

            var chat2 = new Chat();
            chat.Id = 2;
            chat.Users.Add(user);
            chat.Messages.Add(new Message
            {
                Content = "testMessage2",
                CreatedAt = DateTime.UtcNow.AddDays(2)
            });

            var chat3 = new Chat();
            chat.Id = 3;
            chat.Users.Add(user);
            chat.Messages.Add(new Message
            {
                Content = "testMessage3",
                CreatedAt = DateTime.UtcNow.AddDays(3)
            });

            data.Users.Add(user);

            data.Chats.Add(chat);
            data.Chats.Add(chat2);
            data.Chats.Add(chat3);

            data.SaveChanges();

            var chatService = new ChatService(data);

            // Act
            var result = chatService.RetrieveRecentChats(userId);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.FirstOrDefault().Id == 3);
        }

        [Theory]
        [InlineData(testChatName)]
        public void GetChatIdShouldReturnCorrectChatId(string chatName)
        {
            // Arrange
            using var data = DatabaseMock.Instance;

            data.Chats.Add(new Chat { Id = 1, Name = chatName });

            data.SaveChanges();

            var chatService = new ChatService(data);

            // Act
            var result = chatService.GetChatId(chatName);

            // Assert
            Assert.NotNull(result);
            Assert.True(result == 1);
        }

        [Theory]
        [InlineData(1)]
        public void GetDateTimeOfLastMessageShouldReturnTheDateTimeOfLastMessageInChat(int chatId)
        {
            // Arrange
            using var data = DatabaseMock.Instance;

            var message = new Message { Id = 1, ChatId = chatId, CreatedAt = DateTime.UtcNow };
            var message2 = new Message { Id = 2, ChatId = chatId, CreatedAt = DateTime.UtcNow.AddDays(1) };

            data.Messages.Add(message);
            data.Messages.Add(message2);

            data.SaveChanges();

            var chatService = new ChatService(data);

            // Act
            var result = chatService.GetDateTimeOfLastMessage(chatId);

            // Assert
            Assert.NotNull(result);
            Assert.True(result == message2.CreatedAt);
        }

        [Fact]
        public void GetMessageSentTimeAgoShouldReturnXSecondsAgo()
        {
            // Arrange
            using var data = DatabaseMock.Instance;

            var message = new Message { CreatedAt = DateTime.UtcNow, ChatId = 1 };

            data.Messages.Add(message);

            data.SaveChanges();

            var chatService = new ChatService(data);

            // Act
            var result = chatService.GetMessageSentTimeAgo(new Chat { Id = 1 });
            var resultSplit = result.Split(" ");

            // Assert
            Assert.True(resultSplit[1] == "seconds");
        }

        [Fact]
        public void GetMessageSentTimeAgoShouldReturnAMinuteAgo()
        {
            // Arrange
            using var data = DatabaseMock.Instance;

            var message = new Message { CreatedAt = DateTime.UtcNow.AddMinutes(-1.5), ChatId = 1 };

            data.Messages.Add(message);

            data.SaveChanges();

            var chatService = new ChatService(data);

            // Act
            var result = chatService.GetMessageSentTimeAgo(new Chat { Id = 1 });

            // Assert
            Assert.True(result == "a minute ago");
        }

        [Fact]
        public void GetMessageSentTimeAgoShouldReturnXMinutesAgo()
        {
            // Arrange
            using var data = DatabaseMock.Instance;

            var message = new Message { CreatedAt = DateTime.UtcNow.AddMinutes(-3), ChatId = 1 };

            data.Messages.Add(message);

            data.SaveChanges();

            var chatService = new ChatService(data);

            // Act
            var result = chatService.GetMessageSentTimeAgo(new Chat { Id = 1 });
            var resultSplit = result.Split(" ");

            // Assert
            Assert.True(resultSplit[1] == "minutes");
        }

        [Fact]
        public void GetMessageSentTimeAgoShouldReturnAnHourAgo()
        {
            // Arrange
            using var data = DatabaseMock.Instance;

            var message = new Message { CreatedAt = DateTime.UtcNow.AddHours(-1.1), ChatId = 1 };

            data.Messages.Add(message);

            data.SaveChanges();

            var chatService = new ChatService(data);

            // Act
            var result = chatService.GetMessageSentTimeAgo(new Chat { Id = 1 });

            // Assert
            Assert.True(result == "an hour ago");
        }

        [Fact]
        public void GetMessageSentTimeAgoShouldReturnXHoursAgo()
        {
            // Arrange
            using var data = DatabaseMock.Instance;

            var message = new Message { CreatedAt = DateTime.UtcNow.AddHours(-3), ChatId = 1 };

            data.Messages.Add(message);

            data.SaveChanges();

            var chatService = new ChatService(data);

            // Act
            var result = chatService.GetMessageSentTimeAgo(new Chat { Id = 1 });
            var resultSplit = result.Split(" ");

            // Assert
            Assert.True(resultSplit[1] == "hours");
        }

        [Fact]
        public void GetMessageSentTimeAgoShouldReturnYesterday()
        {
            // Arrange
            using var data = DatabaseMock.Instance;

            var message = new Message { CreatedAt = DateTime.UtcNow.AddDays(-1.1), ChatId = 1 };

            data.Messages.Add(message);

            data.SaveChanges();

            var chatService = new ChatService(data);

            // Act
            var result = chatService.GetMessageSentTimeAgo(new Chat { Id = 1 });

            // Assert
            Assert.True(result == "yesterday");
        }

        [Fact]
        public void GetMessageSentTimeAgoShouldReturnXDaysAgo()
        {
            // Arrange
            using var data = DatabaseMock.Instance;

            var message = new Message { CreatedAt = DateTime.UtcNow.AddDays(-3), ChatId = 1 };

            data.Messages.Add(message);

            data.SaveChanges();

            var chatService = new ChatService(data);

            // Act
            var result = chatService.GetMessageSentTimeAgo(new Chat { Id = 1 });
            var resultSplit = result.Split(" ");

            // Assert
            Assert.True(resultSplit[1] == "days");
        }

        [Fact]
        public void GetMessageSentTimeAgoShouldReturnOneMonthAgo()
        {
            // Arrange
            using var data = DatabaseMock.Instance;

            var message = new Message { CreatedAt = DateTime.UtcNow.AddMonths(-1), ChatId = 1 };

            data.Messages.Add(message);

            data.SaveChanges();

            var chatService = new ChatService(data);

            // Act
            var result = chatService.GetMessageSentTimeAgo(new Chat { Id = 1 });

            // Assert
            Assert.True(result == "one month ago");
        }

        [Fact]
        public void GetMessageSentTimeAgoShouldReturnXMonthsAgo()
        {
            // Arrange
            using var data = DatabaseMock.Instance;

            var message = new Message { CreatedAt = DateTime.UtcNow.AddMonths(-3), ChatId = 1 };

            data.Messages.Add(message);

            data.SaveChanges();

            var chatService = new ChatService(data);

            // Act
            var result = chatService.GetMessageSentTimeAgo(new Chat { Id = 1 });
            var resultSplit = result.Split(" ");

            // Assert
            Assert.True(resultSplit[1] == "months");
        }

        [Fact]
        public void GetMessageSentTimeAgoShouldReturnOneYearAgo()
        {
            // Arrange
            using var data = DatabaseMock.Instance;

            var message = new Message { CreatedAt = DateTime.UtcNow.AddYears(-1), ChatId = 1 };

            data.Messages.Add(message);

            data.SaveChanges();

            var chatService = new ChatService(data);

            // Act
            var result = chatService.GetMessageSentTimeAgo(new Chat { Id = 1 });

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.True(result == "one year ago");
        }

        [Fact]
        public void GetMessageSentTimeAgoShouldReturnXYearsAgo()
        {
            // Arrange
            using var data = DatabaseMock.Instance;

            var message = new Message { CreatedAt = DateTime.UtcNow.AddYears(-3), ChatId = 1 };

            data.Messages.Add(message);

            data.SaveChanges();

            var chatService = new ChatService(data);

            // Act
            var result = chatService.GetMessageSentTimeAgo(new Chat { Id = 1 });
            var resultSplit = result.Split(" ");

            // Assert
            Assert.NotNull(resultSplit[1]);
            Assert.NotEmpty(resultSplit[1]);
            Assert.True(resultSplit[1] == "years");
        }
    }
}
