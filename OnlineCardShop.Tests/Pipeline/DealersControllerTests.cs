namespace OnlineCardShop.Tests.Pipeline
{
    using Xunit;
    using MyTested.AspNetCore.Mvc;
    using System.Linq;
    using OnlineCardShop.Controllers;
    using OnlineCardShop.Models.Dealers;
    using System.Collections.Generic;
    using OnlineCardShop.Data.Models;
    using OnlineCardShop.Services.Dealers;

    public class DealersControllerTests
    {
        [Fact]
        public void GetCreateShouldBeForAuthorizedUsersAndReturnView()
        {
            MyMvc
                .Pipeline()
                .ShouldMap(request => request
                    .WithPath("/Dealers/Create")
                    .WithUser())
                .To<DealersController>(c => c.Create())
                .Which()
                .ShouldHave()
                .ActionAttributes(attributes => attributes
                    .RestrictingForAuthorizedRequests())
                .AndAlso()
                .ShouldReturn()
                .View();
        }

        [Theory]
        [InlineData("userTest", "0882713110")]
        public void PostCreateShouldHaveValidModelStateAndReturnRedirectToAction(
            string dealerName,
            string phoneNumber)
        {
            MyPipeline
                .Configuration()
                .ShouldMap(request => request
                    .WithPath("/Dealers/Create")
                    .WithMethod(HttpMethod.Post)
                    .WithFormFields(new
                    {
                        Name = dealerName,
                        PhoneNUmber = phoneNumber
                    })
                    .WithUser())
                .To<DealersController>(c => c.Create(new BecomeDealerFormModel
                {
                    Name = dealerName,
                    PhoneNumber = phoneNumber
                }))
                .Which()
                .ShouldHave()
                .ValidModelState()
                .Data(data => data
                    .WithSet<Dealer>(dealers => dealers
                        .Any(d =>
                        d.Name == dealerName &&
                        d.PhoneNumber == phoneNumber &&
                        d.UserId == TestUser.Identifier)))
                .AndAlso()
                .ShouldReturn()
                .RedirectToAction("Index", "Home");
        }

        [Theory]
        [InlineData(null, 1, "TestName", "0000000000", null, null)]
        public void GetDealerShouldBeForAuthroizedUsersAndReturnView(
            List<Card> cards,
            int id,
            string name,
            string phoneNumber,
            User dealer,
            string userId)
        {
            MyMvc
                .Pipeline()
                .ShouldMap(request => request
                    .WithPath("/Dealers/Dealer/testId")
                    .WithUser())
                .To<DealersController>(c => c.Dealer(null))
                .Which(controller => controller
                    .WithData(new Dealer
                    {
                        Cards = cards,
                        Id = id,
                        Name = name,
                        PhoneNumber = phoneNumber,
                        User = dealer,
                        UserId = userId
                    }))
                .ShouldHave()
                .ActionAttributes(attributes => attributes
                    .RestrictingForAuthorizedRequests())
                .AndAlso()
                .ShouldReturn()
                .View();
        }

        //[Theory]
        //[InlineData("testId", "testDescription", 1)]
        //public void PostDealerShouldHaveValidModelStateAndReturnRedirectToAction(string userId,
        //    string description,
        //    int rating)
        //{
        //    MyPipeline
        //        .Configuration()
        //            .ShouldMap(request => request
        //                .WithFormFields(new DealerServiceViewModel
        //                {
        //                    Id = userId,
        //                    Description = description,
        //                    Name = "testName",
        //                    PhoneNumber = "0882713110",
        //                    TotalRaters = 1,
        //                    TotalRating = 1,
        //                    Reviews = null,
        //                    Submitters = null,
        //                    Rating = 1
        //                })
        //                .WithPath($"/Dealers/Dealer/{userId}")
        //                .WithMethod(HttpMethod.Post)
        //                .WithUser())
        //            .To<DealersController>(c => c.Dealer(new DealerServiceViewModel
        //            {
        //                Id  = userId,
        //                Description = description,
        //                Name = "testName",
        //                PhoneNumber = "0882713110",
        //                TotalRaters = 1,
        //                TotalRating = 1,
        //                Reviews = null,
        //                Submitters = null,
        //                Rating = 1
        //            }, userId))
        //            .Which()
        //            .ShouldHave()
        //            .ValidModelState()
        //            .AndAlso()
        //            .ShouldReturn()
        //            .RedirectToAction("Dealer", "Dealers");
        //}
    }
}
