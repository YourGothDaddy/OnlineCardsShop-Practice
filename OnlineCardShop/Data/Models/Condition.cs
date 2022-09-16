namespace OnlineCardShop.Data.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    public class Condition
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public IEnumerable<Card> Cards { get; set; } = new List<Card>();
    }
}
