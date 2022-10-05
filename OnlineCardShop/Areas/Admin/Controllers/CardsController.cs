namespace OnlineCardShop.Areas.Admin.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using OnlineCardShop.Areas.Admin.Services;

    public class CardsController : AdminController
    {
        public readonly IAdminCardService cards;

        public CardsController(IAdminCardService cards)
        {
            this.cards = cards;
        }
        public IActionResult Index()
        {
            var cards = this.cards.All();

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
    }
}
