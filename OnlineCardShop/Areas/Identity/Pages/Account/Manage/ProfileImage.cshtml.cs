using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OnlineCardShop.Data.Models;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace OnlineCardShop.Areas.Identity.Pages.Account.Manage
{
    public class ProfileImage : PageModel
    {
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;

        public ProfileImage(UserManager<User> userManager,
            SignInManager<User> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        public class ChangeProfileImageFormModel
        {
            [Display(Name = "Profile Image")]
            public Data.Models.ProfileImage ProfileImage { get; init; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await userManager.GetUserAsync(User);
            if(user == null)
            {
                return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
            }

            return Page();
        }
    }
}
