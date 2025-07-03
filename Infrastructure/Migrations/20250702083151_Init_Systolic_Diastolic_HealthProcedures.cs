using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Init_Systolic_Diastolic_HealthProcedures : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Pressure",
                table: "HealthProcedures",
                newName: "Systolic");

            migrationBuilder.AddColumn<int>(
                name: "Diastolic",
                table: "HealthProcedures",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Diastolic",
                table: "HealthProcedures");

            migrationBuilder.RenameColumn(
                name: "Systolic",
                table: "HealthProcedures",
                newName: "Pressure");
        }
    }
}
