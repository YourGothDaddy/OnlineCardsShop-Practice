namespace OnlineCardShop.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using OnlineCardShop.Data;
    using OnlineCardShop.Data.Models;
    using OnlineCardShop.Infrastructure;
    using OnlineCardShop.Models.Dealers;
    using System.Linq;

    public class DealersController : Controller
    {
        private readonly OnlineCardShopDbContext data;

        public DealersController(OnlineCardShopDbContext data)
        {
            this.data = data;
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

        public bool IsDealer(string userId)
        {
            return this.data
                .Dealers
                .Any(d => d.UserId == userId);
        }
    }
}
