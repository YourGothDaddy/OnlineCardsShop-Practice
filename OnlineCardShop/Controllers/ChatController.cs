namespace OnlineCardShop.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using OnlineCardShop.Infrastructure;

    public class ChatController : Controller
    {
        [Authorize]
        public IActionResult Chat([FromRoute] string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return RedirectToAction("NoChats");
            }

            if (User.GetId() == id)
            {
                return RedirectToAction("Error");
            }

            return View();
        }

        [Authorize]
        public IActionResult NoChats()
        {
            return View();
        }

        [Authorize]
        public IActionResult Error()
        {
            return View();
        }
    }
}
