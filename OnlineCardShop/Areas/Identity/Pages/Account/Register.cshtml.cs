namespace OnlineCardShop.Areas.Identity.Pages.Account
{
    using System.ComponentModel.DataAnnotations;
    using System.IO;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.RazorPages;
    using OnlineCardShop.Controllers;
    using OnlineCardShop.Data;
    using OnlineCardShop.Data.Models;
    using OnlineCardShop.Services.Cards;

    using static OnlineCardShop.Data.DataConstants.User;

    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly UserManager<User> userManager;
        private readonly IWebHostEnvironment env;
        private readonly ICardService cards;
        private readonly OnlineCardShopDbContext data;

        public RegisterModel(
            UserManager<User> userManager,
            OnlineCardShopDbContext data,
            IWebHostEnvironment env,
            ICardService cards)
        {
            this.userManager = userManager;
            this.env = env;
            this.cards = cards;
            this.data = data;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [Display(Name = "Full Name")]
            [StringLength(MaxNameLength, MinimumLength = MinNameLength)]
            public string FullName { get; set; }

            [Required]
            [Display(Name = "Profile Image")]
            public IFormFile ProfileImage { get; set; }

            [Required]
            [StringLength(PasswordMaxLength, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = PasswordMinLength)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
        }

        public void OnGet(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            if (Input.ProfileImage == null)
            {
                this.ModelState.AddModelError(nameof(Input.ProfileImage), "There is no image selected");
            }

                if (ModelState.IsValid)
            {

                var wwwPath = this.env.WebRootPath;
                var imageDirectory = ControllersConstants.CardsController.profileImageDirectory;
                ProfileImage profileImage = new ProfileImage();

                if (ImageIsWithinDesiredSize(Input.ProfileImage))
                {
                    string originalImageName, imageName, imagePath, imagePathForDb;

                    ProcessImageDetails(Input.ProfileImage, wwwPath, imageDirectory, out originalImageName, out imageName, out imagePath, out imagePathForDb);

                    profileImage = this.cards.CreateProfileImage(imageName, imagePathForDb, originalImageName);

                    this.data.ProfileImages.Add(profileImage);

                    using (var fileStream = System.IO.File.Create(imagePath))
                    {
                        await Input.ProfileImage.CopyToAsync(fileStream);
                    }
                }

                var user = new User
                {
                    UserName = Input.Email,
                    Email = Input.Email,
                    FullName = Input.FullName,
                    ProfileImage = profileImage
                };

                var result = await userManager.CreateAsync(user, Input.Password);
                
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return Page();
        }

        private static bool ImageIsWithinDesiredSize(IFormFile imageFile)
        {
            return imageFile.Length > 0 && imageFile.Length <= (2 * 1024 * 1024);
        }

        private static void ProcessImageDetails(IFormFile imageFile, string wwwPath, string imageDirectory, out string originalImageName, out string imageName, out string imagePath, out string imagePathForDb)
        {
            var imageExtension = Path.GetExtension(imageFile.FileName);

            originalImageName = imageFile.FileName;
            imageName = Path.GetRandomFileName() + imageExtension;
            imagePath = Path.Combine(wwwPath, imageDirectory, imageName);
            imagePathForDb = imageDirectory + "/" + "res" + imageName;
        }
    }
}
