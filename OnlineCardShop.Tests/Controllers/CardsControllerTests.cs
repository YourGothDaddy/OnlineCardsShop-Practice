namespace OnlineCardShop.Tests.Controllers
{
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Moq;
    using MyTested.AspNetCore.Mvc;
    using OnlineCardShop.Controllers;
    using OnlineCardShop.Data;
    using OnlineCardShop.Data.Models;
    using OnlineCardShop.Models.Cards;
    using OnlineCardShop.Services.Cards;
    using OnlineCardShop.Services.Dealers;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using Xunit;

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
    }
}
