﻿namespace OnlineCardShop.Models.Cards
{
    public class CardListingViewModel
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public double Price { get; set; }

        public string Description { get; set; }

        public string ImageUrl { get; set; }

        public string Category { get; init; }

        public string Condition { get; init; }
    }
}
