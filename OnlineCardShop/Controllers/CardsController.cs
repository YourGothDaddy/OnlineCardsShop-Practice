﻿namespace OnlineCardShop.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using OnlineCardShop.Data;
    using OnlineCardShop.Data.Models;
    using OnlineCardShop.Data.Models.Enums;
    using OnlineCardShop.Infrastructure;
    using OnlineCardShop.Models.Cards;
    using OnlineCardShop.Services.Cards;
    using System.Collections.Generic;
    using System.Linq;

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
        public IActionResult Mine()
        {
            var dealerId = this.data
                .Dealers
                .Where(d => d.UserId == this.User.GetId())
                .Select(d => d.Id)
                .FirstOrDefault();

            var cards = this.data
                .Cards
                .Where(c => c.DealerId == dealerId)
                .Select(c => new CardListingViewModel
                {
                    Id = c.Id,
                    Title = c.Title,
                    Price = c.Price,
                    Description = c.Description,
                    ImageUrl = c.ImageUrl,
                    Category = c.Category.Name,
                    Condition = c.Condition.Name
                })
                .ToList();

            MyCardsListingViewModel cardsToModel = new MyCardsListingViewModel();
            cardsToModel.Cards = cards;

            return View(cardsToModel);
        }

        public IActionResult Details([FromRoute] CardListingViewModel query)
        {
            var card = this.data.Cards
                .Where(c => c.Id == query.Id)
                .Select(c => new CardListingViewModel
            {
                Id = c.Id,
                Title = c.Title,
                Price = c.Price,
                Description = c.Description,
                ImageUrl = c.ImageUrl,
                Category = c.Category.Name,
                Condition = c.Condition.Name
            })
                .FirstOrDefault();

            return View(card);
        }

        public IActionResult Game([FromQuery] AllCardsQueryModel query, [FromQuery] int currentPage)
        {
            var queryResult = this.cards.All(
                query.SearchTerm,
                query.Sorting,
                query.CurrentPage,
                AllCardsQueryModel.CardsPerPage,
                2,
                query.Order);

            query.TotalCards = queryResult.TotalCards;
            var cardsToAdd = queryResult.Cards
                .Select(c => new CardListingViewModel
                {
                    Id = c.Id,
                    Title = c.Title,
                    Description = c.Description,
                    ImageUrl = c.ImageUrl,
                    Category = c.Category,
                    Condition = c.Condition,
                    Price = c.Price
                })
                .ToList();
            query.Cards = cardsToAdd;
            if (currentPage == 0)
            {
                ViewBag.CurrentPage = 1;
            }
            else
            {
                ViewBag.CurrentPage = currentPage;
            }
            return View(query); ;
        }

        public IActionResult Kpop([FromQuery] AllCardsQueryModel query, [FromQuery] int currentPage)
        {
            var queryResult = this.cards.All(
                query.SearchTerm,
                query.Sorting,
                query.CurrentPage,
                AllCardsQueryModel.CardsPerPage,
                1,
                query.Order);

            query.TotalCards = queryResult.TotalCards;
            var cardsToAdd = queryResult.Cards
                .Select(c => new CardListingViewModel
                {
                    Id = c.Id,
                    Title = c.Title,
                    Description = c.Description,
                    ImageUrl = c.ImageUrl,
                    Category = c.Category,
                    Condition = c.Condition,
                    Price = c.Price
                })
                .ToList();
            query.Cards = cardsToAdd;
            if (currentPage == 0)
            {
                ViewBag.CurrentPage = 1;
            }
            else
            {
                ViewBag.CurrentPage = currentPage;
            }
            return View(query); ;
        }

        public IActionResult All([FromQuery]AllCardsQueryModel query, [FromQuery]int currentPage)
        {
            var queryResult = this.cards.All(
                query.SearchTerm,
                query.Sorting,
                query.CurrentPage, 
                AllCardsQueryModel.CardsPerPage,
                null,
                null);

            query.TotalCards = queryResult.TotalCards;
            var cardsToAdd = queryResult.Cards
                .Select(c => new CardListingViewModel
                {
                    Id = c.Id,
                    Title = c.Title,
                    Description = c.Description,
                    ImageUrl = c.ImageUrl,
                    Category = c.Category,
                    Condition = c.Condition,
                    Price = c.Price
                })
                .ToList();
            query.Cards = cardsToAdd;
            if(currentPage == 0)
            {
                ViewBag.CurrentPage = 1;
            }
            else
            {
                ViewBag.CurrentPage = currentPage;
            }
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
        public IActionResult Add(AddCardFormModel card)
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

            var cardData = new Card
            {
                Title = card.Title,
                Description = card.Description,
                ImageUrl = card.ImageUrl,
                CategoryId = card.CategoryId,
                ConditionId = card.ConditionId,
                Price = card.Price,
                DealerId = dealerId
            };

            this.data.Cards.Add(cardData);

            this.data.SaveChanges();

            return RedirectToAction("Index", "Home");
        }

        private bool UserIsDealer()
        {
            return this.data
                .Dealers
                .Any(d => d.UserId == this.User.GetId());
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
