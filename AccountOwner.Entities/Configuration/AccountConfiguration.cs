using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AccountOwner.Models;

namespace AccountOwner.Entities
{
    public class AccountConfiguration : IEntityTypeConfiguration<Account>
    {
        public void Configure(EntityTypeBuilder<Account> builder)
        {
            builder.ToTable("Account");

            builder.Property(s => s.Id)
                .HasColumnName("AccountId");

            builder.Property(s => s.AccountType)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(s => s.DateCreated)
                .IsRequired(true);

        }
    }
}
