namespace OnlineCardShop.Tests.Controllers
{
    using MyTested.AspNetCore.Mvc;
    using OnlineCardShop.Controllers;
    using OnlineCardShop.Data.Models;
    using OnlineCardShop.Models.Home;
    using OnlineCardShop.Services.Cards;
    using Shouldly;
    using System.Collections.Generic;
    using System.Linq;
    using Xunit;

    public class HomeControllerTests
    {
        [Fact]
        public void IndexShouldReturnViewWithTwoCategories()
        {
            MyController<HomeController>
                .Instance(controller => controller
                    .WithData(data => data
                    .WithEntities(new Category
                    { Id = 1, Name = "Test" }
                    )))
                .Calling(c => c.Index())
                .ShouldReturn()
                .View(view => view
                    .WithModelOfType<AllCategoriesServiceViewModel>()
                    .Passing(m => m.Categories.Count().ShouldBe(1)));
        }

        private IQueryable<CardCategoryServiceViewModel> GetTwoCategories()
        {
            var categories = new List<CardCategoryServiceViewModel>();
            categories.Add(new CardCategoryServiceViewModel() { Id = 1, Name = "Test" });
            categories.Add(new CardCategoryServiceViewModel() { Id = 2, Name = "Test2" });

            return categories.AsQueryable();
        }
    }
}
