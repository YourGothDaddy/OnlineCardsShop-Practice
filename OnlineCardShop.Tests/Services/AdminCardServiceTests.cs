namespace OnlineCardShop.Tests.Services
{
    using OnlineCardShop.Areas.Admin.Services;
    using OnlineCardShop.Data.Models;
    using OnlineCardShop.Tests.Mocks;
    using System.Linq;
    using Xunit;

    public class AdminCardServiceTests
    {
        [Fact]
        public void AllReturnsAllTheExpectedCard()
        {
            // Arrange
            using var data = DatabaseMock.Instance;

            data.Cards.Add(new Card
            {
                Id = 1,
                Title = "testTitle",
                DealerId = 1,
                IsDeleted = false,
                IsPublic = true
            });

            data.Cards.Add(new Card
            {
                Id = 2,
                Title = "testTitle2",
                DealerId = 2,
                IsDeleted = false,
                IsPublic = true
            });

            data.SaveChanges();

            var service = new AdminCardService(data);

            // Act
            var result = service.All(1, 2);

            var cardsAreCorrect = result.Cards.FirstOrDefault().Id == 1 && 
                result.Cards.LastOrDefault().Id == 2;

            var cardsCountIsTwo = result.TotalCards == 2;

            var currentPageIsOne = result.CurrentPage == 1;

            // Assert
            Assert.True(cardsAreCorrect);
            Assert.True(cardsCountIsTwo);
            Assert.True(currentPageIsOne);
        }

        [Fact]
        public void ApproveCardChangesTheCardToPublic()
        {
            // Arrange
            using var data = DatabaseMock.Instance;

            data.Cards.Add(new Card
            {
                Id = 1,
                IsPublic = false
            });

            data.SaveChanges();

            var service = new AdminCardService(data);

            // Act
            service.ApproveCard(1);

            var cardIsPublic = data.Cards
                .Where(c => c.Id == 1)
                .FirstOrDefault().IsPublic == true;

            // Assert
            Assert.True(cardIsPublic);
        }

        [Fact]
        public void DeleteCardChangesTheCardToDeleted()
        {
            // Arrange
            using var data = DatabaseMock.Instance;

            data.Cards.Add(new Card
            {
                Id = 1,
                IsPublic = true,
                IsDeleted = false
            });

            data.SaveChanges();

            var service = new AdminCardService(data);

            // Act
            service.DeleteCard(1);

            var cardIsDeletedAndNotPublic = data.Cards
                .Where(c => c.Id == 1)
                .FirstOrDefault().IsPublic == false &&
                data.Cards
                .Where(c => c.Id == 1)
                .FirstOrDefault().IsDeleted == true;

            // Assert
            Assert.True(cardIsDeletedAndNotPublic);
        }

        [Fact]
        public void HideCardChangesTheCardToNotPublic()
        {
            // Arrange
            using var data = DatabaseMock.Instance;

            data.Cards.Add(new Card
            {
                Id = 1,
                IsPublic = true
            });

            data.SaveChanges();

            var service = new AdminCardService(data);

            // Act
            service.HideCard(1);

            var cardIsNotPublic = data.Cards
                .Where(c => c.Id == 1)
                .FirstOrDefault().IsPublic == false;

            // Assert
            Assert.True(cardIsNotPublic);
        }
    }
}
