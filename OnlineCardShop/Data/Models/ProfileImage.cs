namespace OnlineCardShop.Data.Models
{
    using System.ComponentModel.DataAnnotations;

    public class ProfileImage
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string OriginalName { get; set; }

        [Required]
        public string Path { get; set; }

        public int UserId { get; set; }

        public User User { get; set; }
    }
}
