namespace OnlineCardShop.Tests.Pipeline
{
    using MyTested.AspNetCore.Mvc;
    using OnlineCardShop.Controllers;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Xunit;

    public class UsersControllerTests
    {
        [Fact]
        public void GetDetailsShouldBeForAuthorizedUsersAndReturnView()
        {
            MyMvc
                .Pipeline()
                .ShouldMap(request => request
                    .WithPath("/Users/Details/testId")
                    .WithUser())
                .To<UsersController>(c => c.Details("testId"))
                .Which()
                .ShouldHave()
                .ActionAttributes(attributes => attributes
                    .RestrictingForAuthorizedRequests())
                .AndAlso()
                .ShouldReturn()
                .View();
        }
    }
}
