namespace OnlineCardShop.Models.Cards
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using static OnlineCardShop.Data.DataConstants.Card;

    public class AddCardFormModel
    {
        [Required(ErrorMessage = "The 'Title' field is required!")]
        [StringLength
            (MaxTitleLength, 
            MinimumLength = MinTitleLength, 
            ErrorMessage = "The title length must be between {2} and {1}.")]
        public string Title { get; init; }

        [Range(double.MinValue, double.MaxValue)]
        public double Price { get; init; }

        [Required(ErrorMessage = "The 'Description' field is required!")]
        public string Description { get; init; }

        [Required(ErrorMessage = "The 'Image URL' field is required!")]
        [Url]
        [Display(Name = "Image URL")]
        public string ImageUrl { get; init; }

        [Display(Name = "Condition")]
        public int ConditionId { get; init; }

        public IEnumerable<CardConditionViewModel> Conditions { get; set; }

        [Display(Name="Category")]
        public int CategoryId { get; init; }

        public IEnumerable<CardCategoryViewModel> Categories { get; set; }
    }
}
