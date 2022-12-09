namespace OnlineCardShop.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    public class ChatController : Controller
    {
        [Authorize]
        public IActionResult Chat([FromRoute] string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return RedirectToAction("NoChats");
            }

            return View();
        }

        [Authorize]
        public IActionResult NoChats()
        {
            return View();
        }
    }
}
