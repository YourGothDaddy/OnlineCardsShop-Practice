namespace OnlineCardShop.Services.Dealers
{
    using OnlineCardShop.Data;
    using OnlineCardShop.Data.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class DealerService : IDealerService
    {
        private readonly OnlineCardShopDbContext data;

        public DealerService(OnlineCardShopDbContext data)
        {
            this.data = data;
        }

        public DealerServiceViewModel GetDealer(int dealerId)
        {
            return this.data
                .Dealers
                .Where(d => d.Id == dealerId)
                .Select(d => new DealerServiceViewModel
                {
                    Name = d.Name,
                    PhoneNumber = d.PhoneNumber
                })
                .FirstOrDefault();
        }

        public void AddReview(string description, int rating, string userId)
        {
            this.data
                .Reviews
                .Add(new Review
                {
                    Description = description,
                    Rating = rating,
                    UserId = userId
                });

            this.data.SaveChanges();
        }

        public void UpdateRatings(string userId)
        {
            var allUserRatings = this.data
                .Reviews
                .Where(r => r.UserId == userId)
                .Select(r => r.Rating)
                .ToList();

            //var countOfRatings = allUserRatings.Count();
            var averageUserRating = allUserRatings.Sum();

            var currentUser = this.data
                .Users
                .Where(u => u.Id == userId)
                .FirstOrDefault();

            currentUser.TotalRating = averageUserRating;
            currentUser.TotalRaters = allUserRatings.Count();

            this.data.SaveChanges();
        }

        public IEnumerable<Review> GetReviews(string userId)
        {
            var allReviews = this.data
                .Reviews
                .Where(r => r.UserId == userId)
                .ToList();

            return allReviews;
        }

        public int GetDealerId(string userId)
        {
            return this.data
                .Dealers
                .Where(d => d.UserId == userId)
                .Select(d => d.Id)
                .FirstOrDefault();
        }

        public bool IsDealer(string userId)
        {
            return this.data
                .Dealers
                .Any(d => d.UserId == userId);
        }
    }
}
