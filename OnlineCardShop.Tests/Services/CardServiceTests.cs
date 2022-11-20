namespace OnlineCardShop.Tests.Services
{
    using OnlineCardShop.Data.Models;
    using OnlineCardShop.Services.Cards;
    using OnlineCardShop.Tests.Mocks;
    using Xunit;

    public class CardServiceTests
    {
        [Theory]
        [InlineData("testImageName", "testImagePathForDb", "testOriginalImageName")]
        public void CreateImageShouldReturnAnImage(string imageName,
            string imagePathForDb,
            string originalImageName)
        {
            // Arrange
            using var data = DatabaseMock.Instance;

            data.SaveChanges();

            var cardService = new CardService(data);

            // Act
            var result = cardService.CreateImage(imageName, imagePathForDb, originalImageName);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Name == imageName);
            Assert.True(result.Path == imagePathForDb);
            Assert.True(result.OriginalName == originalImageName);
            Assert.IsType<Image>(result);
        }

        [Theory]
        [InlineData("testImageName", "testImagePathForDb", "testOriginalImageName",
            "testTitle", 2.0, "testDescription", 1, 1, 1)]
        public void CreateCardShouldReturnACard(string imageName,
            string imagePathForDb,
            string originalImageName,
            string title, 
            double price, 
            string description, 
            int categoryId, 
            int conditionId, 
            int dealerId)
        {
            // Arrange
            using var data = DatabaseMock.Instance;

            var image = new Image
            {
                Name = imageName,
                Path = imagePathForDb,
                OriginalName = originalImageName
            };

            data.SaveChanges();

            var cardService = new CardService(data);

            // Act
            var result = cardService.CreateCard(title, price, description, categoryId,
                conditionId, dealerId, image);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Title == title);
            Assert.True(result.Price == price);
            Assert.True(result.Description == description);
            Assert.True(result.CategoryId == categoryId);
            Assert.True(result.ConditionId == conditionId);
            Assert.True(result.DealerId == dealerId);
            Assert.IsType<Card>(result);
        }
    }
}
