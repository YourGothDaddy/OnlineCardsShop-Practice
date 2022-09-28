namespace OnlineCardShop.Models.Dealers
{
    using System.ComponentModel.DataAnnotations;

    using static Data.DataConstants.Dealer;

    public class BecomeDealerFormModel
    {
        [Required]
        [StringLength(MaxNameLength, MinimumLength = MinNameLength)]
        public string Name { get; set; }

        [Required]
        [StringLength(PhoneNumberMaxLength, MinimumLength = PhoneNumberMinLength)]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }
    }
}
