namespace OnlineCardShop.Data.Models
{
    using System.Collections.Generic;

    public class Chat
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public ICollection<Message> Messages { get; set; } = new List<Message>();


        public ICollection<User> Users { get; set; } = new List<User>();
    }
}
