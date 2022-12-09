namespace OnlineCardShop.Tests.Controllers
{
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
    using Moq;
    using OnlineCardShop.Areas.Admin.Controllers;
    using OnlineCardShop.Areas.Admin.Services;
    using OnlineCardShop.Areas.Admin.Services.Cards;
    using System.Collections.Generic;
    using Xunit;

    public class AdminCardsControllerTests
    {
        private readonly CardsController controller;
        private readonly Mock<IAdminCardService> mockAdminCardService;

        public AdminCardsControllerTests()
        {
            mockAdminCardService = new Mock<IAdminCardService>();
            controller = new CardsController(mockAdminCardService.Object);
        }

        [Fact]
        public void IndexReturnsViewResult()
        {
            // Arrange
            var validQuery = new AllCardsServiceModel
            {
                CurrentPage = 1
            };
            var validCurrentPage = 1;

            var expectedCards = new AllCardsServiceModel
            {
                TotalCards = 2,
                Cards = new List<CardServiceModel>
                {
                    new CardServiceModel
                    {
                        Id = 1,
                        Title = "Card 1",
                        DealerId = 1,
                        IsPublic = true,
                        IsDeleted = false
                    },
                    new CardServiceModel
                    {
                        Id = 2,
                        Title = "Card 2",
                        DealerId = 2,
                        IsPublic = true,
                        IsDeleted = false
                    },
                    // More cards...
                }
            };

            mockAdminCardService
                .Setup(a => a.All(validQuery.CurrentPage, AllCardsServiceModel.CardsPerPage))
                .Returns(expectedCards);

            // Act
            var result = controller.Index(validQuery, validCurrentPage);

            // Assert
            Assert.IsType<ViewResult>(result);
            var viewResult = result as ViewResult;
            var actualCards = viewResult.Model as AllCardsServiceModel;
            Assert.Equal(expectedCards, actualCards);
            Assert.Equal(validCurrentPage, controller.ViewBag.CurrentPage);
        }

        [Fact]
        public void DeleteWithValidIdReturnsRedirectToActionResult()
        {
            // Arrange
            var httpContext = new DefaultHttpContext();
            var tempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
            controller.TempData = tempData;
            var validId = 1;

            // Act
            var result = controller.Delete(validId);

            // Assert
            Assert.IsType<RedirectToActionResult>(result);
            var redirectResult = result as RedirectToActionResult;
            Assert.Equal("Index", redirectResult.ActionName);
            Assert.Equal("Cards", redirectResult.ControllerName);
            mockAdminCardService.Verify(a => a.DeleteCard(validId), Times.Once);
            var expectedMessage = "You have successfully deleted the card!";
            Assert.Equal(expectedMessage, controller.TempData[WebConstants.GlobalMessage]);
        }

        [Fact]
        public void ApproveWithValidIdReturnsRedirectToActionResult()
        {
            // Arrange
            var httpContext = new DefaultHttpContext();
            var tempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
            controller.TempData = tempData;
            var validId = 1;

            // Act
            var result = controller.Approve(validId);

            // Assert
            Assert.IsType<RedirectToActionResult>(result);
            var redirectResult = result as RedirectToActionResult;
            Assert.Equal("Index", redirectResult.ActionName);
            Assert.Equal("Cards", redirectResult.ControllerName);
            mockAdminCardService.Verify(a => a.ApproveCard(validId), Times.Once);
            var expectedMessage = "You have approved the card!";
            Assert.Equal(expectedMessage, controller.TempData[WebConstants.GlobalMessage]);
        }

        [Fact]
        public void HideWithValidIdReturnsRedirectToActionResult()
        {
            // Arrange
            var httpContext = new DefaultHttpContext();
            var tempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
            controller.TempData = tempData;
            var validId = 1;

            // Act
            var result = controller.Hide(validId);

            // Assert
            Assert.IsType<RedirectToActionResult>(result);
            var redirectResult = result as RedirectToActionResult;
            Assert.Equal("Index", redirectResult.ActionName);
            Assert.Equal("Cards", redirectResult.ControllerName);
            mockAdminCardService.Verify(a => a.HideCard(validId), Times.Once);
            var expectedMessage = "You have hid the card!";
            Assert.Equal(expectedMessage, controller.TempData[WebConstants.GlobalMessage]);
        }
    }
}
