
namespace Ecommerce.DataAccess.Configuration
{
    internal class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("Products");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .HasColumnType("nvarchar(100)")
                .IsRequired(true);

            builder.Property(x => x.ISBN)
                .HasColumnType("nvarchar(20)")
                .IsRequired(true);

            builder.Property(x => x.ImageUrl)
                .HasColumnType("nvarchar(200)")
                .IsRequired(false);

            builder.Property(x => x.ListPrice)
                .HasPrecision(12, 2)
                 .IsRequired(true);

            builder.Property(x => x.Price50)
                .HasPrecision(12, 2)
                 .IsRequired(true);

            builder.Property(x => x.Price100)
                .HasPrecision(12, 2)
                 .IsRequired(true);

            builder.Property(x => x.Description)
                .HasColumnType("nvarchar(1000)")
                .IsRequired(false);

            builder.HasOne(x => x.Category)
                .WithMany(x => x.Products)
                .HasForeignKey(x => x.CategoryId)
                .IsRequired(true);

            builder.HasData(
                new List<Product>
                {
                    new Product { Id = 1, Name = "iPhone 14 Pro", ISBN = "978-1-00-000001-0", Author = "Apple", Description = "Apple smartphone with A16 Bionic chip and Dynamic Island.", ListPrice = 1200, Price50 = 1150, Price100 = 1100, ImageUrl = "", CategoryId = 1 },
                    new Product { Id = 2, Name = "Samsung Galaxy S23", ISBN = "978-1-00-000001-1", Author = "Samsung", Description = "Flagship Android phone with excellent camera and display.", ListPrice = 1100, Price50 = 1050, Price100 = 1000, ImageUrl = "", CategoryId = 1 },
                    new Product { Id = 3, Name = "Xiaomi 13 Pro", ISBN = "978-1-00-000001-2", Author = "Xiaomi", Description = "High-end phone with Leica camera and powerful Snapdragon processor.", ListPrice = 900, Price50 = 850, Price100 = 800, ImageUrl = "", CategoryId = 1 },
                    new Product { Id = 4, Name = "Google Pixel 7", ISBN = "978-1-00-000001-3", Author = "Google", Description = "Pixel phone with clean Android and great photography features.", ListPrice = 950, Price50 = 900, Price100 = 850, ImageUrl = "", CategoryId = 1 },
                    new Product { Id = 5, Name = "OnePlus 11", ISBN = "978-1-00-000001-4", Author = "OnePlus", Description = "Flagship killer with fast charging and smooth performance.", ListPrice = 800, Price50 = 750, Price100 = 700, ImageUrl = "", CategoryId = 1 },

                    new Product { Id = 6, Name = "MacBook Air M2", ISBN = "978-1-00-000002-0", Author = "Apple", Description = "Ultra-thin laptop with M2 chip and long battery life.", ListPrice = 1300, Price50 = 1250, Price100 = 1200, ImageUrl = "", CategoryId = 2 },
                    new Product { Id = 7, Name = "Dell XPS 13", ISBN = "978-1-00-000002-1", Author = "Dell", Description = "Premium Windows Ultrabook with compact design.", ListPrice = 1250, Price50 = 1200, Price100 = 1150, ImageUrl = "", CategoryId = 2 },
                    new Product { Id = 8, Name = "HP Specter x360", ISBN = "978-1-00-000002-2", Author = "HP", Description = "2-in-1 touchscreen laptop with elegant build and strong specs.", ListPrice = 1200, Price50 = 1150, Price100 = 1100, ImageUrl = "", CategoryId = 2 },
                    new Product { Id = 9, Name = "Lenovo ThinkPad X1 Carbon", ISBN = "978-1-00-000002-3", Author = "Lenovo", Description = "Business-class laptop with powerful performance and durability.", ListPrice = 1400, Price50 = 1350, Price100 = 1300, ImageUrl = "", CategoryId = 2 },
                    new Product { Id = 10, Name = "ASUS ROG Zephyrus G14", ISBN = "978-1-00-000002-4", Author = "ASUS", Description = "Gaming laptop with Ryzen processor and high refresh rate screen.", ListPrice = 1500, Price50 = 1450, Price100 = 1400, ImageUrl = "", CategoryId = 2 },

                    new Product { Id = 11, Name = "iPad Pro 12.9-inch (M2)", ISBN = "978-1-00-000003-0", Author = "Apple", Description = "High-end iPad with M2 chip and ProMotion display.", ListPrice = 1500, Price50 = 1450, Price100 = 1400, ImageUrl = "", CategoryId = 3 },
                    new Product { Id = 12, Name = "iPad Air 5th Gen", ISBN = "978-1-00-000003-1", Author = "Apple", Description = "Lightweight iPad with M1 chip and 10.9-inch screen.", ListPrice = 900, Price50 = 850, Price100 = 800, ImageUrl = "", CategoryId = 3 },
                    new Product { Id = 13, Name = "iPad 10th Gen", ISBN = "978-1-00-000003-2", Author = "Apple", Description = "Affordable iPad with modern design and USB-C port.", ListPrice = 600, Price50 = 550, Price100 = 500, ImageUrl = "", CategoryId = 3 },
                    new Product { Id = 14, Name = "iPad Mini 6", ISBN = "978-1-00-000003-3", Author = "Apple", Description = "Compact iPad with A15 Bionic chip and 8.3-inch screen.", ListPrice = 700, Price50 = 650, Price100 = 600, ImageUrl = "", CategoryId = 3 },
                    new Product { Id = 15, Name = "iPad Pro 11-inch", ISBN = "978-1-00-000003-4", Author = "Apple", Description = "Powerful tablet with M2 chip and Apple Pencil 2 support.", ListPrice = 1300, Price50 = 1250, Price100 = 1200, ImageUrl = "", CategoryId = 3 },

                    new Product { Id = 16, Name = "Apple Watch Series 9", ISBN = "978-1-00-000004-0", Author = "Apple", Description = "Latest Apple Watch with S9 chip and advanced health tracking.", ListPrice = 500, Price50 = 470, Price100 = 450, ImageUrl = "", CategoryId = 4 },
                    new Product { Id = 17, Name = "Samsung Galaxy Watch 6", ISBN = "978-1-00-000004-1", Author = "Samsung", Description = "Smart watch with AMOLED display and Wear OS.", ListPrice = 450, Price50 = 420, Price100 = 400, ImageUrl = "", CategoryId = 4 },
                    new Product { Id = 18, Name = "Fitbit Sense 2", ISBN = "978-1-00-000004-2", Author = "Fitbit", Description = "Health-focused smartwatch with stress tracking and ECG.", ListPrice = 400, Price50 = 370, Price100 = 350, ImageUrl = "", CategoryId = 4 },
                    new Product { Id = 19, Name = "Garmin Venue 2 Plus", ISBN = "978-1-00-000004-3", Author = "Garmin", Description = "Fitness watch with GPS and voice assistant support.", ListPrice = 550, Price50 = 520, Price100 = 500, ImageUrl = "", CategoryId = 4 },
                    new Product { Id = 20, Name = "Huawei Watch GT 3", ISBN = "978-1-00-000004-4", Author = "Huawei", Description = "Stylish smartwatch with long battery life and health monitoring.", ListPrice = 430, Price50 = 400, Price100 = 380, ImageUrl = "", CategoryId = 4 }
                }
            );



        }
    }
}
