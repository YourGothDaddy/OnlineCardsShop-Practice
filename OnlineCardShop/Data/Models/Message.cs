namespace OnlineCardShop.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Text.Json.Serialization;

    using static Data.DataConstants.Message;

    public class Message
    {
        public int Id { get; set; }

        public int ChatId { get; set; }

        [JsonIgnore]
        public Chat Chat { get; set; }

        [Required]
        public string UserId { get; set; }

        public User User { get; set; }

        [Required]
        [StringLength(MaxContentLength,
            MinimumLength = MinContentLength)]
        public string Content { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
