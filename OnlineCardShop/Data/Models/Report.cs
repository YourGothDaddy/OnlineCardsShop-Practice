namespace OnlineCardShop.Data.Models
{
    using System.ComponentModel.DataAnnotations;

    public class Report
    {
        public int Id { get; set; }

        [Required]
        public string Reason { get; set; }

        public User User { get; set; }
    }
}
