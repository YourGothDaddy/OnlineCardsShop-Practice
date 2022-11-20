namespace OnlineCardShop.Tests.Controllers
{
    using System.Linq;
    using MyTested.AspNetCore.Mvc;
    using Xunit;
    using OnlineCardShop.Controllers;
    using OnlineCardShop.Models.Dealers;
    using OnlineCardShop.Data.Models;
    using System.Collections.Generic;
    using OnlineCardShop.Services.Dealers;

    public class DealersControllerTest
    {
        [Fact]
        public void GetCreateShouldBeForAuthorizedUsers()
        {
            MyController<DealersController>
                .Instance()
                .Calling(c => c.Create())
                .ShouldHave()
                .ActionAttributes(attributes => attributes
                    .RestrictingForAuthorizedRequests());
        }

        [Fact]
        public void GetCreateShouldReturnView()
        {
            MyController<DealersController>
                .Instance()
                .Calling(c => c.Create())
                .ShouldReturn()
                .View();
        }

        [Fact]
        public void PostCreateShouldBeForAuthorizedUsers()
        {
            MyController<DealersController>
                .Instance()
                .Calling(c => c.Create(new BecomeDealerFormModel
                {

                }))
                .ShouldHave()
                .ActionAttributes(attributes => attributes
                    .RestrictingForHttpMethod(HttpMethod.Post)
                    .RestrictingForAuthorizedRequests());
        }

        [Theory]
        [InlineData("Dealer", "0882713110")]
        public void PostCreateShouldHaveValidModelStateAndReturnRedirectToAction(
            string dealerName,
            string phoneNumber)
        {
            MyController<DealersController>
                .Instance(controller => controller
                    .WithUser())
                .Calling(c => c.Create(new BecomeDealerFormModel
                {
                    Name = dealerName,
                    PhoneNumber = phoneNumber
                }))
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
        [InlineData("Dealer", "0882713110")]
        public void PostCreateShouldReturnBadRequest(
            string dealerName,
            string phoneNumber)
        {
            MyController<DealersController>
                .Instance(controller => controller
                    .WithData(new Dealer
                    {
                        UserId = "TestId",
                        Name = dealerName,
                        PhoneNumber = phoneNumber
                    })
                    .WithUser())
                .Calling(c => c.Create(new BecomeDealerFormModel
                {
                    Name = dealerName,
                    PhoneNumber = phoneNumber
                }))
                .ShouldReturn()
                .BadRequest();
        }

        [Theory]
        [InlineData(null, 1, "TestName", "0000000000", null, null)]
        public void GetDealerShouldBeForAuthorizedUsers(
            List<Card> cards,
            int id,
            string name,
            string phoneNumber,
            User dealer,
            string userId)
        {
            MyController<DealersController>
                .Instance(controller => controller
                    .WithData(new Dealer
                    {
                        Cards = cards,
                        Id  = id,
                        Name = name,
                        PhoneNumber = phoneNumber,
                        User = dealer,
                        UserId = userId
                    }))
                .Calling(c => c.Dealer(userId))
                .ShouldHave()
                .ActionAttributes(attributes => attributes
                    .RestrictingForAuthorizedRequests());
        }

        [Theory]
        [InlineData(null, 1, "TestName", "0000000000", null, null)]
        public void GetDealerShouldReturnView(
            List<Card> cards,
            int id,
            string name,
            string phoneNumber,
            User dealer,
            string userId)
        {
            MyController<DealersController>
                .Instance(controller => controller
                    .WithData(new Dealer
                    {
                        Cards = cards,
                        Id = id,
                        Name = name,
                        PhoneNumber = phoneNumber,
                        User = dealer,
                        UserId = userId
                    }))
                .Calling(c => c.Dealer(userId))
                .ShouldReturn()
                .View();
        }

        [Theory]
        [InlineData(null, 1, "TestName", "0000000000", null, null)]
        public void PostDealerShouldBeForAuthorizedUsers(
            List<Card> cards,
            int id,
            string name,
            string phoneNumber,
            User dealer,
            string userId)
        {
            MyController<DealersController>
                .Instance(controller => controller
                    .WithData(new Dealer
                    {
                        Cards = cards,
                        Id = id,
                        Name = name,
                        PhoneNumber = phoneNumber,
                        User = dealer,
                        UserId = userId
                    }))
                .Calling(c => c.Dealer(new DealerServiceViewModel
                {

                }, null))
                .ShouldHave()
                .ActionAttributes(attributes => attributes
                    .RestrictingForHttpMethod(HttpMethod.Post)
                    .RestrictingForAuthorizedRequests());
        }

        [Theory]
        [InlineData("testId", "testDescription", 1)]
        public void PostDealerShouldHaveValidModelStateAndReturnRedirectToAction(string userId,
            string description,
            int rating)
        {
            MyController<DealersController>
                .Instance(controller => controller
                    .WithData(new User
                    {
                        Id = userId
                    })
                    .WithData(new Dealer
                    {
                    }))
                .Calling(c => c.Dealer(new DealerServiceViewModel
                {
                    Description = description,
                    Rating = rating
                }, userId))
                .ShouldHave()
                .ValidModelState()
                .AndAlso()
                .ShouldReturn()
                .RedirectToAction("Dealer", "Dealers");
        }

        [Theory]
        [InlineData("TestId", null, 1)]
        public void PostDealerShouldHaveInvalidModelStateAndReturnView(string userId,
            string description,
            int rating)
        {
            MyController<DealersController>
                .Instance(controller => controller
                    .WithData(new User
                    {
                        Id = userId
                    })
                    .WithData(new Dealer
                    {
                        UserId = userId
                    }))
                .Calling(c => c.Dealer(new DealerServiceViewModel
                {
                    Description = description,
                    Rating = rating
                }, userId))
                .ShouldHave()
                .InvalidModelState()
                .AndAlso()
                .ShouldReturn()
                .View(view => view
                    .WithModelOfType<DealerServiceViewModel>());
        }
    }
}
