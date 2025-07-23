
namespace Ecommerce.DataAccess.Configuration
{
    internal class OrderDetailConfiguration : IEntityTypeConfiguration<OrderDetail>
    {
        public void Configure(EntityTypeBuilder<OrderDetail> builder)
        {
            builder.ToTable("OrderDetails");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Price)
                .HasPrecision(12, 2)
                   .IsRequired();

            builder.Property(x => x.Count)
                   .IsRequired();

            builder.HasOne(x => x.OrderHeader)
                   .WithMany(x => x.OrderDetails)
                   .HasForeignKey(x => x.OrderHeaderId);

            builder.HasOne(x => x.Product)
                   .WithMany()
                   .HasForeignKey(x => x.ProductId);

            builder.HasOne(x => x.OrderHeader)
                .WithMany(x => x.OrderDetails)
                .HasForeignKey(x => x.OrderHeaderId);
        }
    }
}
