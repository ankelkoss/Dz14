using InternetShop.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection.Emit;

namespace InternetShop.Data.EntityConfigurations
{
    public class ConsumerConfiguration : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            // Уникальность Email клиента
            builder.HasIndex(c => c.Email)
                .IsUnique();
        }
    }
}