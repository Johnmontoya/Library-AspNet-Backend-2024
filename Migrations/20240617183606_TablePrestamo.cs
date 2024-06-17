using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Migrations
{
    /// <inheritdoc />
    public partial class TablePrestamo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "e366bb7a-544d-4491-9276-bae31a343265");

            migrationBuilder.CreateTable(
                name: "Prestamos",
                columns: table => new
                {
                    Id = table.Column<string>(type: "Varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AutenticacionId = table.Column<string>(type: "Varchar(255)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    LibroId = table.Column<string>(type: "Varchar(255)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FechaPrestamo = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FechaDevolucion = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Prestamos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Prestamos_AspNetUsers_AutenticacionId",
                        column: x => x.AutenticacionId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Prestamos_Libros_LibroId",
                        column: x => x.LibroId,
                        principalTable: "Libros",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "dd6f18a0-5293-4b88-a014-cb6b8ddb4051", null, "User", "USER" });

            migrationBuilder.CreateIndex(
                name: "IX_Prestamos_AutenticacionId",
                table: "Prestamos",
                column: "AutenticacionId");

            migrationBuilder.CreateIndex(
                name: "IX_Prestamos_LibroId",
                table: "Prestamos",
                column: "LibroId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Prestamos");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "dd6f18a0-5293-4b88-a014-cb6b8ddb4051");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "e366bb7a-544d-4491-9276-bae31a343265", null, "User", "USER" });
        }
    }
}
