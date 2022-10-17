namespace OnlineCardShop.Data.Models
{
    using System.Collections.Generic;

    public class Chat
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public IEnumerable<Message> Messages { get; set; }

        public ICollection<User> Users { get; set; } = new List<User>();

        public IEnumerable<UserChat> UserChats { get; set; }
    }
}
