using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CyberPulseUso.Backend.Data.Migrations
{
    /// <inheritdoc />
    public partial class LnpaNormative : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_LaborRemunerationParametersValues_StatuId",
                schema: "Lnpa",
                table: "LaborRemunerationParametersValues");

            migrationBuilder.DropIndex(
                name: "IX_LaborRemunerationParametersOtherValues_StatuId",
                schema: "Lnpa",
                table: "LaborRemunerationParametersOtherValues");

            migrationBuilder.CreateTable(
                name: "Normatives",
                schema: "Lnpa",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "varchar(50)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "varchar(1700)", maxLength: 1700, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Normatives", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NormativeChapters",
                schema: "Lnpa",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NormativeId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "varchar(20)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "varchar(70)", maxLength: 70, nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NormativeChapters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NormativeChapters_Normatives_NormativeId",
                        column: x => x.NormativeId,
                        principalSchema: "Lnpa",
                        principalTable: "Normatives",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "NormativeArticles",
                schema: "Lnpa",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NormativeChapterId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "varchar(20)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false),
                    Content = table.Column<string>(type: "varchar(2700)", maxLength: 2700, nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NormativeArticles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NormativeArticles_NormativeChapters_NormativeChapterId",
                        column: x => x.NormativeChapterId,
                        principalSchema: "Lnpa",
                        principalTable: "NormativeChapters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LaborRemunerationParametersValues_StatuId_LRPId_Incl",
                schema: "Lnpa",
                table: "LaborRemunerationParametersValues",
                columns: new[] { "StatuId", "LRPId" })
                .Annotation("SqlServer:Include", new[] { "GroupId", "Date", "Value" });

            migrationBuilder.CreateIndex(
                name: "IX_LaborRemunerationParametersOtherValues_StatuId_LRPId_Incl",
                schema: "Lnpa",
                table: "LaborRemunerationParametersOtherValues",
                columns: new[] { "StatuId", "LRPId" })
                .Annotation("SqlServer:Include", new[] { "Value" });

            migrationBuilder.CreateIndex(
                name: "IX_Charges_Name",
                schema: "Lnpa",
                table: "Charges",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_FirstName",
                schema: "Admi",
                table: "AspNetUsers",
                column: "FirstName");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_LastName",
                schema: "Admi",
                table: "AspNetUsers",
                column: "LastName");

            migrationBuilder.CreateIndex(
                name: "IX_NormativeArticles_NormativeChapterId_SortOrder",
                schema: "Lnpa",
                table: "NormativeArticles",
                columns: new[] { "NormativeChapterId", "SortOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_NormativeChapters_NormativeId_SortOrder",
                schema: "Lnpa",
                table: "NormativeChapters",
                columns: new[] { "NormativeId", "SortOrder" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NormativeArticles",
                schema: "Lnpa");

            migrationBuilder.DropTable(
                name: "NormativeChapters",
                schema: "Lnpa");

            migrationBuilder.DropTable(
                name: "Normatives",
                schema: "Lnpa");

            migrationBuilder.DropIndex(
                name: "IX_LaborRemunerationParametersValues_StatuId_LRPId_Incl",
                schema: "Lnpa",
                table: "LaborRemunerationParametersValues");

            migrationBuilder.DropIndex(
                name: "IX_LaborRemunerationParametersOtherValues_StatuId_LRPId_Incl",
                schema: "Lnpa",
                table: "LaborRemunerationParametersOtherValues");

            migrationBuilder.DropIndex(
                name: "IX_Charges_Name",
                schema: "Lnpa",
                table: "Charges");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_FirstName",
                schema: "Admi",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_LastName",
                schema: "Admi",
                table: "AspNetUsers");

            migrationBuilder.CreateIndex(
                name: "IX_LaborRemunerationParametersValues_StatuId",
                schema: "Lnpa",
                table: "LaborRemunerationParametersValues",
                column: "StatuId");

            migrationBuilder.CreateIndex(
                name: "IX_LaborRemunerationParametersOtherValues_StatuId",
                schema: "Lnpa",
                table: "LaborRemunerationParametersOtherValues",
                column: "StatuId");
        }
    }
}
