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
            string imagePathForDb,
            string originalImageName)
        {
            return new Image
            {
                Name = imageName,
                Path = imagePathForDb,
                OriginalName = originalImageName
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
                Image = newImage,
                IsPublic = false
            };
        }

        public void DeleteCard(int id)
        {
            var card = this.data
                .Cards
                .Where(c => c.Id == id)
                .FirstOrDefault();

            card.IsDeleted = true;
            card.IsPublic = false;

            this.data.SaveChanges();
        }

        public bool EditCard(
            int id,
            string title,
            double price,
            string description,
            int categoryId,
            int conditionId,
            Image? newImage)
        {
            var cardData = this.data.Cards.Find(id);

            if(cardData == null)
            {
                return false;
            }

            cardData.Title = title;
            cardData.Price = price;
            cardData.Description = description;
            cardData.CategoryId = categoryId;
            cardData.ConditionId = conditionId;

            if (newImage != null)
            {
                cardData.Image = newImage;
            }

            this.data.SaveChanges();

            return true;
        }

        public CardQueryServiceModel All(
            string searchTerm, 
            CardSorting sorting, 
            int currentPage,
            int cardsPerPage,
            int? categoryId,
            SortingOrder? order)
        {
            var cardsQuery = this.data
                .Cards
                .Where(c => c.IsPublic != false && c.IsDeleted != true)
                .AsQueryable();

            if (cardsQuery.Count() != 0)
            {
                if (categoryId != null)
                {
                    if (categoryId == 1)//Kpop
                    {
                        cardsQuery = cardsQuery
                            .Where(c => c.CategoryId == 1)
                            .AsQueryable();
                    }
                    else if (categoryId == 2)//Game
                    {
                        cardsQuery = cardsQuery
                            .Where(c => c.CategoryId == 2)
                            .AsQueryable();
                    }
                }
                else
                {
                    cardsQuery = this.data
                    .Cards
                    .Where(c => c.IsPublic != false)
                    .AsQueryable();
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
            }

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
                    Path = c.Image.Path,
                    IsPublic = c.IsPublic

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
                    Path = c.Image.Path,
                    IsPublic = c.IsPublic
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
                    ImageFile = c.Image,
                    Path = c.Image.Path.Replace("res", string.Empty),
                    DealerId = c.DealerId,
                    DealerName = c.Dealer.Name,
                    UserId = c.Dealer.UserId
                })
                .FirstOrDefault();

            return card;
        }

        public bool CardIsByDealer(int cardId, int dealerId)
        {
            return this.data
                .Cards
                .Any(c => c.Id == cardId && c.DealerId == dealerId);
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
