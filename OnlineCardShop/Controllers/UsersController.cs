namespace OnlineCardShop.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using OnlineCardShop.Services.Users;

    public class UsersController : Controller
    {
        private readonly IUserService users;

        public UsersController(IUserService users)
        {
            this.users = users;
        }

        [Authorize]
        public IActionResult Details([FromRoute] string id)
        {
            var userDetails = this.users.GetUserDetails(id);

            return View(userDetails);
        }

        [Authorize]
        [HttpPost]
        public IActionResult Details(UserDetailsServiceViewModel model, [FromRoute] string id)
        {
            var reportReason = model.ReportReason;

            this.users.SaveReport(reportReason, id);

            return View();
        }
    }
}
