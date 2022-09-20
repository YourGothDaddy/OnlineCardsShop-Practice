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

        public IActionResult Kpop([FromQuery] AllCardsQueryModel query, [FromQuery] int currentPage)
        {
            var cardsQuery = this.data.Cards
                .Where(c => c.CategoryId == 1)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.SearchTerm))
            {
                cardsQuery = cardsQuery
                    .Where(c =>
                    c.Title.ToLower().Contains(query.SearchTerm.ToLower()));
            }

            cardsQuery = query.Sorting switch
            {
                CardSorting.Condition => cardsQuery.OrderBy(c => c.Condition),
                _ => cardsQuery.OrderByDescending(c => c.Condition)
            };

            cardsQuery = query.Order switch
            {
                SortingOrder.BestToWorse => cardsQuery.OrderBy(c => c.Condition),
                SortingOrder.WorseToBest => cardsQuery.OrderByDescending(c => c.Condition),
                _ => cardsQuery.OrderBy(c => c.Condition)
            };

            var totalCards = cardsQuery.Count();

            var cards = cardsQuery
                .Skip((query.CurrentPage - 1) * AllCardsQueryModel.CardsPerPage)
                .Take(AllCardsQueryModel.CardsPerPage)
                .Select(c => new CardListingViewModel
                {
                    Title = c.Title,
                    Description = c.Description,
                    ImageUrl = c.ImageUrl,
                    Category = c.Category.Name,
                    Condition = c.Condition.Name

                })
                .ToList();

            query.TotalCards = totalCards;
            query.Cards = cards;
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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
