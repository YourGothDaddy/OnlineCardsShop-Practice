namespace OnlineCardShop.Tests.Pipeline
{
    using MyTested.AspNetCore.Mvc;
    using OnlineCardShop.Controllers;
    using Xunit;

    public class ChatControllerTests
    {
        [Fact]
        public void GetChatShouldBeForAuthorizedUsersAndReturnView()
        {
            MyMvc
                .Pipeline()
                .ShouldMap(request => request
                    .WithPath("/Chat/Chat/testId")
                    .WithUser())
                .To<ChatController>(c => c.Chat("testId"))
                .Which()
                .ShouldHave()
                .ActionAttributes(attributes => attributes
                    .RestrictingForAuthorizedRequests())
                .AndAlso()
                .ShouldReturn()
                .View();
        }

        [Fact]
        public void GetChatShouldBeForAuthorizedUsersAndReturnRedirectToActionIfWithNullId()
        {
            MyMvc
                .Pipeline()
                .ShouldMap(request => request
                    .WithPath("/Chat/Chat")
                    .WithUser())
                .To<ChatController>(c => c.Chat(null))
                .Which()
                .ShouldHave()
                .ActionAttributes(attributes => attributes
                    .RestrictingForAuthorizedRequests())
                .AndAlso()
                .ShouldReturn()
                .RedirectToAction("NoChats");
        }

        [Fact]
        public void GetNosChatsShouldBeForAuthorizedUsersAndReturnView()
        {
            MyMvc
                .Pipeline()
                .ShouldMap(request => request
                    .WithPath("/Chat/NoChats")
                    .WithUser())
                .To<ChatController>(c => c.NoChats())
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
