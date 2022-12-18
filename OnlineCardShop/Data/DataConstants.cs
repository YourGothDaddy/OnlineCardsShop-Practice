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
            public const int MinNameLength = 5;
            public const int MaxNameLength = 40;

            public const int MinAboutMeLength = 1;
            public const int MaxAboutMeLength = 200;

            public const int PasswordMaxLength = 100;
            public const int PasswordMinLength = 6;
        }

        public static class Dealer
        {
            public const int MinNameLength = 2;
            public const int MaxNameLength = 20;

            public const int PhoneNumberMinLength = 6;
            public const int PhoneNumberMaxLength = 20;
        }

        public static class Image
        {
            public const int MinNameLength = 3;
            public const int MaxNameLength = 200;

            public const int MinOriginalNameLength = 3;
            public const int MaxOriginalNameLength = 200;

            public const int MinPathLength = 10;
            public const int MaxPathLength = 60;
        }

        public static class Message
        {
            public const int MinContentLength = 1;
            public const int MaxContentLength = 200;
        }

        public static class Report
        {
            public const int MinReasonLength = 1;
            public const int MaxReasonLength = 200;
        }

        public static class Review
        {
            public const int MinDescriptionlength = 1;
            public const int MaxDescriptionLength = 150;

            public const int MinRating = 1;
            public const int MaxRating = 5;
        }
    }
}
