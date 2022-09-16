namespace OnlineCardShop.Data.Models
{
    using System.ComponentModel.DataAnnotations;
    using static DataConstants.Card;
    public class Card
    {
        public int Id { get; set; }

        [Required]
        [MinLength(MinTitleLength)]
        [MaxLength(MaxTitleLength)]
        public string Title { get; set; }

        [Required]
        public string ImageUrl { get; set; }

        [Required]
        [MinLength(MinDescriptionLength)]
        [MaxLength(MaxDescriptionLength)]
        public string Description { get; set; }

        public int ConditionId { get; set; }

        public Condition Condition { get; set; }

        //TODO: Uncomment when time to add users
        //[Column("OwnerId")]
        //public int UserId { get; set; }

        //public User User { get; set; }

        public int CategoryId { get; set; }

        public Category Category { get; set; }
    }
}
