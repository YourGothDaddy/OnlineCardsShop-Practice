namespace OnlineCardShop.Tests.Routing
{
    using MyTested.AspNetCore.Mvc;
    using OnlineCardShop.Controllers;
    using Xunit;

    public class ChatControllerTests
    {
        [Fact]
        public void GetChatShouldBeMapped()
        {
            MyRouting
                .Configuration()
                .ShouldMap("/Chat/Chat/testId")
                .To<ChatController>(c => c.Chat("testId"));
        }

        [Fact]
        public void GetNoChatsShouldBeMapped()
        {
            MyRouting
                .Configuration()
                .ShouldMap("/Chat/NoChats")
                .To<ChatController>(c => c.NoChats());
        }
    }
}
