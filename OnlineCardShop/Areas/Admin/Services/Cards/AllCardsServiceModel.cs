namespace OnlineCardShop.Areas.Admin.Services.Cards
{
    using System.Collections.Generic;

    public class AllCardsServiceModel
    {
        public int CurrentPage { get; set; } = 1;

        public const int CardsPerPage = 8;
        public int TotalCards { get; set; }
        public IEnumerable<CardServiceModel> Cards { get; set; }
    }
}
