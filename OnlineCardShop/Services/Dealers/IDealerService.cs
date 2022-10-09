namespace OnlineCardShop.Services.Dealers
{
    using OnlineCardShop.Data.Models;
    using System.Collections.Generic;

    public interface IDealerService
    {
        public bool IsDealer(string userId);

        public int GetDealerId(string userId);

        public DealerServiceViewModel GetDealer(int dealerId);

        public void AddReview(string description, int rating, string userId);

        public void UpdateRatings(string userId);

        public IEnumerable<Review> GetReviews(string userId);
    }
}
