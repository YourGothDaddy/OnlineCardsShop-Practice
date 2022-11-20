namespace OnlineCardShop.Tests.Routing
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
        public void GetDetailsShouldBeMapped()
        {
            MyRouting
                .Configuration()
                .ShouldMap("/Users/Details/testId")
                .To<UsersController>(c => c.Details("testId"));
        }
    }
}
