namespace OnlineCardShop.Areas.Admin.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Server.IIS.Core;
    using OnlineCardShop.Areas.Admin.Services.Dealers;
    using OnlineCardShop.Areas.Admin.Services.Reports;
    using OnlineCardShop.Services.Dealers;

    public class DealersController : AdminController
    {
        private readonly IAdminDealerService adminDealers;
        private readonly IDealerService dealers;
        public readonly IAdminReportService reports;

        public DealersController(IAdminDealerService adminDealers,
            IDealerService dealers,
            IAdminReportService reports)
        {
            this.adminDealers = adminDealers;
            this.dealers = dealers;
            this.reports = reports;
        }

        public IActionResult Dealer([FromQuery] DealerAndReportsServiceModel query, [FromRoute]int id)
        {
            var data = new DealerAndReportsServiceModel();

            if (TempData["dealerId"] != null && id == 0)
            {
                var tempdataDealerId = (int)TempData["dealerId"];

                if (tempdataDealerId >= 1)
                {
                    id = tempdataDealerId;
                }
            }

            var reports = this.adminDealers.GetAllReportsOfDealer(query.CurrentPage, DealerAndReportsServiceModel.ReportsPerPage, id);

            data.Reports = reports.Reports;
            data.TotalReports = reports.TotalReports;
            data.CurrentPage = reports.CurrentPage;

            var userData = this.dealers.GetDealer(id);

            data.Username = userData.Name;
            data.DealerId = id;
            data.TotalRating = this.dealers.GetDealerTotalRating(id);
            data.TotalNumberOfRaters = this.dealers.GetDealerTotalRaters(id);
            data.ReviewsCount = this.dealers.GetDealerReviewsCount(id);

            return View(data);
        }

        public IActionResult Deny([FromRoute]int id)
        {
            this.reports.DenyReport(id);

            return RedirectToAction("Dealer", "Dealers");
        }
    }
}
