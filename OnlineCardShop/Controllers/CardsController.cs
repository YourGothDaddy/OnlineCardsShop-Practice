namespace OnlineCardShop.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using OnlineCardShop.Data;
    using OnlineCardShop.Data.Models;
    using OnlineCardShop.Infrastructure;
    using OnlineCardShop.Models.Cards;
    using OnlineCardShop.Services.Cards;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using SixLabors.ImageSharp;
    using System;
    using SixLabors.ImageSharp.Processing;
    using OnlineCardShop.Services.Dealers;

    public class CardsController : Controller
    {
        private readonly ICardService cards;
        private readonly OnlineCardShopDbContext data;
        private readonly IWebHostEnvironment env;
        private readonly IDealerService dealers;

        public CardsController(ICardService cards,
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

        public IActionResult Details([FromRoute] CardServiceModel query)
        {
            var card = this.cards.CardByUser(query.Id);

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
        public async Task<IActionResult> Add(AddCardFormModel card, IFormFile imageFile)
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
                var imageExtension = Path.GetExtension(imageFile.FileName);

                var imageName = Path.GetRandomFileName() + imageExtension;

                var imagePath = Path.Combine(wwwPath, imageDirectory, imageName);

                var imagePathForDb = imageDirectory + "/" + "res" + imageName;

                var newImage = this.cards.CreateImage(imageName, imagePathForDb);

                this.data.Images.Add(newImage);

                using (var imageResized = SixLabors.ImageSharp.Image.Load(imageFile.OpenReadStream()))
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

            return RedirectToAction("Index", "Home");
        }

        private async Task ResizeAndCropImage(SixLabors.ImageSharp.Image imageResized, string imageName, string imagePath)
        {
            var resizedImagePath = imagePath.Split('\\');
            resizedImagePath[resizedImagePath.Length - 1] = "res" + imageName;

            var imageResizedPath = string.Join('\\', resizedImagePath);

            imageResized.Mutate(i => i
            .Resize(imageResized.Width / 2, imageResized.Height / 2)
            .Crop(new Rectangle((imageResized.Width - 348) / 2, (imageResized.Height - 418) / 2, 348, 418)));

            await SaveImage(imageResized, imageResizedPath);
        }

        private static async Task SaveImage(SixLabors.ImageSharp.Image imageResized, string imageResizedPath)
        {
            await imageResized.SaveAsync(imageResizedPath);
        }

        private static bool ImageIsWithinDesiredSize(IFormFile imageFile)
        {
            return imageFile.Length > 0 && imageFile.Length <= (2 * 1024 * 1024);
        }

        private static bool ImageIsWithinDesiredRes(SixLabors.ImageSharp.Image image)
        {
            return image.Height >= 1024 && image.Width >= 1024;
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
                                Path = c.Path
                            })
                            .ToList();

            query.TotalCards = queryResult.TotalCards;
            query.Cards = cardsToAdd;
        }
    }
}
