namespace OnlineCardShop.Services.Home
{
    using OnlineCardShop.Services.Cards;
    using System.Linq;

    public interface IHomeService
    {
        public IQueryable<CardCategoryServiceViewModel> GetCategories();
    }
}
