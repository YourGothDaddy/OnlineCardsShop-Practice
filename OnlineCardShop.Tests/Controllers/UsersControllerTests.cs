﻿namespace OnlineCardShop.Tests.Controllers
{
    using MyTested.AspNetCore.Mvc;
    using OnlineCardShop.Controllers;
    using OnlineCardShop.Services.Users;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Xunit;

    public class UsersControllerTests
    {
        [Fact]
        public void GetDetailsShouldBeForAuthorizedUser()
        {
            MyController<UsersController>
                .Instance()
                .Calling(c => c.Details("testId"))
                .ShouldHave()
                .ActionAttributes(attributes => attributes
                    .RestrictingForAuthorizedRequests());
        }

        [Fact]
        public void GetDetailsShouldReturnView()
        {
            MyController<UsersController>
                .Instance()
                .Calling(c => c.Details("testId"))
                .ShouldReturn()
                .View();
        }


        [Fact]
        public void PostDetailsShouldReturnView()
        {
            MyController<UsersController>
                .Instance()
                .Calling(c => c
                    .Details(new UserDetailsServiceViewModel { ReportReason = "testReason" }, "test"))
                .ShouldReturn()
                .RedirectToAction("Index", "Home");
        }
    }
}
