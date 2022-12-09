namespace OnlineCardShop.Tests.Services
{
    using Microsoft.AspNetCore.Cors.Infrastructure;
    using Microsoft.EntityFrameworkCore;
    using OnlineCardShop.Data;
    using OnlineCardShop.Data.Models;
    using OnlineCardShop.Services.Cards;
    using OnlineCardShop.Tests.Mocks;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Xunit;
    using Xunit.Sdk;

    public class CardServiceTests
    {

        private IQueryable<Card> GetTestCards()
        {
            return new List<Card>
            {
                new Card
                {
                    Id = 1,
                    Title = "Test Card 1",
                    Price = 10,
                    Description = "Test Description 1",
                    CategoryId = 1,
                    ConditionId = 1,
                    DealerId = 1,
                    Image = new Image { Name = "Test Name 1" },
                    IsPublic = true,
                    IsDeleted = false
                },
                new Card
                {
                    Id = 2,
                    Title = "Test Card 2",
                    Price = 15,
                    Description = "Test Description 2",
                    CategoryId = 2,
                    ConditionId = 2,
                    DealerId = 2,
                    Image = new Image { Name = "Test Name 2" },
                    IsPublic = true,
                    IsDeleted = false
                },
                new Card
                {
                    Id = 3,
                    Title = "Test Card 3",
                    Price = 20,
                    Description = "Test Description 3",
                    CategoryId = 2,
                    ConditionId = 1,
                    DealerId = 3,
                    Image = new Image { Name = "Test Name 3" },
                    IsPublic = false,
                    IsDeleted = true
                }
            }.AsQueryable();
        }

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

        [Fact]
        public void DeleteCard_CardIsDeleted()
        {
            //Arrange
            using var data = DatabaseMock.Instance;

            var testCard = new Card();
            testCard.Id = 1;
            testCard.IsDeleted = false;
            testCard.IsPublic = true;

            data.Cards.Add(testCard);

            data.SaveChanges();

            var cardService = new CardService(data);


            //Act
            cardService.DeleteCard(testCard.Id);

            //Assert
            var card = data.Cards.Where(c => c.Id == testCard.Id).FirstOrDefault();
            Assert.False(card.IsPublic);
            Assert.True(card.IsDeleted);
        }

        [Fact]
        public void DeleteCardCardDoesNotExistThrowsException()
        {
            //Arrange
            using var data = DatabaseMock.Instance;
            var cardId = 99;
            var cardService = new CardService(data);


            //Act and Assert
            Assert.Throws<NullReferenceException>(() => cardService.DeleteCard(cardId));
        }
    }
}
