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
            builder.Property<Point>("Location")
            .HasColumnType("geography(Point,4326)")
            .HasComputedColumnSql("ST_SetSRID(ST_MakePoint(\"Longitude\", \"Latitude\"), 4326)", stored: true);

            builder.HasIndex("Location").HasMethod("GIST");
        }
    }
}
