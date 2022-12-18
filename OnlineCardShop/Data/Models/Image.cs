namespace OnlineCardShop.Data.Models
{
    using System.ComponentModel.DataAnnotations;
    using static DataConstants.Image;

    public class Image
    {
        public int Id { get; set; }

        [Required]
        [StringLength(MaxNameLength,
            MinimumLength = MinNameLength)]
        public string Name { get; set; }

        [Required]
        [StringLength(MaxOriginalNameLength,
            MinimumLength = MinOriginalNameLength)]
        public string OriginalName { get; set; }

        [Required]
        [StringLength(MaxPathLength,
            MinimumLength = MinPathLength)]
        public string Path { get; set; }

        public int? CardId { get; set; }

        public Card Card { get; set; }
    }
}
