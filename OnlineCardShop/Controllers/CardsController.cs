namespace OnlineCardShop.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using OnlineCardShop.Data;
    using OnlineCardShop.Infrastructure;
    using OnlineCardShop.Models.Cards;
    using OnlineCardShop.Services.Cards;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using SixLabors.ImageSharp;
    using SixLabors.ImageSharp.Processing;
    using OnlineCardShop.Services.Dealers;

    using static WebConstants;

    public class CardsController : Controller
    {
        private readonly OnlineCardShopDbContext data;
        private readonly ICardService cards;
        private readonly IWebHostEnvironment env;
        private readonly IDealerService dealers;

        public CardsController(
            ICardService cards,
            IDealerService dealers, 
            IWebHostEnvironment env, 
            OnlineCardShopDbContext data)
        {
            this.cards = cards;
            this.data = data;
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

        public IActionResult Details([FromRoute] CardDetailsServiceModel query)
        {
            var requestingUserId = this.User.GetId();

            var card = this.cards.CardByUser(query.Id, requestingUserId);

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

            CardsToAddOnPage(query, queryResult);
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

            CardsToAddOnPage(query, queryResult);
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

            CardsToAddOnPage(query, queryResult);
            SelectCurrentPage(currentPage);

            return View(query); ;
        }

        [Authorize]
        public IActionResult Add()
        {
            if (!this.UserIsDealer())
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
            var dealerId = this.dealers.GetDealer(this.User.GetId());

            if (dealerId == 0)
            {
                return RedirectToAction(nameof(DealersController.Create), "Dealers");
            }

            if (!this.data.Categories.Any(c => c.Id == card.CategoryId))
            {
                this.ModelState.AddModelError(nameof(card.CategoryId), "Category does not exist");
            }

            if (!this.data.Conditions.Any(c => c.Id == card.ConditionId))
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
            var imageDirectory = ControllersConstants.CardsController.imageDirectory;

            if (ImageIsWithinDesiredSize(imageFile))
            {
                string originalImageName, imageName, imagePath, imagePathForDb;

                ProcessImageDetails(imageFile, wwwPath, imageDirectory, out originalImageName, out imageName, out imagePath, out imagePathForDb);

                var newImage = this.cards.CreateImage(imageName, imagePathForDb, originalImageName);

                this.data.Images.Add(newImage);

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

                var newCard = this.cards.CreateCard(card.Title,
                    card.Price,
                    card.Description,
                    card.CategoryId,
                    card.ConditionId,
                    dealerId,
                    newImage);

                this.data.Cards.Add(newCard);
                this.data.SaveChanges();
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

            return RedirectToAction("All", "Cards");
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
            var dealerId = this.dealers.GetDealer(this.User.GetId());

            if (dealerId == 0 && !User.IsAdmin())
            {
                return RedirectToAction(nameof(DealersController.Create), "Dealers");
            }

            if (!this.data.Categories.Any(c => c.Id == card.CategoryId))
            {
                this.ModelState.AddModelError(nameof(card.CategoryId), "Category does not exist");
            }

            if (!this.data.Conditions.Any(c => c.Id == card.ConditionId))
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
            var imageDirectory = ControllersConstants.CardsController.imageDirectory;

            Data.Models.Image newImage;

            if (imageFile != null)
            {
                if (ImageIsWithinDesiredSize(imageFile))
                {
                    string originalImageName, imageName, imagePath, imagePathForDb;

                    ProcessImageDetails(imageFile, wwwPath, imageDirectory, out originalImageName, out imageName, out imagePath, out imagePathForDb);

                    newImage = this.cards.CreateImage(imageName, imagePathForDb, originalImageName);

                    this.data.Images.Add(newImage);

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

        private static void ProcessImageDetails(IFormFile imageFile, string wwwPath, string imageDirectory, out string originalImageName, out string imageName, out string imagePath, out string imagePathForDb)
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

            if((imageResized.Width < 1024 || imageResized.Height < 1024) &&
                (imageResized.Width >= 696 || imageResized.Height >= 836))
            {

                imageResized.Mutate(i => i
                .Resize(418,348));
            }
            else
            {
                imageResized.Mutate(i => i
            .Resize(imageResized.Width / 2, imageResized.Height / 2)
            .Crop(new Rectangle((imageResized.Width - 348) / 2, (imageResized.Height - 418) / 2, 348, 418)));
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
            return image.Height >= 836 && image.Width >= 696;
        }

        private bool UserIsDealer()
        {
            return this.data
                .Dealers
                .Any(d => d.UserId == this.User.GetId());
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

        private static void CardsToAddOnPage(AllCardsServiceModel query, CardQueryServiceModel queryResult)
        {
            var cardsToAdd = queryResult.Cards
                            .Select(c => new CardServiceModel
                            {
                                Id = c.Id,
                                Title = c.Title,
                                Description = c.Description,
                                Category = c.Category,
                                Condition = c.Condition,
                                Price = c.Price,
                                Path = c.Path,
                                IsPublic = c.IsPublic
                            })
                            .ToList();

            query.TotalCards = queryResult.TotalCards;
            query.Cards = cardsToAdd;
        }
    }
}
