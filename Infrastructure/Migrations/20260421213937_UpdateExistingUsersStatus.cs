using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateExistingUsersStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Update existing users to have Status = Approved (1) so they can login
            migrationBuilder.Sql(
                "UPDATE [Users] SET [Status] = 1 WHERE [Status] = 0 OR [Status] IS NULL"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Revert existing users back to Pending (0)
            migrationBuilder.Sql(
                "UPDATE [Users] SET [Status] = 0 WHERE [Status] = 1"
            );
        }
    }
}
