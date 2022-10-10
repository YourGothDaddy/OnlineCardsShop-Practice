using OnlineCardShop.Infrastructure;

namespace OnlineCardShop.Services.Dealers
{
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using OnlineCardShop.Data;
    using OnlineCardShop.Data.Models;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;

    public class DealerService : IDealerService
    {
        private readonly OnlineCardShopDbContext data;
        private readonly UserManager<User> userManager;

        public DealerService(OnlineCardShopDbContext data,
            UserManager<User> userManager)
        {
            this.data = data;
            this.userManager = userManager;
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

        public void AddReview(string description, int rating, string userId, ClaimsPrincipal user)
        {
            var reviewSubmitterId = user.GetId();
            var reviewSubmitter = this.data
                .Users
                .Where(u => u.Id == reviewSubmitterId)
                .FirstOrDefault();

            this.data
                .Reviews
                .Add(new Review
                {
                    Description = description,
                    Rating = rating,
                    UserId = userId,
                    SubmitterId = reviewSubmitterId
                });

            this.data.SaveChanges();
        }

        public void UpdateRatings(string userId)
        {
            var totalReviews = this.data
                .Reviews
                .Include(x => x.User)
                .Include(x => x.User.ProfileImage)
                .Where(r => r.UserId == userId)
                .Select(r => r.Rating)
                .ToList();

            var totalReviewsCount = this.data
                .Reviews
                .Where(r => r.UserId == userId)
                .Count();

            var reviewsToSkip = 3;

            if (totalReviewsCount < reviewsToSkip)
            {
                reviewsToSkip = 0;
                totalReviewsCount = 0;
            }

            var mostRecentReviews = this.data
                .Reviews
                .Include(x => x.User)
                .Include(x => x.User.ProfileImage)
                .Skip(totalReviewsCount - reviewsToSkip)
                .Where(r => r.UserId == userId)
                .Select(r => r.Rating)
                .ToList();

            //var countOfRatings = allUserRatings.Count();

            var currentUser = this.data
                .Users
                .Where(u => u.Id == userId)
                .FirstOrDefault();

            currentUser.TotalRating = totalReviews.Sum();
            currentUser.TotalRaters = totalReviewsCount;

            this.data.SaveChanges();
        }

        public IEnumerable<Review> GetReviews(string userId)
        {

            var totalReviewsCount = this.data
                .Reviews
                .Where(r => r.UserId == userId)
                .Count();

            var reviewsToSkip = 3;

            if(totalReviewsCount <= reviewsToSkip)
            {
                reviewsToSkip = 0;
                totalReviewsCount = 0;
            }

            var mostRecentReviews = this.data
                .Reviews
                .Include(x => x.User)
                .Include(x => x.User.ProfileImage)
                .Skip(totalReviewsCount - reviewsToSkip)
                .Where(r => r.UserId == userId)
                .OrderBy(r => r.SubmitterId)
                .ToList();

            return mostRecentReviews;
        }

        public IEnumerable<User> GetSubmitters(IEnumerable<Review> reviews)
        {
            var mostRecentReviewsSubmitters = reviews
                .Select(r => r.SubmitterId)
                .ToList();

            var allUsers = this.data
                .Users
                .Include(x => x.ProfileImage)
                .ToList();

            var usersResult = new List<User>();

            foreach (var user in allUsers)
            {
                if (mostRecentReviewsSubmitters.Any(mrrs => mrrs == user.Id))
                {
                    usersResult.Add(user);
                }
            }

            return usersResult.OrderBy(ur => ur.Id);
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
