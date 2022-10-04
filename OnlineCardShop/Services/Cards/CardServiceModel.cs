namespace OnlineCardShop.Services.Cards
{
    using OnlineCardShop.Data.Models;

    public class CardServiceModel
    {
        public int Id { get; init; }

        public string Title { get; init; }

        public double Price { get; init; }

        public string Description { get; init; }

        public string Path { get; set; }

        public Image ImageFile { get; set; }

        public string Category { get; init; }

        public string Condition { get; init; }

        public int DealerId { get; init; }

        public string DealerName { get; init; }

        public string UserId { get; init; }

        public bool IsPublic { get; set; }
    }
}
