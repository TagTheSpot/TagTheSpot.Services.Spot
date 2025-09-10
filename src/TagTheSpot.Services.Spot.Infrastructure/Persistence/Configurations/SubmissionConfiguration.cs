using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NetTopologySuite.Geometries;
using TagTheSpot.Services.Spot.Domain.Submissions;

namespace TagTheSpot.Services.Spot.Infrastructure.Persistence.Configurations
{
    public sealed class SubmissionConfiguration
        : IEntityTypeConfiguration<Submission>
    {
        public void Configure(EntityTypeBuilder<Submission> builder)
        {
            builder.Property<Point>("Location")
                .HasColumnType("geography(Point,4326)")
                .HasComputedColumnSql("ST_SetSRID(ST_MakePoint(\"Longitude\", \"Latitude\"), 4326)", stored: true);

            builder.HasIndex("Location").HasMethod("GIST");
        }
    }
}
