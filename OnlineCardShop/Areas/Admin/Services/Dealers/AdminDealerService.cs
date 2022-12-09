namespace OnlineCardShop.Areas.Admin.Services.Dealers
{
    using Microsoft.EntityFrameworkCore;
    using OnlineCardShop.Data;
    using System.Linq;

    public class AdminDealerService : IAdminDealerService
    {
        private readonly OnlineCardShopDbContext data;

        public AdminDealerService(OnlineCardShopDbContext data)
        {
            this.data = data;
        }

        public DealerAndReportsServiceModel GetAllReportsOfDealer(int currentPage, int cardsPerPage, int id)
        {
            var dealerIdOfUser = this.data
                .Dealers
                .Where(d => d.Id == id)
                .Select(d => d.UserId)
                .FirstOrDefault();

            var totalReports = this.data
                .Reports
                .Include(x => x.User)
                .Where(r => r.User.Id == dealerIdOfUser)
                .ToList();

            var reportsQuery = totalReports
                .Skip((currentPage - 1) * cardsPerPage)
                .Take(cardsPerPage)
                .Select(r => new ReportServiceModel
                {
                    Id = r.Id,
                    Reason = r.Reason
                })
                .ToList();

            var reports = new DealerAndReportsServiceModel();

            reports.Reports = reportsQuery;
            reports.TotalReports = totalReports.Count();
            reports.CurrentPage = currentPage;

            return reports;
        }
    }
}
