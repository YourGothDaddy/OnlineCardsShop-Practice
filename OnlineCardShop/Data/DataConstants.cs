namespace OnlineCardShop.Data
{
    public class DataConstants
    {
        public static class Card
        {
            public const int MinTitleLength = 5;
            public const int MaxTitleLength = 50;

            public const int MinDescriptionLength = 50;
            public const int MaxDescriptionLength = 200;
        }

        public static class User
        {
            public const int MinNameLength = 2;
            public const int MaxNameLength = 20;
        }

        public static class Dealer
        {
            public const int MinNameLength = 2;
            public const int MaxNameLength = 20;
            public const int PhoneNumberMinLength = 6;
            public const int PhoneNumberMaxLength = 20;
        }
    }
}
