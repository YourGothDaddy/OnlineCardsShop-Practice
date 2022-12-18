namespace OnlineCardShop.Tests.Services
{
    using OnlineCardShop.Areas.Admin.Services.Reports;
    using OnlineCardShop.Data.Models;
    using OnlineCardShop.Tests.Mocks;
    using System.Linq;
    using Xunit;

    public class AdminReportServiceTests
    {

        [Fact]
        public void DenyReportShouldDeleteReport()
        {
            // Arrange
            using var data = DatabaseMock.Instance;

            data.Reports.Add(new Report
            {
                Id = 1
            });

            data.SaveChanges();

            var service = new AdminReportService(data);

            // Act
            service.DenyReport(1);

            var reportIsDeleted = data.Reports
                .Where(r => r.Id == 1)
                .FirstOrDefault() == null;

            // Assert
            Assert.True(reportIsDeleted);
        }
    }
}
