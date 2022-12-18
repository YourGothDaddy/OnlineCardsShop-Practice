namespace OnlineCardShop.Data.Models
{
    using System.ComponentModel.DataAnnotations;

    using static Data.DataConstants.Image;

    public class ProfileImage
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

        public int UserId { get; set; }

        public User User { get; set; }
    }
}
