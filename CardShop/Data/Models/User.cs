namespace OnlineCardShop.Data.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using static DataConstants.User;
    public class User
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(MaxNameLength)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(MaxNameLength)]
        public string LastName { get; set; }

        [Required]
        [MaxLength(MaxNameLength)]
        public string Username { get; set; }

        public int Age { get; set; }

        [Required]
        public string ProfileImageUrl { get; set; }

        public IEnumerable<Card> OwnedCards { get; set; } = new List<Card>();

        //public IEnumerable<Card> CurrentlySellingCards { get; set; } = new List<Card>();

        //public IEnumerable<Card> SoldCards { get; set; } = new List<Card>();

        //public IEnumerable<Card> BoughtCards { get; set; } = new List<Card>();
    }
}
