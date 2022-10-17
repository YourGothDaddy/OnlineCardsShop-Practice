namespace OnlineCardShop.Data.Models
{
    using Microsoft.AspNetCore.Identity;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Text.Json.Serialization;
    using static DataConstants.User;

    public class User : IdentityUser
    {
        [MaxLength(MaxNameLength)]
        public string FullName { get; set; }

        public string AboutMe { get; set; }

        public string ConnectionId { get; set; }

        public int TotalRating { get; set; }

        public int TotalRaters { get; set; }

        public IEnumerable<Review> Reviews { get; set; }

        public int? ProfileImageId { get; set; }

        public ProfileImage ProfileImage { get; set; }

        [JsonIgnore]
        public IEnumerable<Message> Messages { get; set; }

        public IEnumerable<Chat> Chats { get; set; }

        public IEnumerable<UserChat> UserChats { get; set; }
    }
}
