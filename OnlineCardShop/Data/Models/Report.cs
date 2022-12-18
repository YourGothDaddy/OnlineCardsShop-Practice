namespace OnlineCardShop.Data.Models
{
    using System.ComponentModel.DataAnnotations;

    using static Data.DataConstants.Report;

    public class Report
    {
        public int Id { get; set; }

        [Required]
        [StringLength(MaxReasonLength,
            MinimumLength = MinReasonLength)]
        public string Reason { get; set; }

        public User User { get; set; }
    }
}
