﻿using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OnlineCardShop.Data.Models;
using OnlineCardShop.Services.Users;

namespace OnlineCardShop.Areas.Identity.Pages.Account.Manage
{
    public partial class IndexModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IUserService users;

        public IndexModel(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IUserService users)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            this.users = users;
        }

        public string Username { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }

            [Display(Name = "About Me")]
            public string AboutMe { get; set; }
        }

        private async Task LoadAsync(User user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            var aboutMe = this.users.GetAboutMe(User.FindFirstValue(ClaimTypes.NameIdentifier));

            Username = userName;

            Input = new InputModel
            {
                PhoneNumber = phoneNumber,
                AboutMe = aboutMe
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    StatusMessage = "Unexpected error when trying to set phone number.";
                    return RedirectToPage();
                }
            }

            var currentAboutMe = this.users.GetAboutMe(User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (Input.AboutMe != currentAboutMe)
            {
                this.users.SetAboutMe(User.FindFirstValue(ClaimTypes.NameIdentifier), Input.AboutMe);
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
    }
}
