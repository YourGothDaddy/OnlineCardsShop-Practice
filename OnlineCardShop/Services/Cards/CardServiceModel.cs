namespace OnlineCardShop.Services.Cards
{
    using OnlineCardShop.Data.Models;

    public class CardServiceModel : CardDetailsServiceModel
    {
        public Image ImageFile { get; set; }

        public int DealerId { get; init; }

        public bool IsPublic { get; set; }
    }
}
