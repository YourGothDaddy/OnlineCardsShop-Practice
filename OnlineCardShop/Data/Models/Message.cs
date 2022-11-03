namespace OnlineCardShop.Data.Models
{
    using System;
    using System.Text.Json.Serialization;

    public class Message
    {
        public int Id { get; set; }

        public int ChatId { get; set; }

        [JsonIgnore]
        public Chat Chat { get; set; }

        public string UserId { get; set; }

        public User User { get; set; }

        public string Content { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
