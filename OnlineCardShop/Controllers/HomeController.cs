﻿namespace OnlineCardShop.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using OnlineCardShop.Data;
    using OnlineCardShop.Models;
    using OnlineCardShop.Models.Home;
    using OnlineCardShop.Services.Cards;
    using System.Diagnostics;
    using System.Linq;

    public class HomeController : Controller
    {
        private readonly OnlineCardShopDbContext data;

        public HomeController(OnlineCardShopDbContext data)
        {
            this.data = data;
        }

        public IActionResult Index()
        {
            var categories = this.data
                .Categories
                .Select(c => new CardCategoryServiceViewModel
                {
                    Id = c.Id,
                    Name = c.Name
                });

            return View(new AllCategoriesServiceViewModel
            {
                Categories = categories
            });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
