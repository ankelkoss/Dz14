using InternetShop.Models;
using Microsoft.EntityFrameworkCore;

namespace InternetShop.Data
{
    public class DbInitializer
    {
        private readonly ModelBuilder _modelBuilder;

        public DbInitializer(ModelBuilder modelBuilder)
        {
            _modelBuilder = modelBuilder;
        }

        public void Seed()
        {
            var seedDate = new DateTime(2026, 01, 01, 12, 00, 00, DateTimeKind.Utc);

            // Categories
            var catPhones = new Category { Id = 1, Name = "Phones", Description = "Smartphones and accessories" };
            var catLaptops = new Category { Id = 2, Name = "Laptops", Description = "Notebooks and ultrabooks" };
            var catAudio = new Category { Id = 3, Name = "Audio", Description = "Headphones, speakers" };

            _modelBuilder.Entity<Category>().HasData(catPhones, catLaptops, catAudio);

            // Products
            _modelBuilder.Entity<Product>().HasData(
                new Product
                {
                    Id = 1,
                    Name = "iPhone 14 Pro Max",
                    Description = "Test product",
                    Price = 999.99m,
                    StockQuantity = 15,
                    IsActive = true,
                    CategoryId = 1
                },
                new Product
                {
                    Id = 2,
                    Name = "Samsung Galaxy S23",
                    Description = "Test product",
                    Price = 799.50m,
                    StockQuantity = 25,
                    IsActive = true,
                    CategoryId = 1
                },
                new Product
                {
                    Id = 3,
                    Name = "Dell XPS 13",
                    Description = "Test product",
                    Price = 1299.00m,
                    StockQuantity = 7,
                    IsActive = true,
                    CategoryId = 2
                },
                new Product
                {
                    Id = 4,
                    Name = "Sony WH-1000XM5",
                    Description = "Noise cancelling headphones",
                    Price = 349.99m,
                    StockQuantity = 30,
                    IsActive = true,
                    CategoryId = 3
                }
            );

            // Customers
            _modelBuilder.Entity<Customer>().HasData(
                new Customer
                {
                    Id = 1,
                    FirstName = "Oleg",
                    LastName = "Shytov",
                    Email = "oleg@test.local",
                    Phone = "+380000000001",
                    CreatedAtUtc = seedDate
                },
                new Customer
                {
                    Id = 2,
                    FirstName = "Ivan",
                    LastName = "Petrenko",
                    Email = "ivan@test.local",
                    Phone = "+380000000002",
                    CreatedAtUtc = seedDate
                }
            );

            // Orders
            _modelBuilder.Entity<Order>().HasData(
                new Order
                {
                    Id = 1,
                    CustomerId = 1,
                    CreatedAtUtc = seedDate,
                    Status = OrderStatus.Created
                },
                new Order
                {
                    Id = 2,
                    CustomerId = 2,
                    CreatedAtUtc = seedDate.AddHours(2),
                    Status = OrderStatus.Paid
                }
            );

            // OrderLines
            _modelBuilder.Entity<OrderLine>().HasData(
                new OrderLine
                {
                    Id = 1,
                    OrderId = 1,
                    ProductId = 1,
                    Quantity = 1,
                    UnitPrice = 999.99m
                },
                new OrderLine
                {
                    Id = 2,
                    OrderId = 1,
                    ProductId = 4,
                    Quantity = 2,
                    UnitPrice = 349.99m
                },
                new OrderLine
                {
                    Id = 3,
                    OrderId = 2,
                    ProductId = 2,
                    Quantity = 1,
                    UnitPrice = 799.50m
                }
            );
        }
    }
}
