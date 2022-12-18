namespace OnlineCardShop.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using SixLabors.ImageSharp;
    using SixLabors.ImageSharp.Processing;
    using SixLabors.ImageSharp.Metadata.Profiles.Exif;
    using OnlineCardShop.Services.Dealers;
    using OnlineCardShop.Data;
    using OnlineCardShop.Infrastructure;
    using OnlineCardShop.Models.Cards;
    using OnlineCardShop.Services.Cards;

    using static WebConstants;
    using static OnlineCardShop.Services.Cards.CardsControllerConstants;
    using Image = SixLabors.ImageSharp.Image;

    public class CardsController : Controller
    {
        private readonly ICardService cards; 
        private readonly IWebHostEnvironment env;
        private readonly IDealerService dealers;

        public CardsController(
            ICardService cards,
            IDealerService dealers, 
            IWebHostEnvironment env)
        {
            this.cards = cards;
            this.env = env;
            this.dealers = dealers;
        }

        [Authorize]
        public IActionResult Mine([FromQuery] int currentPage)
        {
            if (currentPage == 0)
            {
                currentPage = 1;
            }

            var myCards = this.cards.ByUser(this.User.GetId(),
                currentPage,
                AllCardsServiceModel.CardsPerPage);

            SelectCurrentPage(currentPage);

            return View(myCards);
        }

        [Authorize]
        public IActionResult Details([FromRoute] CardDetailsServiceModel query)
        {
            var requestingUserId = this.User.GetId();

            var card = this.cards.CardByUser(query.Id, requestingUserId);

            var requestingUserIsNotTheUserCreatedTheCard = requestingUserId != card.UserId;

            if (card.IsDeleted || !card.IsPublic)
            {
                if(requestingUserIsNotTheUserCreatedTheCard && !User.IsAdmin())
                {
                    return View("~/Views/Shared/_userError.cshtml");
                }
                else
                {
                    return View(card);
                }
            }

            if(card == null)
            {
                return View("~/Views/Shared/_userError.cshtml");
            }

            return View(card);
        }

        public IActionResult Game([FromQuery] AllCardsServiceModel query, [FromQuery] int currentPage)
        {
            var queryResult = this.cards.All(
                query.SearchTerm,
                query.Sorting,
                query.CurrentPage,
                AllCardsServiceModel.CardsPerPage,
                2,
                query.Order);

            this.cards.GetCardsToAddOnPage(query, queryResult);
            SelectCurrentPage(currentPage);

            return View(query);
        }

        public IActionResult Kpop([FromQuery] AllCardsServiceModel query, [FromQuery] int currentPage)
        {
            var queryResult = this.cards.All(
                query.SearchTerm,
                query.Sorting,
                query.CurrentPage,
                AllCardsServiceModel.CardsPerPage,
                1,
                query.Order);

            this.cards.GetCardsToAddOnPage(query, queryResult);
            SelectCurrentPage(currentPage);
            return View(query); ;
        }

        public IActionResult All([FromQuery] AllCardsServiceModel query, [FromQuery]int currentPage)
        {
            var queryResult = this.cards.All(
                query.SearchTerm,
                query.Sorting,
                query.CurrentPage,
                AllCardsServiceModel.CardsPerPage,
                null,
                null);

            this.cards.GetCardsToAddOnPage(query, queryResult);
            SelectCurrentPage(currentPage);

            return View(query); ;
        }

        [Authorize]
        public IActionResult Add()
        {
            if (!this.cards.UserIsDealer(this.User.GetId()))
            {
                return RedirectToAction(nameof(DealersController.Create), "Dealers");
            }

            return View(new AddCardFormModel
            {
                Categories = this.cards.GetCardCategories(),
                Conditions = this.cards.GetCardConditions()
            });
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddAsync(AddCardFormModel card, IFormFile imageFile)
        {
            var dealerId = this.dealers.GetDealerId(this.User.GetId());

            if (dealerId == 0)
            {
                return RedirectToAction(nameof(DealersController.Create), "Dealers");
            }

            if (!this.cards.CategoryExist(card.CategoryId))
            {
                this.ModelState.AddModelError(nameof(card.CategoryId), "Category does not exist");
            }

            if (!this.cards.ConditionExist(card.ConditionId))
            {
                this.ModelState.AddModelError(nameof(card.ConditionId), "Condition does not exist");
            }

            if (imageFile == null)
            {
                this.ModelState.AddModelError(nameof(imageFile), "There is no image selected");
            }

            if (!ModelState.IsValid)
            {
                //The card object has null the categories and conditions, so I add them again
                card.Categories = this.cards.GetCardCategories();
                card.Conditions = this.cards.GetCardConditions();

                return View(card);
            }

            var wwwPath = this.env.WebRootPath;
            var imageDirectory = WebConstants.imageDirectory;

            if (ImageIsWithinDesiredSize(imageFile))
            {
                string originalImageName, imageName, imagePath, imagePathForDb;

                ProcessImageDetails(imageFile, wwwPath, imageDirectory, out originalImageName, out imageName, out imagePath, out imagePathForDb);

                var newImage = this.cards.CreateImage(imageName, imagePathForDb, originalImageName);

                this.cards.AddImageToDB(newImage);

                using (var imageResized = Image.Load(imageFile.OpenReadStream()))
                {
                    if (!ImageIsWithinDesiredRes(imageResized))
                    {
                        this.ModelState.AddModelError(nameof(imageFile), "Image should be at least 836x696");

                        //The card object has null the categories and conditions, so I add them again
                        card.Categories = this.cards.GetCardCategories();
                        card.Conditions = this.cards.GetCardConditions();

                        return View(card);
                    }

                    await ResizeAndCropImage(imageResized, imageName, imagePath);
                }

                using (var fileStream = System.IO.File.Create(imagePath))
                {
                    await imageFile.CopyToAsync(fileStream);
                }

                var newCard = this.cards.CreateCard(card.Title,
                    card.Price,
                    card.Description,
                    card.CategoryId,
                    card.ConditionId,
                    dealerId,
                    newImage);

                this.cards.AddCardToDB(newCard);
            }
            else
            {
                this.ModelState.AddModelError(nameof(imageFile), "The image is too big! The max size is 2MB");

                //The card object has null the categories and conditions, so I add them again
                card.Categories = this.cards.GetCardCategories();
                card.Conditions = this.cards.GetCardConditions();

                return View(card);
            }

            TempData[GlobalMessage] = "You card will be public after an Administrator approves it!";

            return RedirectToAction("Mine", "Cards");
        }

        [Authorize]
        public IActionResult Delete([FromRoute]int id)
        {
            this.cards.DeleteCard(id);

            TempData[GlobalMessage] = "You have successfully deleted the card!";

            return RedirectToAction("All", "Cards");
        }

        [Authorize]
        public IActionResult Edit(int id)
        {
            var userId = this.User.GetId();

            if (!this.dealers.IsDealer(userId) && !User.IsAdmin())
            {
                return RedirectToAction(nameof(DealersController.Create), "Dealers");
            }

            var card = this.cards.CardByUser(id, userId);

            if (card.UserId != userId && !User.IsAdmin())
            {
                return Unauthorized();
            }

            return View(new AddCardFormModel
            {
                Title = card.Title,
                Price = card.Price,
                Description = card.Description,
                Categories = this.cards.GetCardCategories(),
                Conditions = this.cards.GetCardConditions()
            });
        }

        [HttpPost]
        [Authorize]

        public async Task<IActionResult> EditAsync(int id, AddCardFormModel card, IFormFile imageFile)
        {
            var dealerId = this.dealers.GetDealerId(this.User.GetId());

            if (dealerId == 0 && !User.IsAdmin())
            {
                return RedirectToAction(nameof(DealersController.Create), "Dealers");
            }

            if (!this.cards.CategoryExist(card.CategoryId))
            {
                this.ModelState.AddModelError(nameof(card.CategoryId), "Category does not exist");
            }

            if (!this.cards.ConditionExist(card.ConditionId))
            {
                this.ModelState.AddModelError(nameof(card.ConditionId), "Condition does not exist");
            }

            if (!ModelState.IsValid)
            {
                //The card object has null the categories and conditions, so I add them again
                card.Categories = this.cards.GetCardCategories();
                card.Conditions = this.cards.GetCardConditions();

                return View(card);
            }

            if (!this.cards.CardIsByDealer(id, dealerId) && !User.IsAdmin())
            {
                return BadRequest();
            }

            var wwwPath = this.env.WebRootPath;
            var imageDirectory = WebConstants.imageDirectory;

            Data.Models.Image newImage;

            if (imageFile != null)
            {
                if (ImageIsWithinDesiredSize(imageFile))
                {
                    string originalImageName, imageName, imagePath, imagePathForDb;

                    ProcessImageDetails(imageFile, wwwPath, imageDirectory, out originalImageName, out imageName, out imagePath, out imagePathForDb);

                    newImage = this.cards.CreateImage(imageName, imagePathForDb, originalImageName);

                    this.cards.AddImageToDB(newImage);

                    using (var imageResized = Image.Load(imageFile.OpenReadStream()))
                    {
                        if (!ImageIsWithinDesiredRes(imageResized))
                        {
                            this.ModelState.AddModelError(nameof(imageFile), "Image should be at least 1024x1024");

                            //The card object has null the categories and conditions, so I add them again
                            card.Categories = this.cards.GetCardCategories();
                            card.Conditions = this.cards.GetCardConditions();

                            return View(card);
                        }

                        await ResizeAndCropImage(imageResized, imageName, imagePath);
                    }

                    using (var fileStream = System.IO.File.Create(imagePath))
                    {
                        await imageFile.CopyToAsync(fileStream);
                    }
                }

                else
                {
                    this.ModelState.AddModelError(nameof(imageFile), "The image is too big! The max size is 2MB");

                    //The card object has null the categories and conditions, so I add them again
                    card.Categories = this.cards.GetCardCategories();
                    card.Conditions = this.cards.GetCardConditions();

                    return View(card);
                }


                this.cards.EditCard(card.id,
                        card.Title,
                        card.Price,
                        card.Description,
                        card.CategoryId,
                        card.ConditionId,
                        newImage);

                TempData[GlobalMessage] = "You have sucessfully edited the card!";

                return RedirectToAction(nameof(All));
            }
            else
            {
                this.cards.EditCard(card.id,
                        card.Title,
                        card.Price,
                        card.Description,
                        card.CategoryId,
                        card.ConditionId,
                        null);

                TempData[GlobalMessage] = "You have sucessfully edited the card!";

                return RedirectToAction(nameof(All));
            }
        }

        public static void ProcessImageDetails(IFormFile imageFile, string wwwPath, string imageDirectory, out string originalImageName, out string imageName, out string imagePath, out string imagePathForDb)
        {
            var imageExtension = Path.GetExtension(imageFile.FileName);

            originalImageName = imageFile.FileName;
            imageName = Path.GetRandomFileName() + imageExtension;
            imagePath = Path.Combine(wwwPath, imageDirectory, imageName);
            imagePathForDb = imageDirectory + "/" + "res" + imageName;
        }

        private async Task ResizeAndCropImage(Image imageResized, string imageName, string imagePath)
        {
            var resizedImagePath = imagePath.Split('\\');
            resizedImagePath[resizedImagePath.Length - 1] = "res" + imageName;

            var imageResizedPath = string.Join('\\', resizedImagePath);

            if(imageResized.Width < 1024 || imageResized.Height < 1024)
            {
                IExifValue exifOrientation = imageResized.Metadata?.ExifProfile?.GetValue(ExifTag.Orientation);

                if(exifOrientation != null)
                {
                    var exifValue = exifOrientation.GetValue().ToString();
                    if (exifValue == "8")
                    {
                        imageResized.Mutate(i => i
                        .Resize(418, minRenderedImageHeight));
                    }
                }
                else
                {
                    imageResized.Mutate(i => i
                    .Resize(minRenderedImageHeight, minRenderedImageWidth));
                }
            }
            else
            {
                imageResized.Mutate(i => i
            .Resize(imageResized.Width / 2, imageResized.Height / 2)
            .Crop(new Rectangle((imageResized.Width - minRenderedImageHeight) / 2, (imageResized.Height - minRenderedImageWidth) / 2, minRenderedImageHeight, minRenderedImageWidth)));
            }

            await SaveImage(imageResized, imageResizedPath);
        }

        private static async Task SaveImage(Image imageResized, string imageResizedPath)
        {
            await imageResized.SaveAsync(imageResizedPath);
        }

        private static bool ImageIsWithinDesiredSize(IFormFile imageFile)
        {
            return imageFile.Length > 0 && imageFile.Length <= (2 * 1024 * 1024);
        }

        private static bool ImageIsWithinDesiredRes(Image image)
        {
            return image.Height >= minImageHeight && image.Width >= minImageWidth;
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
    }
}
