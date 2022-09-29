namespace OnlineCardShop.Models.Cards
{
    using OnlineCardShop.Data.Models;
    using OnlineCardShop.Services.Cards;
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

        public Image ImageFile { get; init; }

        [Display(Name = "Condition")]
        public int ConditionId { get; init; }

        public IEnumerable<CardConditionServiceViewModel> Conditions { get; set; }

        [Display(Name="Category")]
        public int CategoryId { get; init; }

        public IEnumerable<CardCategoryServiceViewModel> Categories { get; set; }
    }
}
