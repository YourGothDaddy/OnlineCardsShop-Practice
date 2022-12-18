namespace OnlineCardShop.Tests.Services
{
    using OnlineCardShop.Areas.Admin.Services.Dealers;
    using OnlineCardShop.Data.Models;
    using OnlineCardShop.Tests.Mocks;
    using System.Linq;
    using Xunit;

    public class AdminDealerServiceTests
    {

        [Fact]
        public void GetAllReportsOfDealerShouldReturnCorrectReports()
        {
            // Arrange
            using var data = DatabaseMock.Instance;

            data.Dealers.Add(new Dealer
            {
                UserId = "testId"
            });

            data.Reports.Add(new Report
            {
                Id = 1,
                Reason = "testReason",
                User = new User
                {
                    Id = "testId"
                }
            });

            data.SaveChanges();

            var service = new AdminDealerService(data);

            // Act
            var result = service.GetAllReportsOfDealer(1, 1, 1);

            var reportsAreCorrect = result.Reports.FirstOrDefault().Id == 1;
            var totalReportsCountIsOne = result.TotalReports == 1;
            var reportsCurrentPageIsOne = result.CurrentPage == 1;

            // Assert
            Assert.True(reportsAreCorrect);
            Assert.True(totalReportsCountIsOne);
            Assert.True(reportsCurrentPageIsOne);
        }
    }
}
