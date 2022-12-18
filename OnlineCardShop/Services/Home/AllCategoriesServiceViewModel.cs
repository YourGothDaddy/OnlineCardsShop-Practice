namespace OnlineCardShop.Models.Home
{
    using OnlineCardShop.Data.Models;
    using OnlineCardShop.Services.Cards;
    using System.Collections.Generic;

    public class AllCategoriesServiceViewModel
    {
        public IEnumerable<CardCategoryServiceViewModel> Categories { get; set; }

        public ProfileImage ProfileImage { get; set; }

        public StatisticsViewModel Statistics { get; set; }
    }
}
