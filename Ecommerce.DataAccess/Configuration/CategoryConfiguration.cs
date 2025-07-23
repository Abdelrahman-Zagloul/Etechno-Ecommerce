using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecommerce.DataAccess.Configuration
{
    internal class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.ToTable("Categories");
            builder.HasKey(c => c.Id);

            builder.Property(x => x.CategoryName)
                .HasColumnType("nvarchar(100)")
                .IsRequired(true);

            builder.HasData(
            new List<Category>
            {
                new Category { Id = 1, CategoryName = "Mobiles" },
                new Category { Id = 2, CategoryName = "Laptops" },
                new Category { Id = 3, CategoryName = "iPads" },
                new Category { Id = 4, CategoryName = "Smart Watches" }
            });

        }
    }
}
