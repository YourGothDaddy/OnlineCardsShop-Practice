namespace OnlineCardShop.Areas.Admin.Services.Dealers
{
    using System.Collections.Generic;

    public class DealerAndReportsServiceModel
    {
        public int DealerId { get; set; }

        public string Username { get; set; }

        public string ProfileImage { get; set; }

        public int CardsCount { get; set; }

        public int ReviewsCount { get; set; }

        public int TotalRating { get; set; }

        public int TotalNumberOfRaters { get; set; }

        public int CurrentPage { get; set; } = 1;

        public const int ReportsPerPage = 8;
        public int TotalReports { get; set; }
        public IEnumerable<ReportServiceModel> Reports { get; set; }
    }
}
