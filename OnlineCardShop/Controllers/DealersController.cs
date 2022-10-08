namespace OnlineCardShop.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using OnlineCardShop.Data;
    using OnlineCardShop.Data.Models;
    using OnlineCardShop.Infrastructure;
    using OnlineCardShop.Models.Dealers;
    using OnlineCardShop.Services.Dealers;
    using System.Linq;

    public class DealersController : Controller
    {
        private readonly OnlineCardShopDbContext data;
        private readonly IDealerService dealers;

        public DealersController(OnlineCardShopDbContext data,
            IDealerService dealers)
        {
            this.data = data;
            this.dealers = dealers;
        }

        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public IActionResult Create(BecomeDealerFormModel dealer)
        {
            var userId = this.User.GetId();

            var userIdAlreadyDealer = this.data
                .Dealers
                .Any(d => d.UserId == userId);

            if (userIdAlreadyDealer)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View(dealer);
            }

            var currentDealer = new Dealer
            {
                Name = dealer.Name,
                PhoneNumber = dealer.PhoneNumber,
                UserId = userId
            };

            this.data.Dealers.Add(currentDealer);
            this.data.SaveChanges();

            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        public IActionResult Dealer([FromRoute] string id)
        {
            var cardDealerId = this.dealers.GetDealerId(id);

            var cardDealer = this.dealers.GetDealer(cardDealerId);

            return View(cardDealer);
        }

        public bool IsDealer(string userId)
        {
            return this.data
                .Dealers
                .Any(d => d.UserId == userId);
        }
    }
}
