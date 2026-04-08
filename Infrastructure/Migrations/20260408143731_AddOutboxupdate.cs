using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class AddOutboxupdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_outbox_message)",
                table: "outbox_message)");

            migrationBuilder.RenameTable(
                name: "outbox_message)",
                newName: "outbox_message");

            migrationBuilder.AddPrimaryKey(
                name: "PK_outbox_message",
                table: "outbox_message",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_outbox_message",
                table: "outbox_message");

            migrationBuilder.RenameTable(
                name: "outbox_message",
                newName: "outbox_message)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_outbox_message)",
                table: "outbox_message)",
                column: "Id");
        }
    }
}
