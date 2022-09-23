namespace OnlineCardShop.Services.Cards
{
    using System.Collections.Generic;

    public class CardQueryServiceModel
    {
        public IEnumerable<CardServiceModel> Cards { get; init; }
    }
}
