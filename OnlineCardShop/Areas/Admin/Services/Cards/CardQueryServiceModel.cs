namespace OnlineCardShop.Areas.Admin.Services.Cards
{
    using System.Collections.Generic;

    public class CardQueryServiceModel
    {
        public IEnumerable<CardServiceModel> Cards { get; set; }
    }
}
