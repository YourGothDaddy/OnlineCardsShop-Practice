namespace OnlineCardShop.Tests.Pipeline
{
    using MyTested.AspNetCore.Mvc;
    using OnlineCardShop.Controllers;
    using OnlineCardShop.Data.Models;
    using OnlineCardShop.Services.Cards;
    using Xunit;

    public class CardsControllerTests
    {
        [Fact]
        public void GetGameShouldReturnView()
        {
            MyMvc
                .Pipeline()
                .ShouldMap("/Cards/Game?currentPage=1")
                .To<CardsController>(c => c.Game(new AllCardsServiceModel { }, 1))
                .Which()
                .ShouldReturn()
                .View();
        }

        [Fact]
        public void GetKpopShouldReturnView()
        {
            MyMvc
                .Pipeline()
                .ShouldMap("/Cards/Kpop?currentPage=1")
                .To<CardsController>(c => c.Kpop(new AllCardsServiceModel { }, 1))
                .Which()
                .ShouldReturn()
                .View();
        }

        [Fact]
        public void GetAllShouldReturnView()
        {
            MyMvc
                .Pipeline()
                .ShouldMap("/Cards/All?currentPage=1")
                .To<CardsController>(c => c.All(new AllCardsServiceModel { }, 1))
                .Which()
                .ShouldReturn()
                .View();
        }

        [Fact]
        public void GetDeleteShouldReturnView()
        {
            MyMvc
                .Pipeline()
                .ShouldMap(request => request
                    .WithPath("/Cards/Delete/1")
                    .WithUser())
                .To<CardsController>(c => c.Delete(1))
                .Which(controller => controller
                    .WithData(new Card
                    {
                        Id = 1,
                        IsDeleted = false,
                        IsPublic = true
                    }))
                .ShouldReturn()
                .RedirectToAction("All", "Cards");
        }
    }
}
