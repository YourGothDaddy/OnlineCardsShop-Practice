namespace OnlineCardShop.Data.Models
{
    using System.ComponentModel.DataAnnotations;

    using static Data.DataConstants.Review;

    public class Review
    {
        public int Id { get; set; }

        [Required]
        [StringLength(MaxDescriptionLength,
            MinimumLength = MinDescriptionlength)]
        public string Description { get; set; }

        [Range(MinRating, MaxRating)]
        public int Rating { get; set; }

        [Required]
        public string SubmitterId { get; set; }

        [Required]
        public string UserId { get; set; }

        public User User { get; set; }
    }
}
