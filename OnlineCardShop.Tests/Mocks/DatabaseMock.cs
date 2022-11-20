namespace OnlineCardShop.Tests.Mocks
{
    using Microsoft.EntityFrameworkCore;
    using OnlineCardShop.Data;
    using System;

    public class DatabaseMock
    {
        public static OnlineCardShopDbContext Instance
        {
            get
            {
                var dbContextOptions = new DbContextOptionsBuilder<OnlineCardShopDbContext>()
                    .UseInMemoryDatabase(Guid.NewGuid().ToString())
                    .Options;

                return new OnlineCardShopDbContext(dbContextOptions);
            }
        }
    }
}
