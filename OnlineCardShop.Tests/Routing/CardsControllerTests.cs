namespace OnlineCardShop.Tests.Routing
{
    using Microsoft.AspNetCore.Http;
    using MyTested.AspNetCore.Mvc;
    using OnlineCardShop.Controllers;
    using OnlineCardShop.Models.Cards;
    using OnlineCardShop.Services.Cards;
    using Xunit;

    public class CardsControllerTests
    {
        [Fact]
        public void GetMineShouldBeMapped()
        {
            MyRouting
                .Configuration()
                .ShouldMap("/Cards/Mine?currentPage=1")
                .To<CardsController>(c => c.Mine(1));
        }

        [Fact]
        public void GetDetailsShouldBeMapped()
        {
            MyRouting
                .Configuration()
                .ShouldMap("/Cards/Details")
                .To<CardsController>(c => c.Details(new CardDetailsServiceModel { }));
        }

        [Fact]
        public void GetGameShouldBeMapped()
        {
            MyRouting
                .Configuration()
                .ShouldMap("/Cards/Game?currentPage=1")
                .To<CardsController>(c => c.Game(new AllCardsServiceModel { }, 1));
        }

        [Fact]
        public void GetKpopShouldBeMapped()
        {
            MyRouting
                .Configuration()
                .ShouldMap("/Cards/Kpop?currentPage=1")
                .To<CardsController>(c => c.Kpop(new AllCardsServiceModel { }, 1));
        }

        [Fact]
        public void GetAllShouldBeMapped()
        {
            MyRouting
                .Configuration()
                .ShouldMap("/Cards/All?currentPage=1")
                .To<CardsController>(c => c.All(new AllCardsServiceModel { }, 1));
        }

        [Fact]
        public void GetAddShouldBeMapped()
        {
            MyRouting
                .Configuration()
                .ShouldMap("/Cards/Add")
                .To<CardsController>(c => c.Add());
        }

        [Fact]
        public void GetDeleteShouldBeMapped()
        {
            MyRouting
                .Configuration()
                .ShouldMap("/Cards/Delete/1")
                .To<CardsController>(c => c.Delete(1));
        }

        [Fact]
        public void GetEditShouldBeMapped()
        {
            MyRouting
                .Configuration()
                .ShouldMap("/Cards/Edit/1")
                .To<CardsController>(c => c.Edit(1));
        }
    }
}
