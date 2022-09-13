namespace OnlineCardShop.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using OnlineCardShop.Data;
    using OnlineCardShop.Data.Models;
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
