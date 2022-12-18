namespace OnlineCardShop.Tests.Services
{
    using OnlineCardShop.Data.Models;
    using OnlineCardShop.Services.Dealers;
    using OnlineCardShop.Tests.Mocks;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using Xunit;

    public class DealerServiceTests
    {
        [Fact]
        public void IsDealerShouldReturnTrueWhenUserIsDealer()
        {
            // Arrange
            const string userId = "TestId";

            using var data = DatabaseMock.Instance;

            data.Dealers.Add(new Dealer { UserId = userId });
            data.SaveChanges();

            var dealerService = new DealerService(data);

            // Act
            var result = dealerService.IsDealer(userId);


            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsDealerShouldReturnFalseWhenUserIsDealer()
        {
            // Arrange
            const string userId = "TestId";

            using var data = DatabaseMock.Instance;

            data.Dealers.Add(new Dealer { UserId = userId });
            data.SaveChanges();

            var dealerService = new DealerService(data);

            // Act
            var result = dealerService.IsDealer("incorrectId");


            // Assert
            Assert.False(result);
        }

        [Theory]
        [InlineData("TestId", "TestName", "0882713110")]
        public void CreateDealerShouldAddDealer(string userId,
            string name,
            string phoneNumber)
        {
            // Arrange
            using var data = DatabaseMock.Instance;

            var dealer = new Dealer
            {
                UserId = userId,
                Name = name
            };

            var dealerService = new DealerService(data);

            // Act
            dealerService.CreateDealer(dealer);

            var result = data.Dealers.Contains(dealer);

            Assert.True(result);
        }

        [Theory]
        [InlineData("testDescription", 1, "TestId")]
        public void AddReviewShouldAddReview(string description,
            int rating,
            string userId)
        {
            // Arrange
            using var data = DatabaseMock.Instance;

            var dealerService = new DealerService(data);

            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, "username"),
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim("name", "John Doe"),
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            // Act
            dealerService.AddReview(description, rating, userId, claimsPrincipal);

            var result = data.Reviews.Contains(new Review
            {
                Description = description,
                Rating = rating,
                UserId = userId,
                SubmitterId = userId,
                Id = 1
            });

            // Assert
            Assert.True(result);
        }

        [Theory]
        [InlineData("TestId", 1)]
        public void GetTotalRatingShouldNotBeNullAndShouldBe1(string userId,
            int totalRating)
        {
            // Arrange
            using var data = DatabaseMock.Instance;

            data.Users.Add(new User
            {
                Id = userId,
                TotalRating = totalRating
            });

            data.SaveChanges();

            var dealerService = new DealerService(data);

            // Act
            var result = dealerService.GetTotalRating(userId);

            // Assert
            Assert.NotNull(result);
            Assert.True(result == totalRating);
        }

        [Theory]
        [InlineData("TestId", 1)]
        public void GetTotalRatersShouldNotBeNullAndShouldBe1(string userId,
            int totalRaters)
        {
            // Arrange
            using var data = DatabaseMock.Instance;

            data.Users.Add(new User
            {
                Id = userId,
                TotalRaters = totalRaters
            });

            data.SaveChanges();

            var dealerService = new DealerService(data);

            // Act
            var result = dealerService.GetTotalRaters(userId);

            // Assert
            Assert.NotNull(result);
            Assert.True(result == totalRaters);
        }

        [Theory]
        [InlineData("TestId", 1)]
        public void GetTotalReviewsShouldNotBeNullAndShouldBe1(string userId,
            int totalReviews)
        {
            // Arrange
            using var data = DatabaseMock.Instance;

            var reviews = new List<Review>
            {
                new Review { Id = 1 }
            };

            data.Users.Add(new User
            {
                Id = userId,
                Reviews = reviews
            });

            data.SaveChanges();

            var dealerService = new DealerService(data);

            // Act
            var result = dealerService.GetReviews(userId);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Count() == totalReviews);
        }

        [Theory]
        [InlineData("testSubmitterId", 1)]
        public void GetTotalSubmittersShouldNotBeNullAndShouldBe1(string submitterId,
            int id)
        {
            // Arrange
            using var data = DatabaseMock.Instance;

            var reviews = new List<Review>
            {
                new Review
                {
                    Id = id,
                    SubmitterId = submitterId
                }
            };

            var user = new User
            {
                Id = submitterId
            };

            var incorrectUser = new User
            {
                Id = "incorrectId"
            };

            data.Users.Add(user);
            data.Users.Add(incorrectUser);

            data.SaveChanges();

            var dealerService = new DealerService(data);

            // Act
            var result = dealerService.GetSubmitters(reviews);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Count() == 1);
        }

        [Theory]
        [InlineData("TestId", 1)]
        public void GetDealerIdShouldReturnCorrectDealerId(string userId,
            int id)
        {
            // Arrange
            using var data = DatabaseMock.Instance;

            var dealer = new Dealer
            {
                Id = id,
                UserId = userId
            };

            var incorrectDealer = new Dealer
            {
                Id = 2,
                UserId = "incorrectUserId"
            };

            data.Dealers.Add(dealer);
            data.Dealers.Add(incorrectDealer);

            data.SaveChanges();

            var dealerService = new DealerService(data);

            // Act
            var result = dealerService.GetDealerId(userId);

            // Assert
            Assert.NotNull(result);
            Assert.True(result == id);
        }
    }
}
