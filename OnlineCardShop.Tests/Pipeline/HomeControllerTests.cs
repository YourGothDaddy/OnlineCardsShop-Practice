namespace OnlineCardShop.Tests.Pipeline
{
    using Xunit;
    using MyTested.AspNetCore.Mvc;
    using OnlineCardShop.Controllers;
    using OnlineCardShop.Services.Cards;
    using OnlineCardShop.Models.Home;
    using Shouldly;
    using System.Linq;
    using System.Collections.Generic;
    using OnlineCardShop.Data.Models;

    public class HomeControllerTests
    {
        [Fact]
        public void ErrorShouldReturnView()
        {
            MyMvc
                .Pipeline()
                .ShouldMap("/Home/Error")
                .To<HomeController>(c => c.Error())
                .Which()
                .ShouldReturn()
                .View();
        }
    }
}
