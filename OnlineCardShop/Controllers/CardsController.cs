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

    public class CardsController : Controller
    {
        private readonly ICardService cards;
        private readonly OnlineCardShopDbContext data;
        private readonly IWebHostEnvironment env;

        public CardsController(ICardService cards, OnlineCardShopDbContext data, IWebHostEnvironment env)
        {
            this.cards = cards;
            this.data = data;
            this.env = env;
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
                Categories = this.GetCardCategories(),
                Conditions = this.GetCardConditions()
            });
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Add(AddCardFormModel card, IFormFile imageFile)
        {
            var dealerId = this.data
                .Dealers
                .Where(d => d.UserId == this.User.GetId())
                .Select(d => d.Id)
                .FirstOrDefault();

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

            if (!ModelState.IsValid)
            {
                //The card object has null the categories and conditions, so I add them again
                card.Categories = this.GetCardCategories();
                card.Conditions = this.GetCardConditions();

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

                var newImage = new Data.Models.Image 
                { 
                    Name = imageName,
                    Path = imagePathForDb
                };

                this.data.Images.Add(newImage);

                using (var imageResized = SixLabors.ImageSharp.Image.Load(imageFile.OpenReadStream()))
                {
                    var resizedImagePath = imagePath.Split('\\');
                    resizedImagePath[resizedImagePath.Length - 1] = "res" + imageName;

                    var imageResizedPath = string.Join('\\', resizedImagePath);

                    imageResized.Mutate(i => i
                    .Resize(imageResized.Width/2, imageResized.Height/2)
                    .Crop(new Rectangle((imageResized.Width - 250) / 2, (imageResized.Height - 300) / 2, 250, 300)));

                    await imageResized.SaveAsync(imageResizedPath);
                }

                using (var fileStream = System.IO.File.Create(imagePath))
                {
                    await imageFile.CopyToAsync(fileStream);
                }

                var newCard = new Card
                {
                    Title = card.Title,
                    Price = card.Price,
                    Description = card.Description,
                    CategoryId = card.CategoryId,
                    ConditionId = card.ConditionId,
                    DealerId = dealerId,
                    Image = newImage
                };

                this.data.Cards.Add(newCard);
                this.data.SaveChanges();
            }

            return RedirectToAction("Index", "Home");
        }

        private static bool ImageIsWithinDesiredSize(IFormFile imageFile)
        {
            return imageFile.Length > 0 || imageFile.Length <= (2 * 1024 * 1024);
        }

        public string ResizeImage(SixLabors.ImageSharp.Image img, int maxWidth, int maxHeight)
        {
            if (img.Width > maxWidth || img.Height > maxHeight)
            {
                double widthRatio = (double)img.Width / (double)maxWidth;
                double heightRatio = (double)img.Height / (double)maxHeight;
                double ratio = Math.Max(widthRatio, heightRatio);
                int newWidth = (int)(img.Width / ratio);
                int newHeight = (int)(img.Height / ratio);
                return newHeight.ToString() + "," + newWidth.ToString();
            }
            else
            {
                return img.Height.ToString() + "," + img.Width.ToString();
            }
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

        private IEnumerable<CardCategoryViewModel> GetCardCategories()
        {
           return this.data
                .Categories
                .Select(c => new CardCategoryViewModel
                {
                    Id = c.Id,
                    Name = c.Name
                })
                .ToList();
        }

        private IEnumerable<CardConditionViewModel> GetCardConditions()
        {
            return this.data
                 .Conditions
                 .Select(c => new CardConditionViewModel
                 {
                     Id = c.Id,
                     Name = c.Name
                 })
                 .ToList();
        }
    }
}
