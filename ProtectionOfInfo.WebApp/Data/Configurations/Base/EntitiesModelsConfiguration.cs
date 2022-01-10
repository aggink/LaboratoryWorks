namespace ProtectionOfInfo.WebApp.Data.Configurations
{
    public static class EntitiesModelsConfiguration
    {
        public const int AuthorNameMaxLength = 255;
        public const string AuthorName = "ФИО автора";
        public const int AuthorBiographyMaxLength = 4000;
        public const string AuthorBiography = "Биография автора";

        public const int BookNameMaxLength = 255;
        public const string BookName = "Название книги";
        public const string BookPrice = "Цена книги (руб.)";
        public const string BookAuthorId = "Id автора";
        public const string BookPublisherId = "Id издательства";
        public const string BookCategoryId = "Id категории";

        public const int CategoryNameMaxLength = 255;
        public const string CategoryName = "Наименование категории";
        public const int CategorySynopsisMaxLength = 4000;
        public const string CategorySynopsis = "Описание категории";

        public const int PublisherNameMaxLength = 255;
        public const string PublisherName = "Название издательства";
        public const int PublisherDescriptionMaxLength = 4000;
        public const string PublisherDescription = "Описание издательства";

        public const string OrderUserId = "Id пользователя";
        public const string OrderBooksId = "Корзина";
        public const string OrderPrice = "Стоймость товара";
    }
}
