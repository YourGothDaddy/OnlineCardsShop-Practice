namespace OnlineCardShop.Areas.Admin.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    public class DealersController : AdminController
    {
        public IActionResult Dealer()
        {
            return View();
        }
    }
}
