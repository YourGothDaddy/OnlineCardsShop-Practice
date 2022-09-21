namespace OnlineCardShop.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using OnlineCardShop.Data;
    using OnlineCardShop.Data.Models.Enums;
    using OnlineCardShop.Models;
    using OnlineCardShop.Models.Cards;
    using OnlineCardShop.Models.Home;
    using System.Diagnostics;
    using System.Linq;

    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly OnlineCardShopDbContext data;

        public HomeController(ILogger<HomeController> logger, OnlineCardShopDbContext data)
        {
            _logger = logger;
            this.data = data;
        }

        public IActionResult Index()
        {
            var categories = this.data
                .Categories
                .Select(c => new CardCategoryViewModel
                {
                    Id = c.Id,
                    Name = c.Name
                });

            return View(new AllCategoriesViewModel
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
