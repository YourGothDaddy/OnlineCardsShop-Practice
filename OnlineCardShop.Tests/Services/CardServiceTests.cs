namespace OnlineCardShop.Tests.Services
{
    using OnlineCardShop.Data.Models;
    using OnlineCardShop.Data.Models.Enums;
    using OnlineCardShop.Services.Cards;
    using OnlineCardShop.Tests.Mocks;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Xunit;

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

        [Fact]
        public void EditCardUpdatesCardData()
        {
            // Arrange
            using var data = DatabaseMock.Instance;
            var service = new CardService(data);
            var id = 1;
            var title = "Test Title";
            var price = 9.99;
            var description = "Test Description";
            var categoryId = 1;
            var conditionId = 1;
            var newImage = new Image();

            data.Cards.Add(new Card
            {
                Id = id,
                Title = title,
                Price = price,
                Description = description,
                CategoryId = 1,
                ConditionId = 1,
                Image = newImage
            });

            // Act
            var result = service.EditCard(
                id,
                "NewTitle",
                price,
                description,
                categoryId,
                conditionId,
                newImage);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void EditCardReturnFalseWhenCardDataIsNull()
        {
            //Arrange
            using var data = DatabaseMock.Instance;
            var cardService = new CardService(data);
            var id = 1;
            var title = "Test Title";
            var price = 9.99;
            var description = "Test Description";
            var categoryId = 1;
            var conditionId = 1;
            var newImage = new Image();

            //Act
            var result = cardService.EditCard(
                id,
                title,
                price,
                description,
                categoryId,
                conditionId,
                newImage);

            //Assert
            Assert.False(result);
        }

        [Fact]
        public void EditCardEditsTheImageWhenNewImageIsNotNull()
        {
            //Arrange
            using var data = DatabaseMock.Instance;
            var cardService = new CardService(data);

            var id = 1;
            var title = "Test Title";
            var price = 9.99;
            var description = "Test Description";
            var categoryId = 1;
            var conditionId = 1;
            var newImage = new Image();
            newImage.Id = 1;
            newImage.Path = "testPath";
            newImage.Name = "testName";
            newImage.OriginalName = "testOriginalname";
            newImage.CardId = 1;

            var card = new Card
            {
                Id = id,
                Title = title,
                Price = price,
                Description = description,
                CategoryId = categoryId,
                ConditionId = conditionId,
                Image = newImage
            };

            data.Cards.Add(card);

            data.SaveChanges();

            var otherImage = new Image();
            otherImage.Id = 2;
            otherImage.Path = "testPath";
            otherImage.Name = "testName";
            otherImage.OriginalName = "testOriginalname";
            otherImage.CardId = 1;
            otherImage.Card = card;

            data.Images.Add(otherImage);
            data.SaveChanges();

            //Act
            var resultEditCardIsSuccessful = cardService.EditCard(
                id,
                title,
                price,
                description,
                categoryId,
                conditionId,
                otherImage);

            var resultCard = data.Cards.Where(c => c.Id == id).FirstOrDefault();

            var result = resultCard.Image.Id == otherImage.Id;

            //Assert
            Assert.True(result);
            Assert.True(resultEditCardIsSuccessful);
        }

        [Fact]
        public void AllReturnsCorrectTotalNumberOfKpopCards()
        {
            // Arrange
            using var data = DatabaseMock.Instance;

            data.Cards.Add(new Card
            {
                Id = 1,
                IsPublic = true,
                IsDeleted = false,
                CategoryId = 1,
                Condition = new Condition
                {
                    Id = 1,
                    Name = "test"
                }
            });

            data.Cards.Add(new Card
            {
                Id = 2,
                IsPublic = true,
                IsDeleted = false,
                CategoryId = 1,
                Condition = new Condition{
                    Id = 2,
                    Name = "test"
                }
            });

            data.SaveChanges();

            var cardService = new CardService(data);
            var sorting = CardSorting.Condition;
            var currentPage = 1;
            var cardsPerPage = 2;
            var categoryId = 1;

            // Act
            var result = cardService.All(
                null,
                sorting,
                currentPage,
                cardsPerPage,
                categoryId,
                null);

            // Assert
            Assert.Equal(2, result.TotalCards);
            Assert.NotNull(result);
        }

        [Fact]
        public void AllReturnsCorrectTotalNumberOfGameCards()
        {
            // Arrange
            using var data = DatabaseMock.Instance;

            data.Cards.Add(new Card
            {
                Id = 1,
                IsPublic = true,
                IsDeleted = false,
                CategoryId = 2,
                Condition = new Condition
                {
                    Id = 1,
                    Name = "test"
                }
            });

            data.Cards.Add(new Card
            {
                Id = 2,
                IsPublic = true,
                IsDeleted = false,
                CategoryId = 2,
                Condition = new Condition
                {
                    Id = 2,
                    Name = "test"
                }
            });

            data.SaveChanges();

            var cardService = new CardService(data);
            var sorting = CardSorting.Condition;
            var currentPage = 1;
            var cardsPerPage = 2;
            var categoryId = 2;

            // Act
            var result = cardService.All(
                null,
                sorting,
                currentPage,
                cardsPerPage,
                categoryId,
                null);

            // Assert
            Assert.Equal(2, result.TotalCards);
            Assert.NotNull(result);
        }

        [Fact]
        public void AllReturnsEmptyListWhenNoMatchingCardsFound()
        {
            // Arrange
            using var data = DatabaseMock.Instance;
            var cardService = new CardService(data);
            var searchTerm = "Test";
            var sorting = CardSorting.Condition;
            var currentPage = 1;
            var cardsPerPage = 10;
            var categoryId = 3;
            var order = SortingOrder.BestToWorse;

            // Act
            var result = cardService.All(
                searchTerm,
                sorting,
                currentPage,
                cardsPerPage,
                categoryId,
                order);

            // Assert
            Assert.Empty(result.Cards);
        }

        [Fact]
        public void AllReturnsCardsSortedByCategory()
        {
            // Arrange
            using var data = DatabaseMock.Instance;

            data.Cards.Add(new Card
            {
                Id = 1,
                Title = "Test",
                Description = "Test",
                Price = 0,
                Image = new Image(),
                IsPublic = true,
                IsDeleted = false,
                CategoryId = 0,
                ConditionId = 0,
                Category = new Category(),
                Condition = new Condition()
            });

            data.Cards.Add(new Card
            {
                Id = 2,
                Title = "Test2",
                Description = "Test2",
                Price = 02,
                Image = new Image(),
                IsPublic = true,
                IsDeleted = false,
                CategoryId = 1,
                ConditionId = 1,
                Category = new Category(),
                Condition = new Condition()
            });

            data.SaveChanges();

            var cardService = new CardService(data);
            var sorting = CardSorting.Category;
            var currentPage = 1;
            var cardsPerPage = 2;

            // Act
            var cards = cardService.All(
                null,
                sorting,
                currentPage,
                cardsPerPage,
                null,
                null);

            var result = cards.Cards.FirstOrDefault().Id == 2;

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void AllReturnsCardsSortedByCondition()
        {
            // Arrange
            using var data = DatabaseMock.Instance;

            data.Cards.Add(new Card
            {
                Id = 1,
                Title = "Test",
                Description = "Test",
                Price = 0,
                Image = new Image(),
                IsPublic = true,
                IsDeleted = false,
                CategoryId = 1,
                ConditionId = 1,
                Category = new Category(),
                Condition = new Condition()
            });

            data.Cards.Add(new Card
            {
                Id = 2,
                Title = "Test2",
                Description = "Test2",
                Price = 02,
                Image = new Image(),
                IsPublic = true,
                IsDeleted = false,
                CategoryId = 0,
                ConditionId = 0,
                Category = new Category(),
                Condition = new Condition()
            });

            data.SaveChanges();

            var cardService = new CardService(data);
            var sorting = CardSorting.Condition;
            var currentPage = 1;
            var cardsPerPage = 2;

            // Act
            var cards = cardService.All(
                null,
                sorting,
                currentPage,
                cardsPerPage,
                null,
                null);

            var result = cards.Cards.FirstOrDefault().Id == 2;

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void AllReturnsCardsSortedByTitle()
        {
            // Arrange
            using var data = DatabaseMock.Instance;

            data.Cards.Add(new Card
            {
                Id = 1,
                Title = "Test",
                Description = "Test",
                Price = 0,
                Image = new Image(),
                IsPublic = true,
                IsDeleted = false,
                CategoryId = 0,
                ConditionId = 0,
                Category = new Category(),
                Condition = new Condition()
            });

            data.Cards.Add(new Card
            {
                Id = 2,
                Title = "Test2",
                Description = "Test2",
                Price = 02,
                Image = new Image(),
                IsPublic = true,
                IsDeleted = false,
                CategoryId = 1,
                ConditionId = 1,
                Category = new Category(),
                Condition = new Condition()
            });

            data.SaveChanges();

            var cardService = new CardService(data);
            var searchTerm = "2";
            var sorting = CardSorting.Condition;
            var currentPage = 1;
            var cardsPerPage = 2;

            // Act
            var cards = cardService.All(
                searchTerm,
                sorting,
                currentPage,
                cardsPerPage,
                null,
                null);

            var result = cards.Cards.Count() == 1;

            // Assert
            Assert.True(cards.Cards.FirstOrDefault().Id == 2);
            Assert.True(result);
        }

        [Fact]
        public void AllReturnsCardsSortedByCategoryAndBestToWorseOrder()
        {
            // Arrange
            using var data = DatabaseMock.Instance;

            var testCard = new Card
            {
                Id = 1,
                Title = "Test",
                Description = "Test",
                Price = 0,
                Image = new Image(),
                IsPublic = true,
                IsDeleted = false,
                CategoryId = 1,
                Category = new Category
                {
                    Name = "Kpop"
                },
                Condition = new Condition
                {
                    Id = 2,
                    Name = "BestToWorse"
                }
            };

            data.Cards.Add(testCard);

            var testCard2 = new Card
            {
                Id = 2,
                Title = "Test2",
                Description = "Test2",
                Price = 02,
                Image = new Image(),
                IsPublic = true,
                IsDeleted = false,
                CategoryId = 1,
                Category = new Category
                {
                    Name = "Kpop"
                },
                Condition = new Condition
                {
                    Name = "WorseToBest"
                }
            };

            data.Cards.Add(testCard2);

            data.SaveChanges();

            var cardService = new CardService(data);
            var sorting = CardSorting.Category;
            var currentPage = 1;
            var cardsPerPage = 2;
            var order = SortingOrder.BestToWorse;

            // Act
            var cards = cardService.All(
                null,
                sorting,
                currentPage,
                cardsPerPage,
                1,
                order);

            var result = cards.Cards.FirstOrDefault().Id == 1;

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void AllReturnsCardsSortedByCategoryAndWorseToBestOrder()
        {
            // Arrange
            using var data = DatabaseMock.Instance;

            var testCard = new Card
            {
                Id = 1,
                Title = "Test",
                Description = "Test",
                Price = 0,
                Image = new Image(),
                IsPublic = true,
                IsDeleted = false,
                CategoryId = 1,
                Category = new Category
                {
                    Name = "Kpop"
                },
                Condition = new Condition
                {
                    Id = 2,
                    Name = "WorseToBest"
                }
            };

            data.Cards.Add(testCard);

            var testCard2 = new Card
            {
                Id = 2,
                Title = "Test2",
                Description = "Test2",
                Price = 02,
                Image = new Image(),
                IsPublic = true,
                IsDeleted = false,
                CategoryId = 1,
                Category = new Category
                {
                    Name = "Kpop"
                },
                Condition = new Condition
                {
                    Name = "BestToWorst"
                }
            };

            data.Cards.Add(testCard2);

            data.SaveChanges();

            var cardService = new CardService(data);
            var sorting = CardSorting.Category;
            var currentPage = 1;
            var cardsPerPage = 2;
            var order = SortingOrder.BestToWorse;

            // Act
            var cards = cardService.All(
                null,
                sorting,
                currentPage,
                cardsPerPage,
                1,
                order);

            var result = cards.Cards.FirstOrDefault().Id == 1;

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void AllReturnsCardsSortedByConditionAndBestToWorseOrder()
        {
            // Arrange
            using var data = DatabaseMock.Instance;

            var testCard = new Card
            {
                Id = 1,
                Title = "Test",
                Description = "Test",
                Price = 0,
                Image = new Image(),
                IsPublic = true,
                IsDeleted = false,
                CategoryId = 1,
                Category = new Category
                {
                    Name = "Kpop"
                },
                Condition = new Condition
                {
                    Id = 2,
                    Name = "BestToWorse"
                }
            };

            data.Cards.Add(testCard);

            var testCard2 = new Card
            {
                Id = 2,
                Title = "Test2",
                Description = "Test2",
                Price = 02,
                Image = new Image(),
                IsPublic = true,
                IsDeleted = false,
                CategoryId = 1,
                Category = new Category
                {
                    Name = "Kpop"
                },
                Condition = new Condition
                {
                    Name = "WorseToBest"
                }
            };

            data.Cards.Add(testCard2);

            data.SaveChanges();

            var cardService = new CardService(data);
            var sorting = CardSorting.Category;
            var currentPage = 1;
            var cardsPerPage = 2;
            var order = SortingOrder.BestToWorse;

            // Act
            var cards = cardService.All(
                null,
                sorting,
                currentPage,
                cardsPerPage,
                1,
                order);

            var result = cards.Cards.FirstOrDefault().Id == 1;

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void CheckIfUserHasAccessShouldReturnTrueIfTheIdsAreTheSame()
        {
            // Arrange
            using var data = DatabaseMock.Instance;
            var card = new CardDetailsServiceModel()
            {
                UserId = "12345"
            };
            var requestingUserId = "12345";

            var cardService = new CardService(data);

            // Act
            var result = cardService.CheckIfUserHasAccess(card, requestingUserId);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void CheckIfUserHasAccessShouldReturnFalseIfTheIdsAreNotTheSame()
        {
            // Arrange
            using var data = DatabaseMock.Instance;
            var card = new CardDetailsServiceModel()
            {
                UserId = "12345"
            };
            var requestingUserId = "123456";

            var cardService = new CardService(data);

            // Act
            var result = cardService.CheckIfUserHasAccess(card, requestingUserId);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void CardIsByDealerShouldReturnTrueIfTheDealerHasACardWithTheGivenId()
        {
            // Arrange
            using var data = DatabaseMock.Instance;

            data.Cards.Add(new Card { Id = 1, DealerId = 2 });
            data.SaveChanges();

            var cardId = 1;
            var dealerId = 2;

            var cardService = new CardService(data);

            // Act
            var result = cardService.CardIsByDealer(cardId, dealerId);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void CardIsByDealerShouldReturnFalseIfTheDealerDoesntHaveACardWithTheGivenId()
        {
            // Arrange
            using var data = DatabaseMock.Instance;

            data.Cards.Add(new Card { Id = 2, DealerId = 2 });
            data.SaveChanges();

            var cardId = 1;
            var dealerId = 2;

            var cardService = new CardService(data);

            // Act
            var result = cardService.CardIsByDealer(cardId, dealerId);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void GetCardCategoriesReturnsCardCategoryServiceViewModel()
        {
            // Arrange
            using var data = DatabaseMock.Instance;

            var firstCategory = new Category() { Id = 1, Name = "Category1" };
            var secondCategory = new Category() { Id = 2, Name = "Category2" };

            data.Categories.Add(firstCategory);
            data.Categories.Add(secondCategory);

            data.SaveChanges();

            var cardService = new CardService(data);

            // Act
            var result = cardService.GetCardCategories();

            // Assert
            Assert.IsType<List<CardCategoryServiceViewModel>>(result);
        }

        [Fact]
        public void GetCardCategoriesReturnsCorrectCategories()
        {
            // Arrange
            using var data = DatabaseMock.Instance;

            var firstCategory = new Category() { Id = 1, Name = "Category1" };
            var secondCategory = new Category() { Id = 2, Name = "Category2" };

            data.Categories.Add(firstCategory);
            data.Categories.Add(secondCategory);

            data.SaveChanges();

            var cardService = new CardService(data);

            // Act
            var cards = cardService.GetCardCategories();

            var firstCardResult = cards.FirstOrDefault().Name == "Category1";
            var secondCardResult = cards.LastOrDefault().Name == "Category2";

            // Assert
            Assert.True(firstCardResult);
            Assert.True(secondCardResult);
        }

        [Fact]
        public void GetCardConditionsReturnsCardConditionServiceViewModel()
        {
            // Arrange
            using var data = DatabaseMock.Instance;

            var firstCondition = new Condition() { Id = 1, Name = "Condition1" };
            var secondCondition = new Condition() { Id = 2, Name = "Condition2" };

            data.Conditions.Add(firstCondition);
            data.Conditions.Add(secondCondition);

            data.SaveChanges();

            var cardService = new CardService(data);

            // Act
            var result = cardService.GetCardConditions();

            // Assert
            Assert.IsType<List<CardConditionServiceViewModel>>(result);
        }

        [Fact]
        public void GetCardConditionsReturnsCorrectConditions()
        {
            // Arrange
            using var data = DatabaseMock.Instance;

            var firstCondition = new Condition() { Id = 1, Name = "Condition1" };
            var secondCondition = new Condition() { Id = 2, Name = "Condition2" };

            data.Conditions.Add(firstCondition);
            data.Conditions.Add(secondCondition);

            data.SaveChanges();

            var cardService = new CardService(data);

            // Act
            var cards = cardService.GetCardConditions();

            var firstCardResult = cards.FirstOrDefault().Name == "Condition1";
            var secondCardResult = cards.LastOrDefault().Name == "Condition2";

            // Assert
            Assert.True(firstCardResult);
            Assert.True(secondCardResult);
        }
    }
}
