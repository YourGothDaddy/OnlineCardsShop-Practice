namespace OnlineCardShop.Tests.Controllers
{
    using MyTested.AspNetCore.Mvc;
    using OnlineCardShop.Controllers;
    using Shouldly;
    using Xunit;

    public class ChatControllerTests
    {
        [Fact]
        public void GetChatShouldBeForAuthorizedUser()
        {
            MyController<ChatController>
                .Instance()
                .Calling(c => c.Chat("testId"))
                .ShouldHave()
                .ActionAttributes(attributes => attributes
                    .RestrictingForAuthorizedRequests());
        }

        [Fact]
        public void GetNoChatsShouldBeForAuthorizedUser()
        {
            MyController<ChatController>
                .Instance()
                .Calling(c => c.NoChats())
                .ShouldHave()
                .ActionAttributes(attributes => attributes
                    .RestrictingForAuthorizedRequests());
        }

        [Fact]
        public void GetChatShouldReturnView()
        {
            MyController<ChatController>
                .Instance()
                .Calling(c => c.Chat("testId"))
                .ShouldReturn()
                .View();
        }

        [Fact]
        public void GetChatShouldReturnRedirectToActionIfWithNullId()
        {
            MyController<ChatController>
                .Instance()
                .Calling(c => c.Chat(null))
                .ShouldReturn()
                .RedirectToAction("NoChats");
        }

        [Fact]
        public void GetNoChatsShouldReturnView()
        {
            MyController<ChatController>
                .Instance()
                .Calling(c => c.NoChats())
                .ShouldReturn()
                .View();
        }
    }
}
