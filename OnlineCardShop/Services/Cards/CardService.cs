namespace OnlineCardShop.Services.Cards
{
    using OnlineCardShop.Data;
    using OnlineCardShop.Data.Models;
    using OnlineCardShop.Data.Models.Enums;
    using System.Collections.Generic;
    using System.Linq;

    public class CardService : ICardService
    {

        private readonly OnlineCardShopDbContext data;

        public CardService(OnlineCardShopDbContext data)
        {
            this.data = data;
        }

        public Image CreateImage(
            string imageName, 
            string imagePathForDb)
        {
            return new Image
            {
                Name = imageName,
                Path = imagePathForDb
            };
        }

        public Card CreateCard(
            string title,
            double price,
            string description,
            int categoryId,
            int conditionId,
            int dealerId,
            Image newImage)
        {
            return new Card
            {
                Title = title,
                Price = price,
                Description = description,
                CategoryId = categoryId,
                ConditionId = conditionId,
                DealerId = dealerId,
                Image = newImage
            };
        }

        public CardQueryServiceModel All(
            string searchTerm, 
            CardSorting sorting, 
            int currentPage,
            int cardsPerPage,
            int? categoryId,
            SortingOrder? order)
        {
            var cardsQuery = this.data.Cards.AsQueryable();
            if (categoryId != null)
            {
                if (categoryId == 1)//Kpop
                {
                    cardsQuery = this.data.Cards
                        .Where(c => c.CategoryId == 1)
                        .AsQueryable();
                }
                else if(categoryId == 2)//Game
                {
                    cardsQuery = this.data.Cards
                        .Where(c => c.CategoryId == 2)
                        .AsQueryable();
                }
            }
            else
            {
                cardsQuery = this.data.Cards.AsQueryable();
            }

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                cardsQuery = cardsQuery
                    .Where(c =>
                    c.Title.ToLower().Contains(searchTerm.ToLower()));
            }

            if (categoryId != null)
            {
                cardsQuery = sorting switch
                {
                    CardSorting.Condition => cardsQuery.OrderBy(c => c.Condition),
                    _ => cardsQuery.OrderByDescending(c => c.Condition)
                };

                if (order != null)
                {
                    cardsQuery = order switch
                    {
                        SortingOrder.BestToWorse => cardsQuery.OrderBy(c => c.Condition),
                        SortingOrder.WorseToBest => cardsQuery.OrderByDescending(c => c.Condition),
                        _ => cardsQuery.OrderBy(c => c.Condition)
                    };
                }
            }
            else
            {
                cardsQuery = sorting switch
                {
                    CardSorting.Condition => cardsQuery.OrderBy(c => c.Condition),
                    CardSorting.Category => cardsQuery.OrderByDescending(c => c.Category),
                    _ => cardsQuery.OrderByDescending(c => c.Condition)
                };
            }

            var totalCards = cardsQuery.Count();

            var cards = cardsQuery
                .Skip((currentPage - 1) * cardsPerPage)
                .Take(cardsPerPage)
                .Select(c => new CardServiceModel
                {
                    Id = c.Id,
                    Title = c.Title,
                    Description = c.Description,
                    Category = c.Category.Name,
                    Condition = c.Condition.Name,
                    Price = c.Price,
                    Path = c.Image.Path

                })
                .ToList();

            return new CardQueryServiceModel
            {
                TotalCards = cardsQuery.Count(),
                Cards = cards
            };
        }

        public AllCardsServiceModel ByUser(
            string userId,
            int currentPage,
            int cardsPerPage)
        {
            var dealerId = this.data
                .Dealers
                .Where(d => d.UserId == userId)
                .Select(d => d.Id)
                .FirstOrDefault();

            var totalCards = this.data.Cards.Where(c => c.DealerId == dealerId).Count();

            var cards = this.data
                .Cards
                .Skip((currentPage - 1) * cardsPerPage)
                .Take(cardsPerPage)
                .Where(c => c.DealerId == dealerId)
                .Select(c => new CardServiceModel
                {
                    Id = c.Id,
                    Title = c.Title,
                    Price = c.Price,
                    Description = c.Description,
                    Category = c.Category.Name,
                    Condition = c.Condition.Name,
                    Path = c.Image.Path
                })
                .ToList();

            var cardsResult = new AllCardsServiceModel();
            cardsResult.Cards = cards;
            cardsResult.TotalCards = totalCards;
            cardsResult.CurrentPage = currentPage;

            return cardsResult;
        }

        public CardServiceModel CardByUser(int id)
        {
            var card = this.data
                .Cards
                .Where(c => c.Id == id)
                .Select(c => new CardServiceModel
                {
                    Id = c.Id,
                    Title = c.Title,
                    Price = c.Price,
                    Description = c.Description,
                    Category = c.Category.Name,
                    Condition = c.Condition.Name,
                    Path = c.Image.Path.Replace("res", string.Empty),
                    DealerId = c.DealerId,
                    DealerName = c.Dealer.Name,
                    UserId = c.Dealer.UserId
                })
                .FirstOrDefault();

            return card;
        }

        public IEnumerable<CardCategoryServiceViewModel> GetCardCategories()
        {
            return this.data
                .Categories
                .Select(c => new CardCategoryServiceViewModel
                {
                    Id = c.Id,
                    Name = c.Name
                })
                .ToList();
        }

        public IEnumerable<CardConditionServiceViewModel> GetCardConditions()
        {
            return this.data
                 .Conditions
                 .Select(c => new CardConditionServiceViewModel
                 {
                     Id = c.Id,
                     Name = c.Name
                 })
                 .ToList();
        }
    }
}
