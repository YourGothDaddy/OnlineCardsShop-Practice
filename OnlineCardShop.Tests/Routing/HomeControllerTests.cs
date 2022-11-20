namespace OnlineCardShop.Tests.Routing
{
    using MyTested.AspNetCore.Mvc;
    using OnlineCardShop.Controllers;
    using Xunit;

    public class HomeControllerTests
    {
        [Fact]
        public void GetIndexShouldBeMapped()
        {
            MyRouting
                .Configuration()
                .ShouldMap("/Home/Index")
                .To<HomeController>(c => c.Index());
        }

        [Fact]
        public void ErrorShouldBeMapped()
        {
            MyRouting
                .Configuration()
                .ShouldMap("/Home/Error")
                .To<HomeController>(c => c.Error());
        }
    }
}
