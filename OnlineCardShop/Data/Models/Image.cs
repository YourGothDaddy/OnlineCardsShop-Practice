namespace OnlineCardShop.Data.Models
{
    using System.ComponentModel.DataAnnotations;

    public class Image
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Path { get; set; }

        public int? CardId { get; set; }

        public Card Card { get; set; }
    }
}
