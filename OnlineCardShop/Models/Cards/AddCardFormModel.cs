namespace OnlineCardShop.Models.Cards
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class AddCardFormModel
    {
        public string Title { get; init; }

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
