using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrafficGuard.Domain.Entities;
using TrafficGuard.Domain.Enums;

namespace TrafficGuard.Infrastructure.Data.Configurations;

public class TrafficViolationConfiguration : IEntityTypeConfiguration<TrafficViolation>
{
    public void Configure(EntityTypeBuilder<TrafficViolation> builder)
    {
        builder.ToTable("TrafficViolations");

        builder.HasKey(v => v.Id);

        builder.OwnsOne(v => v.LicensePlate, plate =>
        {
            plate.Property(p => p.Value)
                 .HasColumnName("LicensePlate")
                 .HasMaxLength(7)
                 .IsRequired();

            plate.HasIndex(p => p.Value);
        });

        builder.Property(v => v.Severity)
               .HasConversion(
                   v => v.ToString(),
                   v => (ViolationSeverity)Enum.Parse(typeof(ViolationSeverity), v));

        builder.Property(v => v.FineAmount)
               .HasPrecision(10, 2); 
    }
}