﻿namespace OnlineCardShop.Services.Dealers
{
    using OnlineCardShop.Data.Models;
    using System.Collections.Generic;
    using System.Security.Claims;

    public interface IDealerService
    {
        public bool IsDealer(string userId);

        public int GetDealerReviewsCount(int dealerId);

        public int GetDealerTotalRating(int dealerId);

        public int GetDealerTotalRaters(int dealerId);

        public void CreateDealer(Dealer dealer);

        public int GetDealerId(string userId);

        public DealerServiceViewModel GetDealer(int dealerId);

        public void AddReview(string description, int rating, string userId, ClaimsPrincipal user);

        public void UpdateRatings(string userId);

        public int GetTotalRating(string userId);

        public int GetTotalRaters(string userId);

        public IEnumerable<DetailsUserServiceModel> GetSubmitters(IEnumerable<Review> reviews);

        public IEnumerable<Review> GetReviews(string userId);
    }
}
