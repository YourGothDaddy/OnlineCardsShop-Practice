namespace OnlineCardShop.Models.Home
{
    using OnlineCardShop.Models.Cards;
    using System.Collections.Generic;

    public class AllCategoriesViewModel
    {
        public IEnumerable<CardCategoryViewModel> Categories { get; set; }
    }
}
