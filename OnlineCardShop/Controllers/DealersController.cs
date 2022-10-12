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

    using static WebConstants;

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

            var currentDealerReviews = this.dealers.GetReviews(id);

            var submitters = this.dealers.GetSubmitters(currentDealerReviews);

            cardDealer.Reviews = currentDealerReviews;
            cardDealer.Submitters = submitters;
            cardDealer.TotalRating = this.dealers.GetTotalRating(id); 

            return View(cardDealer);
        }

        [HttpPost]
        [Authorize]
        public IActionResult Dealer(DealerServiceViewModel ratingFormData, [FromRoute] string id)
        {
            if (string.IsNullOrEmpty(ratingFormData.Description))
            {
                this.ModelState.AddModelError(nameof(ratingFormData.Description), "There is no review!");
            }

            if (ratingFormData.Rating <= 0)
            {
                this.ModelState.AddModelError(nameof(ratingFormData.Description), "There is no rating!");
            }

            if(!ModelState.IsValid)
            {
                return View(ratingFormData);
            }

            this.dealers.AddReview(ratingFormData.Description, ratingFormData.Rating, id, User);
            this.dealers.UpdateRatings(id);

            TempData[GlobalMessage] = "Your rating was submitted! Thank you!";
            return RedirectToAction("Dealer", "Dealers");
        }

        public IActionResult Chat()
        {
            return View();
        }

        public bool IsDealer(string userId)
        {
            return this.data
                .Dealers
                .Any(d => d.UserId == userId);
        }
    }
}
