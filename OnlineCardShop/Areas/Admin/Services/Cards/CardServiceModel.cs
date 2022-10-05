namespace OnlineCardShop.Areas.Admin.Services.Cards
{
    public class CardServiceModel
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public int DealerId { get; set; }

        public bool IsDeleted { get; set; }

        public bool IsPublic { get; set; }
    }
}
