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
                Categories = this.GetCardCategories()
            });
        }

        [HttpPost]
        public IActionResult Add(AddCardFormModel card)
        {
            return View();
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
    }
}
