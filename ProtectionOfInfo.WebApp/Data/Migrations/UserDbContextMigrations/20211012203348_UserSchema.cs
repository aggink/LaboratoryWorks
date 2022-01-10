using Microsoft.EntityFrameworkCore.Migrations;

namespace ProtectionOfInfo.WebApp.Data.Migrations.UserDbContextMigrations
{
    public partial class UserSchema : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Roles = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BlockedUser = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PasswordValidation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FirstAccess = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
