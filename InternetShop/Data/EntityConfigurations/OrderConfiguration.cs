using InternetShop.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection.Emit;

namespace InternetShop.Data.EntityConfigurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            // Связи Order -> Lines (1 ко многим)
            builder.HasMany(o => o.Lines)
                .WithOne(l => l.Order!)
                .HasForeignKey(l => l.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}