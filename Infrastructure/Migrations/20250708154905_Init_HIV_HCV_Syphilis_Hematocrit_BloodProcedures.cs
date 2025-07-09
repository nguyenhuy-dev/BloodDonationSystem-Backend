using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Init_HIV_HCV_Syphilis_Hematocrit_BloodProcedures : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HCV",
                table: "BloodProcedures",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "HIV",
                table: "BloodProcedures",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "Hematocrit",
                table: "BloodProcedures",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Syphilis",
                table: "BloodProcedures",
                type: "bit",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HCV",
                table: "BloodProcedures");

            migrationBuilder.DropColumn(
                name: "HIV",
                table: "BloodProcedures");

            migrationBuilder.DropColumn(
                name: "Hematocrit",
                table: "BloodProcedures");

            migrationBuilder.DropColumn(
                name: "Syphilis",
                table: "BloodProcedures");
        }
    }
}
