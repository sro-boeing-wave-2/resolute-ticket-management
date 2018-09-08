using Microsoft.EntityFrameworkCore.Migrations;

namespace ticket_management.Migrations
{
    public partial class chatconnection : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Connectionid",
                table: "Ticket",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Connectionid",
                table: "Ticket");
        }
    }
}
