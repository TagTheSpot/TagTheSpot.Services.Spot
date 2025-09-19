using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NetTopologySuite.Geometries;

namespace TagTheSpot.Services.Spot.Infrastructure.Persistence.Configurations
{
    internal sealed class SpotConfiguration
        : IEntityTypeConfiguration<Domain.Spots.Spot>
    {
        public void Configure(EntityTypeBuilder<Domain.Spots.Spot> builder)
        {
            builder.HasKey(s => s.Id);

            builder.HasIndex(s => s.CityId);

            builder.Property(s => s.Description)
                   .IsRequired()
                   .HasMaxLength(1000);

            builder.Property(s => s.Type)
                   .HasConversion<string>()
                   .IsRequired();

            builder.Property(s => s.SkillLevel)
                   .HasConversion<string>();

            builder.Property(s => s.Accessibility)
                   .HasConversion<string>();

            builder.Property(s => s.Condition)
                   .HasConversion<string>();

            builder.Property(s => s.CreatedAt)
                   .HasDefaultValueSql("NOW()");

            builder.Property<Point>("Location")
                   .HasColumnType("geography(Point,4326)")
                   .HasComputedColumnSql("ST_SetSRID(ST_MakePoint(\"Longitude\", \"Latitude\"), 4326)", stored: true);

            builder.HasIndex("Location").HasMethod("GIST");
        }
    }
}
