using Ambev.DeveloperEvaluation.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Text.RegularExpressions;

namespace Ambev.DeveloperEvaluation.ORM.Mapping;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(u => u.Id);
        builder.Property(u => u.Id).HasColumnType("uuid").ValueGeneratedNever();

        builder.Property(u => u.Username).IsRequired().HasMaxLength(50);
        builder.Property(u => u.Password).IsRequired().HasMaxLength(100);
        builder.Property(u => u.Email).IsRequired().HasMaxLength(100);
        builder.Property(u => u.Phone).HasMaxLength(20);

        builder.Property(u => u.Status)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(u => u.Role)
            .HasConversion<string>()
            .HasMaxLength(20);
                
        builder.Property(u => u.Firstname).IsRequired().HasMaxLength(100);
        builder.Property(u => u.Lastname).IsRequired().HasMaxLength(100);
        
        builder.Property(u => u.City).IsRequired().HasMaxLength(100);
        builder.Property(u => u.Street).IsRequired().HasMaxLength(200);
        builder.Property(u => u.Number).IsRequired();
        builder.Property(u => u.Zipcode).IsRequired().HasMaxLength(20);
        
        builder.Property(u => u.Latitude).HasMaxLength(50);
        builder.Property(u => u.Longitude).HasMaxLength(50);
    }
}
