using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecommerce.DataAccess.Configuration
{
    internal class ShoppingCartConfiguration : IEntityTypeConfiguration<ShoppingCart>
    {
        public void Configure(EntityTypeBuilder<ShoppingCart> builder)
        {
            builder.ToTable("ShoppingCart");

            builder.Ignore(x => x.Price);
        }
    }
}
