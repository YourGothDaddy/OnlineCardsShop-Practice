namespace OnlineCardShop.Data.Models
{
    using Microsoft.AspNetCore.Identity;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using static DataConstants.Dealer;

    public class Dealer
    {
        public int Id { get; init; }

        [Required]
        [MaxLength(MaxNameLength)]
        public string Name { get; set; }

        [Required]
        [MaxLength(PhoneNumberMaxLength)]
        public string PhoneNumber { get; set; }

        [Required]
        public string UserId { get; set; }

        public virtual User User { get; set; }

        public IEnumerable<Card> Cards { get; init; } = new List<Card>();
    }
}
