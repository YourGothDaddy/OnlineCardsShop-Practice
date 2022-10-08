namespace OnlineCardShop.Data.Models
{
    using Microsoft.AspNetCore.Identity;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using static DataConstants.User;

    public class User : IdentityUser
    {
        [MaxLength(MaxNameLength)]
        public string FullName { get; set; }

        public int TotalRating { get; set; }

        public int TotalRaters { get; set; }

        public IEnumerable<Review> Reviews { get; set; }

        public int? ProfileImageId { get; set; }

        public ProfileImage ProfileImage { get; set; }
    }
}
