namespace OnlineCardShop.Data.Models
{
    using OnlineCardShop.Data.Models.Enums;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using static DataConstants.Card;
    public class Card
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(MaxTitleLength)]
        public string Title { get; set; }

        [Required]
        public string ImageUrl { get; set; }

        public CardTypes CardType { get; set; }

        //[Column("OwnerId")]
        public int UserId { get; set; }

        public User User { get; set; }

        public int CategoryId { get; set; }

        public Category Category { get; set; }
    }
}
