namespace OnlineCardShop.Data.Models
{
    using System.ComponentModel.DataAnnotations;
    using static DataConstants.Card;
    public class Card
    {
        public int Id { get; set; }

        [Required]
        [StringLength(MaxTitleLength, 
            MinimumLength = MinTitleLength)]
        public string Title { get; set; }

        [Range(double.MinValue, double.MaxValue)]
        public double Price { get; set; }

        public int ImageId { get; set; }
        public Image Image { get; set; }

        [Required]
        [StringLength(MaxDescriptionLength,
            MinimumLength = MinDescriptionLength)]
        public string Description { get; set; }

        public int ConditionId { get; set; }

        public Condition Condition { get; set; }

        public int CategoryId { get; set; }

        public Category Category { get; init; }

        public int DealerId { get; set; }

        public Dealer Dealer { get; set; }

        public bool IsPublic { get; set; }

        public bool IsDeleted { get; set; }
    }
}
