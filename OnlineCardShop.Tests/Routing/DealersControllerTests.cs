namespace OnlineCardShop.Tests.Routing
{
    using MyTested.AspNetCore.Mvc;
    using OnlineCardShop.Controllers;
    using OnlineCardShop.Models.Dealers;
    using Xunit;

    public class DealersControllerTests
    {
        [Fact]
        public void GetCreateShouldBeMapped()
        {
            MyRouting
                .Configuration()
                .ShouldMap("/Dealers/Create")
                .To<DealersController>(c => c.Create());
        }

        [Fact]
        public void PostCreateShouldBeMapped()
        {
            MyRouting
                .Configuration()
                .ShouldMap(request => request
                    .WithPath("/Dealers/Create")
                    .WithMethod(HttpMethod.Post))
                .To<DealersController>(c => c.Create(With.Any<BecomeDealerFormModel>()));
        }

        [Fact]
        public void GetDealerShouldBeMapped()
        {
            MyRouting
                .Configuration()
                .ShouldMap("/Dealers/Dealer/testId")
                .To<DealersController>(c => c.Dealer("testId"));
        }

        [Fact]
        public void PostDealerShouldBeMapped()
        {
            MyRouting
                .Configuration()
                .ShouldMap(request => request
                    .WithPath("/Dealers/Dealer/testId")
                    .WithMethod(HttpMethod.Post))
                .To<DealersController>(c => c.Dealer("testId"));
        }
    }
}
