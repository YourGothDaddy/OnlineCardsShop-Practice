namespace OnlineCardShop.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using OnlineCardShop.Models;
    using OnlineCardShop.Models.Home;
    using OnlineCardShop.Services.Home;
    using System.Diagnostics;

    public class HomeController : Controller
    {
        private readonly IHomeService home;

        public HomeController(IHomeService home)
        {
            this.home = home;
        }

        public IActionResult Index()
        {
            var categories = this.home.GetCategories();

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
