namespace OnlineCardShop.Services.Cards
{

    public class CardDetailsServiceModel
    {
        public int Id { get; init; }

        public string Title { get; init; }

        public double Price { get; init; }

        public string Description { get; init; }

        public string Path { get; init; }

        public string Category { get; init; }

        public string Condition { get; init; }

        public int DealerId { get; init; }

        public string DealerName { get; init; }

        public string UserId { get; init; }

        public string cardUser { get; set; }

        public bool IsDeleted { get; set; }

        public bool IsPublic { get; set; }
    }
}
