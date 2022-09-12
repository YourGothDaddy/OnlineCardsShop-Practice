namespace OnlineCardShop.Models.Cards
{
    using OnlineCardShop.Data.Models.Enums;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class AddCardFormModel
    {
        public string Title { get; init; }

        [Display(Name = "Image URL")]
        public string ImageUrl { get; init; }

        public Condition Condition { get; init; }

        public int UserId { get; init; }

        [Display(Name="Category")]
        public int CategoryId { get; init; }

        public IEnumerable<CardCategoryViewModel> Categories { get; set; }
    }
}
