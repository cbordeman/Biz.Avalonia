using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class AddInitialTenants : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                @"INSERT INTO tenants (name, is_active, is_default) 
                  VALUES ('Demo Tenant', true, true)");
            migrationBuilder.Sql(
                @"INSERT INTO tenants (name, is_active, is_default) 
                  VALUES ('My Organization', true, false)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
