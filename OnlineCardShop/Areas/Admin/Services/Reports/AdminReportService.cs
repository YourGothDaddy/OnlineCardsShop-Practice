namespace OnlineCardShop.Areas.Admin.Services.Reports
{
    using OnlineCardShop.Data;
    using System.Linq;

    public class AdminReportService : IAdminReportService
    {
        private readonly OnlineCardShopDbContext data;

        public AdminReportService(OnlineCardShopDbContext data)
        {
            this.data = data;
        }
        public void DenyReport(int reportId)
        {
            var report = this.data
                .Reports
                .Where(r => r.Id == reportId)
                .FirstOrDefault();

            this.data
                .Reports
                .Remove(report);

            this.data.SaveChanges();
        }
    }
}
