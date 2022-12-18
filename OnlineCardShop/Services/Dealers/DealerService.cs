using OnlineCardShop.Infrastructure;

namespace OnlineCardShop.Services.Dealers
{
    using Microsoft.EntityFrameworkCore;
    using OnlineCardShop.Data;
    using OnlineCardShop.Data.Models;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;

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
                    Id = d.UserId,
                    Name = d.Name
                })
                .FirstOrDefault();
        }

        public int GetDealerReviewsCount(int dealerId)
        {
            return this.data
                .Dealers
                .Where(d => d.Id == dealerId)
                .Select(d => d.User.Reviews.Count())
                .FirstOrDefault();
        }

        public int GetDealerTotalRating(int dealerId)
        {
            return this.data
                .Dealers
                .Include(x => x.User)
                .Where(d => d.Id == dealerId)
                .Select(d => d.User.TotalRating)
                .FirstOrDefault();
        }

        public int GetDealerTotalRaters(int dealerId)
        {
            return this.data
                .Dealers
                .Where(d => d.Id == dealerId)
                .Select(d => d.User.TotalRaters)
                .FirstOrDefault();
        }

        public void CreateDealer(Dealer dealer)
        {
            this.data.Dealers.Add(dealer);
            this.data.SaveChanges();
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

            var totalReviewers = totalReviewsCount;

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

            var currentUser = this.data
                .Users
                .Where(u => u.Id == userId)
                .FirstOrDefault();

            currentUser.TotalRating = totalReviews.Sum();
            currentUser.TotalRaters = totalReviewers;

            this.data.SaveChanges();
        }

        public int GetTotalRating(string userId)
        {
            return this.data
                .Users
                .Where(u => u.Id == userId)
                .Select(u => u.TotalRating)
                .FirstOrDefault();
        }

        public int GetTotalRaters(string userId)
        {
            return this.data
                .Users
                .Where(u => u.Id == userId)
                .Select(u => u.TotalRaters)
                .FirstOrDefault();
        }

        public IEnumerable<Review> GetReviews(string userId)
        {

            var totalReviewsCount = this.data
                .Reviews
                .Where(r => r.UserId == userId)
                .Count();

            var reviewsToSkip = 3;

            CheckHowManyReviewsCanBeSkipped(ref totalReviewsCount, ref reviewsToSkip);

            var mostRecentReviews = this.data
                .Reviews
                .Include(x => x.User)
                .Include(x => x.User.ProfileImage)
                .Where(r => r.UserId == userId)
                .Skip(totalReviewsCount - reviewsToSkip)
                .OrderByDescending(r => r.Id)
                .ToList();

            return mostRecentReviews;
        }

        public IEnumerable<DetailsUserServiceModel> GetSubmitters(IEnumerable<Review> reviews)
        {
            var mostRecentReviewsSubmitters = reviews
                .Select(r => r.SubmitterId)
                .OrderBy(id => id)
                .ToList();

            var allUsers = this.data
                .Users
                .Include(x => x.ProfileImage)
                .Select(u => new DetailsUserServiceModel
                {
                    Id = u.Id,
                    FullName = u.FullName,
                    ProfileImagePath = u.ProfileImage.Path.Replace("res", string.Empty)
                })
                .ToList();

            var usersResult = new List<DetailsUserServiceModel>();

            foreach (var user in allUsers)
            {
                if (mostRecentReviewsSubmitters.Any(mrrs => mrrs == user.Id))
                {
                    var repeatingSubmitters = mostRecentReviewsSubmitters.FindAll(x => x.Equals(user.Id));
                    foreach (var submitter in repeatingSubmitters)
                    {
                        usersResult.Add(user);
                    }
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

        private static void CheckHowManyReviewsCanBeSkipped(ref int totalReviewsCount, ref int reviewsToSkip)
        {
            if (totalReviewsCount <= reviewsToSkip)
            {
                reviewsToSkip = 0;
                totalReviewsCount = 0;
            }
        }
    }
}
