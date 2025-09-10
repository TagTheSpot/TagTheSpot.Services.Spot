using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

namespace TagTheSpot.Services.Spot.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SubmissionLocation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Point>(
                name: "Location",
                table: "Submissions",
                type: "geography(Point,4326)",
                nullable: true,
                computedColumnSql: "ST_SetSRID(ST_MakePoint(\"Longitude\", \"Latitude\"), 4326)",
                stored: true);

            migrationBuilder.CreateIndex(
                name: "IX_Submissions_Location",
                table: "Submissions",
                column: "Location")
                .Annotation("Npgsql:IndexMethod", "GIST");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Submissions_Location",
                table: "Submissions");

            migrationBuilder.DropColumn(
                name: "Location",
                table: "Submissions");
        }
    }
}
