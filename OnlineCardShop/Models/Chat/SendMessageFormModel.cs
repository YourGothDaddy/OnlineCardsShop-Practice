namespace OnlineCardShop.Models.Chat
{
    using System.ComponentModel.DataAnnotations;

    using static Data.DataConstants.Message;

    public class SendMessageFormModel
    {
        [Required]
        [StringLength(MaxContentLength,
            MinimumLength = MinContentLength,
            ErrorMessage = "The content of the message should be between {2} and {1} characters!")]
        public string Content { get; set; }
    }
}
