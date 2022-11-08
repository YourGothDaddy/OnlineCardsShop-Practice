namespace OnlineCardShop.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using OnlineCardShop.Services.Users;

    public class UsersController : Controller
    {
        private readonly IUserService users;

        public UsersController(IUserService users)
        {
            this.users = users;
        }

        public IActionResult Details([FromRoute] string id)
        {
            var userDetails = this.users.GetUserDetails(id);

            return View(userDetails);
        }

        public IActionResult Chat([FromRoute] string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return RedirectToAction("NoChats");
            }

            return View();
        }

        public IActionResult NoChats()
        {
            return View();
        }
    }
}
