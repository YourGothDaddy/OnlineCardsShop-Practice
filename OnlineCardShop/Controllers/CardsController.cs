namespace OnlineCardShop.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using OnlineCardShop.Data;
    using OnlineCardShop.Data.Models;
    using OnlineCardShop.Data.Models.Enums;
    using OnlineCardShop.Infrastructure;
    using OnlineCardShop.Models.Cards;
    using OnlineCardShop.Services.Cards;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    public class CardsController : Controller
    {
        private readonly ICardService cards;
        private readonly OnlineCardShopDbContext data;

        public CardsController(ICardService cards, OnlineCardShopDbContext data)
        {
            this.cards = cards;
            this.data = data;
        }

        [Authorize]
        public IActionResult Mine([FromQuery] int currentPage)
        {
            if (currentPage == 0)
            {
                currentPage = 1;
            }

            var myCards = this.cards.ByUser(this.User.GetId(),
                currentPage,
                AllCardsServiceModel.CardsPerPage);

            SelectCurrentPage(currentPage);

            return View(myCards);
        }

        public IActionResult Details([FromRoute] CardServiceModel query)
        {
            var card = this.cards.CardByUser(query.Id);

            return View(card);
        }

        public IActionResult Game([FromQuery] AllCardsServiceModel query, [FromQuery] int currentPage)
        {
            var queryResult = this.cards.All(
                query.SearchTerm,
                query.Sorting,
                query.CurrentPage,
                AllCardsServiceModel.CardsPerPage,
                2,
                query.Order);

            CardsToAddOnPage(query, queryResult);
            SelectCurrentPage(currentPage);

            return View(query);
        }

        public IActionResult Kpop([FromQuery] AllCardsServiceModel query, [FromQuery] int currentPage)
        {
            var queryResult = this.cards.All(
                query.SearchTerm,
                query.Sorting,
                query.CurrentPage,
                AllCardsServiceModel.CardsPerPage,
                1,
                query.Order);

            CardsToAddOnPage(query, queryResult);
            SelectCurrentPage(currentPage);
            return View(query); ;
        }

        public IActionResult All([FromQuery] AllCardsServiceModel query, [FromQuery]int currentPage)
        {
            var queryResult = this.cards.All(
                query.SearchTerm,
                query.Sorting,
                query.CurrentPage,
                AllCardsServiceModel.CardsPerPage,
                null,
                null);

            CardsToAddOnPage(query, queryResult);
            SelectCurrentPage(currentPage);

            return View(query); ;
        }

        [Authorize]
        public IActionResult Add()
        {
            if (!this.UserIsDealer())
            {
                return RedirectToAction(nameof(DealersController.Create), "Dealers");
            }

            return View(new AddCardFormModel
            {
                Categories = this.GetCardCategories(),
                Conditions = this.GetCardConditions()
            });
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Add(AddCardFormModel card, List<IFormFile> Image)
        {
            var dealerId = this.data
                .Dealers
                .Where(d => d.UserId == this.User.GetId())
                .Select(d => d.Id)
                .FirstOrDefault();

            if (dealerId == 0)
            {
                return RedirectToAction(nameof(DealersController.Create), "Dealers");
            }

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

            foreach (var image in Image)
            {
                if (image.Length > 0 || image.Length <= (2 * 1024 * 1024))
                {
                    using(var stream = new MemoryStream())
                    {
                        await image.CopyToAsync(stream);

                        var cardData = new Card
                        {
                            Title = card.Title,
                            Description = card.Description,
                            ImageUrl = card.ImageUrl,
                            CategoryId = card.CategoryId,
                            ConditionId = card.ConditionId,
                            Price = card.Price,
                            DealerId = dealerId,
                            Image = stream.ToArray()
                        };

                        this.data.Cards.Add(cardData);
                        this.data.SaveChanges();
                    }
                }
            }

            return RedirectToAction("Index", "Home");
        }

        private bool UserIsDealer()
        {
            return this.data
                .Dealers
                .Any(d => d.UserId == this.User.GetId());
        }

        private void SelectCurrentPage(int currentPage)
        {
            if (currentPage == 0)
            {
                ViewBag.CurrentPage = 1;
            }
            else
            {
                ViewBag.CurrentPage = currentPage;
            }
        }

        private static void CardsToAddOnPage(AllCardsServiceModel query, CardQueryServiceModel queryResult)
        {
            var cardsToAdd = queryResult.Cards
                            .Select(c => new CardServiceModel
                            {
                                Id = c.Id,
                                Title = c.Title,
                                Description = c.Description,
                                ImageUrl = c.ImageUrl,
                                Category = c.Category,
                                Condition = c.Condition,
                                Price = c.Price,
                                Image = c.Image
                            })
                            .ToList();

            query.TotalCards = queryResult.TotalCards;
            query.Cards = cardsToAdd;
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
