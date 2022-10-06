namespace OnlineCardShop.Data.Models
{
    using Microsoft.AspNetCore.Identity;
    using System.ComponentModel.DataAnnotations;

    using static DataConstants.User;

    public class User : IdentityUser
    {
        [MaxLength(MaxNameLength)]
        public string FullName { get; set; }

        public int? ProfileImageId { get; set; }

        public ProfileImage ProfileImage { get; set; }
    }
}
