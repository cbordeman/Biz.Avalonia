using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "app_roles",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    app_role_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_app_roles", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "tenant_roles",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    tenant_role_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_tenant_roles", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "tenants",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    is_default = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_tenants", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "app_users",
                columns: table => new
                {
                    id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    login_provider = table.Column<byte>(type: "smallint", nullable: false),
                    app_role_id = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_app_users", x => x.id);
                    table.ForeignKey(
                        name: "fk_app_users_app_roles_app_role_id",
                        column: x => x.app_role_id,
                        principalTable: "app_roles",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "tenant_users",
                columns: table => new
                {
                    tenant_id = table.Column<int>(type: "integer", nullable: false),
                    app_user_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_tenant_users", x => new { x.tenant_id, x.app_user_id });
                    table.ForeignKey(
                        name: "fk_tenant_users_app_users_app_user_id",
                        column: x => x.app_user_id,
                        principalTable: "app_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_tenant_users_tenants_tenant_id",
                        column: x => x.tenant_id,
                        principalTable: "tenants",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tenant_role_tenant_user",
                columns: table => new
                {
                    roles_id = table.Column<int>(type: "integer", nullable: false),
                    tenant_users_tenant_id = table.Column<int>(type: "integer", nullable: false),
                    tenant_users_app_user_id = table.Column<string>(type: "character varying(100)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_tenant_role_tenant_user", x => new { x.roles_id, x.tenant_users_tenant_id, x.tenant_users_app_user_id });
                    table.ForeignKey(
                        name: "fk_tenant_role_tenant_user_tenant_roles_roles_id",
                        column: x => x.roles_id,
                        principalTable: "tenant_roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_tenant_role_tenant_user_tenant_users_tenant_users_tenant_id",
                        columns: x => new { x.tenant_users_tenant_id, x.tenant_users_app_user_id },
                        principalTable: "tenant_users",
                        principalColumns: new[] { "tenant_id", "app_user_id" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tenant_user_claims",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    claim_name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    tenant_user_id = table.Column<int>(type: "integer", maxLength: 100, nullable: false),
                    tenant_user_tenant_id = table.Column<int>(type: "integer", nullable: false),
                    tenant_user_app_user_id = table.Column<string>(type: "character varying(100)", nullable: false),
                    value = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_tenant_user_claims", x => x.id);
                    table.ForeignKey(
                        name: "fk_tenant_user_claims_tenant_users_tenant_user_tenant_id_tenan",
                        columns: x => new { x.tenant_user_tenant_id, x.tenant_user_app_user_id },
                        principalTable: "tenant_users",
                        principalColumns: new[] { "tenant_id", "app_user_id" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_app_users_app_role_id",
                table: "app_users",
                column: "app_role_id");

            migrationBuilder.CreateIndex(
                name: "ix_tenant_role_tenant_user_tenant_users_tenant_id_tenant_users",
                table: "tenant_role_tenant_user",
                columns: new[] { "tenant_users_tenant_id", "tenant_users_app_user_id" });

            migrationBuilder.CreateIndex(
                name: "ix_tenant_user_claims_claim_name",
                table: "tenant_user_claims",
                column: "claim_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_tenant_user_claims_tenant_user_tenant_id_tenant_user_app_us",
                table: "tenant_user_claims",
                columns: new[] { "tenant_user_tenant_id", "tenant_user_app_user_id" });

            migrationBuilder.CreateIndex(
                name: "ix_tenant_users_app_user_id",
                table: "tenant_users",
                column: "app_user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tenant_role_tenant_user");

            migrationBuilder.DropTable(
                name: "tenant_user_claims");

            migrationBuilder.DropTable(
                name: "tenant_roles");

            migrationBuilder.DropTable(
                name: "tenant_users");

            migrationBuilder.DropTable(
                name: "app_users");

            migrationBuilder.DropTable(
                name: "tenants");

            migrationBuilder.DropTable(
                name: "app_roles");
        }
    }
}
