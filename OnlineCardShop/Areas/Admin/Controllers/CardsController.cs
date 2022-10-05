namespace OnlineCardShop.Areas.Admin.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using OnlineCardShop.Areas.Admin.Services;
    using OnlineCardShop.Areas.Admin.Services.Cards;
    using System.Linq;

    public class CardsController : AdminController
    {
        public readonly IAdminCardService cards;

        public CardsController(IAdminCardService cards)
        {
            this.cards = cards;
        }
        public IActionResult Index([FromQuery] AllCardsServiceModel query, [FromQuery] int currentPage)
        {
            var cards = this.cards.All(query.CurrentPage, AllCardsServiceModel.CardsPerPage);

            CardsToAddOnPage(query, cards);
            SelectCurrentPage(currentPage);

            return View(cards);
        }
        public IActionResult Delete([FromRoute] int id)
        {
            this.cards.DeleteCard(id);

            TempData[WebConstants.GlobalMessage] = "You have successfully deleted the card!";

            return RedirectToAction("Index", "Cards");
        }

        public IActionResult Approve([FromRoute] int id)
        {
            this.cards.ApproveCard(id);

            TempData[WebConstants.GlobalMessage] = "You have approved the card!";

            return RedirectToAction("Index", "Cards");
        }

        public IActionResult Hide([FromRoute] int id)
        {
            this.cards.HideCard(id);

            TempData[WebConstants.GlobalMessage] = "You have hid the card!";

            return RedirectToAction("Index", "Cards");
        }

        private void SelectCurrentPage(int currentPage)
        {
            if (currentPage == 0)
            {
                ViewBag.CurrentPage = 1;
            }
            else
            {
                ViewBag.CurrentPage = currentPage;
            }
        }

        private static void CardsToAddOnPage(AllCardsServiceModel query, AllCardsServiceModel queryResult)
        {
            var cardsToAdd = queryResult.Cards
                            .Select(c => new CardServiceModel
                            {
                                Id = c.Id,
                                Title = c.Title,
                                DealerId = c.DealerId,
                                IsPublic = c.IsPublic,
                                IsDeleted = c.IsDeleted
                            })
                            .ToList();

            query.TotalCards = queryResult.TotalCards;
            query.Cards = cardsToAdd;
        }
    }
}
