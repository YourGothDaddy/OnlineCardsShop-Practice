namespace OnlineCardShop.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using OnlineCardShop.Data;
    using OnlineCardShop.Data.Models;
    using OnlineCardShop.Data.Models.Enums;
    using OnlineCardShop.Models.Cards;
    using System.Collections.Generic;
    using System.Linq;

    public class CardsController : Controller
    {
        private readonly OnlineCardShopDbContext data;

        public CardsController(OnlineCardShopDbContext data)
        {
            this.data = data;
        }

        public IActionResult All(
            CardSorting sorting,
            string searchTerm)
        {
            var cardsQuery = this.data.Cards.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                cardsQuery = cardsQuery
                    .Where(c =>
                    c.Title.ToLower().Contains(searchTerm.ToLower()));
            }

            cardsQuery = sorting switch
            {
                CardSorting.Condition => cardsQuery.OrderBy(c => c.Condition),
                CardSorting.Category => cardsQuery.OrderByDescending(c => c.Category),
                _ => cardsQuery.OrderByDescending(c => c.Condition)
            };

            var cards = cardsQuery
                .Select(c => new CardListingViewModel
                {
                    Title = c.Title,
                    Description = c.Description,
                    ImageUrl = c.ImageUrl,
                    Category = c.Category.Name,
                    Condition = c.Condition.Name

                })
                .ToList();

            return View(new AllCardsQueryModel
            {
                Cards = cards,
                SearchTerm = searchTerm,
                Sorting = sorting
            }); ;
        }

        public IActionResult Add()
        {

            return View(new AddCardFormModel
            {
                Categories = this.GetCardCategories(),
                Conditions = this.GetCardConditions()
            });
        }

        [HttpPost]
        public IActionResult Add(AddCardFormModel card)
        {
            if (!this.data.Categories.Any(c => c.Id == card.CategoryId))
            {
                this.ModelState.AddModelError(nameof(card.CategoryId), "Category does not exist");
            }

            if (!this.data.Conditions.Any(c => c.Id == card.ConditionId))
            {
                this.ModelState.AddModelError(nameof(card.ConditionId), "Condition does not exist");
            }

            if (!ModelState.IsValid)
            {
                //The card object has null the categories and conditions, so I add them again
                card.Categories = this.GetCardCategories();
                card.Conditions = this.GetCardConditions();

                return View(card);
            }

            var cardData = new Card
            {
                Title = card.Title,
                Description = card.Description,
                ImageUrl = card.ImageUrl,
                CategoryId = card.CategoryId,
                ConditionId = card.ConditionId,
            };

            this.data.Cards.Add(cardData);

            this.data.SaveChanges();

            return RedirectToAction("Index", "Home");
        }

        private IEnumerable<CardCategoryViewModel> GetCardCategories()
        {
           return this.data
                .Categories
                .Select(c => new CardCategoryViewModel
                {
                    Id = c.Id,
                    Name = c.Name
                })
                .ToList();
        }

        private IEnumerable<CardConditionViewModel> GetCardConditions()
        {
            return this.data
                 .Conditions
                 .Select(c => new CardConditionViewModel
                 {
                     Id = c.Id,
                     Name = c.Name
                 })
                 .ToList();
        }
    }
}
