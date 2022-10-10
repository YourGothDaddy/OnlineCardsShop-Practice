namespace OnlineCardShop.Services.Dealers
{
    using OnlineCardShop.Data.Models;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class DealerServiceViewModel
    {
        public string Name { get; set; }

        public string PhoneNumber { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public int Rating { get; set; }

        public IEnumerable<Review> Reviews { get; set; }
    }
}
