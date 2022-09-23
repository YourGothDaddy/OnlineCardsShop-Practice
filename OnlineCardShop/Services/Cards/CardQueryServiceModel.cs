namespace OnlineCardShop.Services.Cards
{
    using System.Collections.Generic;

    public class CardQueryServiceModel
    {
        public int TotalCards { get; set; }
        public IEnumerable<CardServiceModel> Cards { get; init; }
    }
}
