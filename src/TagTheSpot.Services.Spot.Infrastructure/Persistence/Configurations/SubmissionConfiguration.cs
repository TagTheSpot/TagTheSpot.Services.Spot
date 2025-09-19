using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NetTopologySuite.Geometries;
using TagTheSpot.Services.Spot.Domain.Submissions;
using TagTheSpot.Services.Spot.Domain.Users;

namespace TagTheSpot.Services.Spot.Infrastructure.Persistence.Configurations
{
    public sealed class SubmissionConfiguration
        : IEntityTypeConfiguration<Submission>
    {
        public void Configure(EntityTypeBuilder<Submission> builder)
        {
            builder.HasKey(s => s.Id);

            builder.HasIndex(s => s.UserId);

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

            builder.Property(s => s.Status)
                   .HasConversion<string>()
                   .IsRequired();

            builder.Property(s => s.SubmittedAt)
                   .HasDefaultValueSql("NOW()");

            builder.Property(s => s.RejectionReason)
                   .HasMaxLength(500);

            builder.Property<Point>("Location")
                .HasColumnType("geography(Point,4326)")
                .HasComputedColumnSql("ST_SetSRID(ST_MakePoint(\"Longitude\", \"Latitude\"), 4326)", stored: true);

            builder.HasIndex("Location").HasMethod("GIST");

            builder.HasOne<User>()
               .WithMany()
               .HasForeignKey(s => s.UserId)
               .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
