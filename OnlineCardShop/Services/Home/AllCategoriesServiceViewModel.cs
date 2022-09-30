namespace OnlineCardShop.Models.Home
{
    using OnlineCardShop.Models.Cards;
    using OnlineCardShop.Services.Cards;
    using System.Collections.Generic;

    public class AllCategoriesServiceViewModel
    {
        public IEnumerable<CardCategoryServiceViewModel> Categories { get; set; }
    }
}
