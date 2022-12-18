namespace OnlineCardShop.Tests.Controllers
{
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Moq;
    using MyTested.AspNetCore.Mvc;
    using OnlineCardShop.Controllers;
    using OnlineCardShop.Data.Models;
    using OnlineCardShop.Models.Cards;
    using OnlineCardShop.Services.Cards;
    using OnlineCardShop.Services.Dealers;
    using System.IO;
    using Xunit;
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Mvc;
    using System.Security.Claims;
    using System.Linq;
    using OnlineCardShop.Tests.Mocks;

    public class CardsControllerTests
    {
        [Fact]
        public void GetMineShouldBeForAuthorizedUsers()
        {
            MyController<CardsController>
                .Instance()
                .Calling(c => c.Mine(1))
                .ShouldHave()
                .ActionAttributes(attributes => attributes
                    .RestrictingForAuthorizedRequests());
        }

        [Fact]
        public void GetMineShouldReturnView()
        {
            MyController<CardsController>
                .Instance()
                .Calling(c => c.Mine(1))
                .ShouldReturn()
                .View();
        }

        [Fact]
        public void GetGameShouldReturnView()
        {
            MyController<CardsController>
                .Instance()
                .Calling(c => c.Game(new AllCardsServiceModel { }, 1))
                .ShouldReturn()
                .View();
        }

        [Fact]
        public void GetKpopShouldReturnView()
        {
            MyController<CardsController>
                .Instance()
                .Calling(c => c.Kpop(new AllCardsServiceModel { }, 1))
                .ShouldReturn()
                .View();
        }

        [Fact]
        public void GetAllShouldReturnView()
        {
            MyController<CardsController>
                .Instance()
                .Calling(c => c.All(new AllCardsServiceModel { }, 1))
                .ShouldReturn()
                .View();
        }

        [Fact]
        public void GetAddShouldReturnView()
        {
            MyController<CardsController>
                .Instance(controller => controller
                    .WithData(new Dealer
                    {
                        UserId = "TestId"
                    })
                    .WithUser())
                .Calling(c => c.Add())
                .ShouldReturn()
                .View(view => view
                    .WithModelOfType<AddCardFormModel>());
        }

        [Fact]
        public void GetAddShouldReturnRedirectToAction()
        {
            MyController<CardsController>
                .Instance()
                .Calling(c => c.Add())
                .ShouldReturn()
                .RedirectToAction("Create", "Dealers");
        }

        [Fact]
        public void GetDeleteShouldReturnRedirectToAction()
        {
            MyController<CardsController>
                .Instance(controller => controller
                    .WithData(new Card { }))
                .Calling(c => c.Delete(1))
                .ShouldReturn()
                .RedirectToAction("All", "Cards");
        }

        [Fact]
        public void GetEditShouldReturnRedirectToActionWhenTheUserIsNotADealerAndIsNotAnAdmin()
        {
            MyController<CardsController>
                .Instance()
                .Calling(c => c.Edit(1))
                .ShouldReturn()
                .RedirectToAction("Create", "Dealers");
        }

        [Fact]
        public void AddAsyncShouldRedirectToCreateDealersActionIfDealerIdIsEqualTo0()
        {
            MyController<CardsController>
                .Instance(controller => controller
                    .WithUser()
                    .WithData(new Dealer { Id = 0 }))
                .Calling(c => c.AddAsync(new AddCardFormModel { }, null))
                .ShouldReturn()
                .RedirectToAction("Create", "Dealers");
        }

        [Fact]
        public void AddAsyncShouldReturnInvalidModelStateAndViewIfTheImageFileIsNull()
        {
            MyController<CardsController>
                .Instance(controller => controller
                    .WithUser()
                    .WithData(new Category { Id = 1 })
                    .WithData(new Condition { Id = 1 })
                    .WithData(new Dealer { Id = 1, UserId = "TestId" }))
                .Calling(c => c.AddAsync(
                    new AddCardFormModel 
                    { 
                        CategoryId = 1,
                        ConditionId = 1 
                    }, null))
                .ShouldHave()
                .InvalidModelState()
                .AndAlso()
                .ShouldReturn()
                .View(view => view
                    .WithModelOfType<AddCardFormModel>());
        }

        [Fact]
        public void AddAsyncShouldReturnInvalidModelStateAndViewIfTheCategoriesAndImageFileAreNull()
        {
            MyController<CardsController>
                .Instance(controller => controller
                    .WithUser()
                    .WithData(new Condition { Id = 1 })
                    .WithData(new Dealer { Id = 1, UserId = "TestId" }))
                .Calling(c => c.AddAsync(new AddCardFormModel { ConditionId = 1 }, null))
                .ShouldHave()
                .InvalidModelState()
                .AndAlso()
                .ShouldReturn()
                .View(view => view
                    .WithModelOfType<AddCardFormModel>());
        }

        [Fact]
        public void AddAsyncShouldReturnInvalidModelStateAndViewIfTheConditionsAndImageFileAreNull()
        {
            MyController<CardsController>
                .Instance(controller => controller
                    .WithUser()
                    .WithData(new Category { Id = 1 })
                    .WithData(new Dealer { Id = 1, UserId = "TestId" }))
                .Calling(c => c.AddAsync(new AddCardFormModel { CategoryId = 1 }, null))
                .ShouldHave()
                .InvalidModelState()
                .AndAlso()
                .ShouldReturn()
                .View(view => view
                    .WithModelOfType<AddCardFormModel>());
        }

        [Fact]
        public void AddAsyncShouldReturnInvalidModelStateAndViewWithoutDescription()
        {
            MyController<CardsController>
                .Instance(controller => controller
                    .WithUser()
                    .WithData(new Category { Id = 1 })
                    .WithData(new Condition { Id = 1 })
                    .WithData(new Dealer { Id = 1, UserId = "TestId" }))
                .Calling(c => c.AddAsync(
                    new AddCardFormModel
                    {
                        Title = "TestTitle",
                        CategoryId = 1,
                        ConditionId = 1
                    }, new FormFile(null, 0, 0, "", "")))
                .ShouldHave()
                .InvalidModelState()
                .AndAlso()
                .ShouldReturn()
                .View(view => view
                    .WithModelOfType<AddCardFormModel>());
        }

        [Fact]
        public void AddAsyncShouldReturnInvalidModelStateAndViewWithoutTitle()
        {
            MyController<CardsController>
                .Instance(controller => controller
                    .WithUser()
                    .WithData(new Category { Id = 1 })
                    .WithData(new Condition { Id = 1 })
                    .WithData(new Dealer { Id = 1, UserId = "TestId" }))
                .Calling(c => c.AddAsync(
                    new AddCardFormModel
                    {
                        Description = "TestDescription",
                        CategoryId = 1,
                        ConditionId = 1
                    }, new FormFile(null, 0, 0, "", "")))
                .ShouldHave()
                .InvalidModelState()
                .AndAlso()
                .ShouldReturn()
                .View(view => view
                    .WithModelOfType<AddCardFormModel>());
        }

        [Fact]
        public void AddAsyncShouldReturnInvalidModelStateAndViewIfTheImageSizeIsNotInRange()
        {
            MyController<CardsController>
                .Instance(controller => controller
                    .WithUser()
                    .WithData(new Category { Id = 1 })
                    .WithData(new Condition { Id = 1 })
                    .WithData(new Dealer { Id = 1, UserId = "TestId" }))
                .Calling(c => c.AddAsync(
                    new AddCardFormModel
                    {
                        Title = "TestTitle",
                        Description = "TestDescription",
                        CategoryId = 1,
                        ConditionId = 1
                    }, new FormFile(null, 0, 0, "", "")))
                .ShouldHave()
                .InvalidModelState()
                .AndAlso()
                .ShouldReturn()
                .View(view => view
                    .WithModelOfType<AddCardFormModel>());
        }

        [Fact]
        public void ProcessImageDetailsShouldProcessImageDetails()
        {
            // Arrange
            var imageFileMock = new Mock<IFormFile>();
            var wwwPath = @"C:\www";
            var imageDirectory = "images";
            string originalImageName;
            string imageName;
            string imagePath;
            string imagePathForDb;

            // Setup the mock IFormFile object
            imageFileMock
                .Setup(f => f.FileName)
                .Returns("test.jpg");

            // Act
            CardsController.ProcessImageDetails(imageFileMock.Object, wwwPath, imageDirectory, out originalImageName, out imageName, out imagePath, out imagePathForDb);

            // Assert
            Assert.True(originalImageName != imageName);
        }

        [Fact]
        public void EditAsyncShouldHaveRedirectResultWhenDealerIdIsZeroAndUserIsNotAdmin()
        {
            MyMvc
                .Controller<CardsController>()
                .WithUser(user => user.WithClaim("IsAdmin", "false"))
                .Calling(c => c.EditAsync(0, null, null))
                .ShouldHave()
                .ActionAttributes(attributes => attributes
                    .RestrictingForAuthorizedRequests())
                .AndAlso()
                .ShouldReturn()
                .Redirect();
        }

        [Fact]
        public void EditAsyncShouldReturnViewWithModelWhenModelStateIsInvalid()
        {
            var model = new AddCardFormModel
            {
                CategoryId = 1,
                Categories = new List<CardCategoryServiceViewModel>()
            };

            MyMvc
                .Controller<CardsController>()
                .Calling(c => c.EditAsync(1, model, null))
                .ShouldReturn()
                .RedirectToAction("Create");
        }

        [Fact]
        public void EditAsyncShouldReturnRedirectToActionResultWhenUserIsNotAdminAndDoesNotHaveDealerId()
        {
            MyMvc
                .Controller<CardsController>()
                .WithUser()
                .Calling(c => c.EditAsync(1, new AddCardFormModel(), null))
                .ShouldReturn()
                .RedirectToAction("Create", "Dealers");
        }

        [Fact]
        public async void EditAsyncShouldReturnViewResultWithInvalidModelStateWhenCategoryIdIsInvalid()
        {
            // Arrange
            using var data = DatabaseMock.Instance;

            data.Categories.Add(new Category { Id = 1, Name = "test" });
            data.Conditions.Add(new Condition { Id = 1, Name = "test" });
            data.SaveChanges();

            var cardConditionServiceViewModel = new CardConditionServiceViewModel
            {
                Id = 1,
                Name = "test"
            };

            var cardCategoryServiceViewModel = new CardCategoryServiceViewModel
            {
                Id = 1,
                Name = "test"
            };

            var cardFormModel = new AddCardFormModel
            { 
                CategoryId = 999, 
                Conditions = new List<CardConditionServiceViewModel>(),
                Categories = new List<CardCategoryServiceViewModel>()
            }; 
            var imageFile = new FormFile(new MemoryStream(), 0, 0, "file", "file");

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(ClaimTypes.Name, "username"),
            };

            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            var dealerServiceMock = new Mock<IDealerService>();
            dealerServiceMock
                .Setup(service => service.GetDealerId(It.IsAny<string>()))
                .Returns(1);

            var categoriesServiceViewModel = new List<CardCategoryServiceViewModel> { cardCategoryServiceViewModel };
            var conditionsServiceViewModel = new List<CardConditionServiceViewModel> { cardConditionServiceViewModel };

            var cardsServiceMock = new Mock<ICardService>();
            cardsServiceMock
                .Setup(service => service.GetCardCategories())
                .Returns(categoriesServiceViewModel);

            cardsServiceMock
                .Setup(service => service.GetCardConditions())
                .Returns(conditionsServiceViewModel);

            var controller = new CardsController(cardsServiceMock.Object, dealerServiceMock.Object, null, data);

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = claimsPrincipal
                }
            };

            // Act
            var result = await controller.EditAsync(1, cardFormModel, imageFile);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsType<AddCardFormModel>(viewResult.Model);
            var model = (AddCardFormModel)viewResult.Model;
            //Assert.True(model.Categories != null && model.Conditions != null && !result.ViewData.ModelState.IsValid);
        }

        [Fact]
        public async void EditAsyncShouldReturnViewResultWithInvalidModelStateWhenConditionIdIsInvalid()
        {
            // Arrange
            using var data = DatabaseMock.Instance;

            data.Categories.Add(new Category { Id = 1, Name = "test" });
            data.Conditions.Add(new Condition { Id = 1, Name = "test" });
            data.SaveChanges();

            var cardConditionServiceViewModel = new CardConditionServiceViewModel
            {
                Id = 1,
                Name = "test"
            };

            var cardCategoryServiceViewModel = new CardCategoryServiceViewModel
            {
                Id = 1,
                Name = "test"
            };

            var cardFormModel = new AddCardFormModel
            {
                ConditionId = 999,
                Conditions = new List<CardConditionServiceViewModel>(),
                Categories = new List<CardCategoryServiceViewModel>()
            };
            var imageFile = new FormFile(new MemoryStream(), 0, 0, "file", "file");

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(ClaimTypes.Name, "username"),
            };

            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            var dealerServiceMock = new Mock<IDealerService>();
            dealerServiceMock
                .Setup(service => service.GetDealerId(It.IsAny<string>()))
                .Returns(1);

            var categoriesServiceViewModel = new List<CardCategoryServiceViewModel> { cardCategoryServiceViewModel };
            var conditionsServiceViewModel = new List<CardConditionServiceViewModel> { cardConditionServiceViewModel };

            var cardsServiceMock = new Mock<ICardService>();
            cardsServiceMock
                .Setup(service => service.GetCardCategories())
                .Returns(categoriesServiceViewModel);

            cardsServiceMock
                .Setup(service => service.GetCardConditions())
                .Returns(conditionsServiceViewModel);

            var controller = new CardsController(cardsServiceMock.Object, dealerServiceMock.Object, null, data);

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = claimsPrincipal
                }
            };

            // Act
            var result = await controller.EditAsync(1, cardFormModel, imageFile);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsType<AddCardFormModel>(viewResult.Model);
            
        }

        [Fact]
        public async void EditAsyncShouldReturnBadRequestIfGivenCardIsNotByDealerAndUserIsNotAnAdmin()
        {
            // Arrange
            using var data = DatabaseMock.Instance;

            data.Categories.Add(new Category { Id = 1, Name = "test" });
            data.Conditions.Add(new Condition { Id = 1, Name = "test" });

            data.SaveChanges();

            var cardFormModel = new AddCardFormModel
            {
                ConditionId = 1,
                CategoryId = 1
            };

            var imageFile = new FormFile(new MemoryStream(), 0, 0, "file", "file");

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(ClaimTypes.Name, "username")
            };

            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            var dealerServiceMock = new Mock<IDealerService>();
            dealerServiceMock
                .Setup(service => service.GetDealerId(It.IsAny<string>()))
                .Returns(1);

            var cardsServiceMock = new Mock<ICardService>();
            cardsServiceMock
                .Setup(service => service.CardIsByDealer(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(false);

            var controller = new CardsController(cardsServiceMock.Object, dealerServiceMock.Object, null, data);

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = claimsPrincipal
                }
            };

            // Act
            var result = await controller.EditAsync(1, cardFormModel, imageFile);

            //Assert
            Assert.IsType<BadRequestResult>(result);
            Assert.True(controller.ModelState.IsValid);
        }

        [Fact]
        public async void EditAsyncShouldReturnViewWithAnInvalidModelStateAndAMessageRegardingTheSizeOfTheImage()
        {
            // Arrange
            using var data = DatabaseMock.Instance;

            data.Categories.Add(new Category { Id = 1, Name = "test" });
            data.Conditions.Add(new Condition { Id = 1, Name = "test" });

            data.SaveChanges();

            var cardFormModel = new AddCardFormModel
            {
                ConditionId = 1,
                CategoryId = 1
            };

            var path = "D:\\AlexDoNotTouch\\Coding\\repos\\OnlineCardShop\\OnlineCardShop\\wwwroot\\ProfileImages\\admin.png";

            // Open the image file as a stream
            FileStream stream = new FileStream(path, FileMode.Open);

            // Create a new IFormFile object to hold the image
            IFormFile imageFile = new FormFile(stream, 0, stream.Length, "image", Path.GetFileName(path));

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(ClaimTypes.Name, "username")
            };

            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            var dealerServiceMock = new Mock<IDealerService>();
            dealerServiceMock
                .Setup(service => service.GetDealerId(It.IsAny<string>()))
                .Returns(1);

            var cardsServiceMock = new Mock<ICardService>();
            cardsServiceMock
                .Setup(service => service.CreateImage(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new Data.Models.Image { Name = "admin", Path = path, OriginalName = "admin.png" });

            cardsServiceMock
                .Setup(service => service.CardIsByDealer(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(true);

            var webHostEnvironmentMock = new Mock<IWebHostEnvironment>();
            webHostEnvironmentMock
                .Setup(env => env.WebRootPath)
                .Returns("/test/www/test");

            var controller = new CardsController(cardsServiceMock.Object, dealerServiceMock.Object, webHostEnvironmentMock.Object, data);

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = claimsPrincipal
                }
            };

            // Act
            var result = await controller.EditAsync(1, cardFormModel, imageFile);
            var errorMessage = controller.ModelState.Values.FirstOrDefault().Errors.FirstOrDefault().ErrorMessage;

            //Assert
            Assert.False(controller.ModelState.IsValid);
            Assert.True(errorMessage == "Image should be at least 1024x1024");
            Assert.IsType<ViewResult>(result);
        }

    }
}
