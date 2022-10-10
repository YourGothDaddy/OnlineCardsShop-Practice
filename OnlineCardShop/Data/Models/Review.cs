namespace OnlineCardShop.Data.Models
{
    public class Review
    {
        public int Id { get; set; }

        public string Description { get; set; }

        public int Rating { get; set; }

        public string SubmitterId { get; set; }

        public string UserId { get; set; }

        public User User { get; set; }
    }
}
