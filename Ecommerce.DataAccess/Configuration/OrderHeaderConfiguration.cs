
namespace Ecommerce.DataAccess.Configuration
{
    internal class OrderHeaderConfiguration : IEntityTypeConfiguration<OrderHeader>
    {
        public void Configure(EntityTypeBuilder<OrderHeader> builder)
        {
            builder.ToTable("OrderHeaders");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.ApplicationUserId);

            builder.Property(x => x.OrderDate);

            builder.Property(x => x.ShippingDate);

            builder.Property(x => x.OrderStatus)
                .HasColumnType("varchar(50)")
                .IsRequired();

            builder.Property(x => x.PaymentStatus)
                .HasColumnType("varchar(50)")
                .IsRequired();

            builder.Property(x => x.TrackingNumber)
                   .HasMaxLength(100);

            builder.Property(x => x.Carrier)
                   .HasMaxLength(100)
                    .IsRequired(false);

            builder.Property(x => x.Name)
                   .HasMaxLength(100)
                   .IsRequired();

            builder.Property(x => x.PhoneNumber)
                   .HasMaxLength(11)
                   .IsRequired();

            builder.Property(x => x.StreetAddress)
                   .HasMaxLength(200)
                   .IsRequired();

            builder.Property(x => x.City)
                   .HasMaxLength(100)
                   .IsRequired();

            builder.Property(x => x.TotalPrice)
                .HasPrecision(18,2)
                   .IsRequired();


        }
    }


}
