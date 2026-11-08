using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CyberPulseUso.Backend.Data.Migrations
{
    /// <inheritdoc />
    public partial class LnpaLRPOV : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LaborRemunerationParametersOtherValues",
                schema: "Lnpa",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LRPId = table.Column<int>(type: "int", nullable: false),
                    Value = table.Column<double>(type: "float", nullable: false),
                    StatuId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LaborRemunerationParametersOtherValues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LaborRemunerationParametersOtherValues_LaborRemunerationParameters_LRPId",
                        column: x => x.LRPId,
                        principalSchema: "Lnpa",
                        principalTable: "LaborRemunerationParameters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LaborRemunerationParametersOtherValues_Status_StatuId",
                        column: x => x.StatuId,
                        principalSchema: "Gene",
                        principalTable: "Status",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LaborRemunerationParametersOtherValues_LRPId",
                schema: "Lnpa",
                table: "LaborRemunerationParametersOtherValues",
                column: "LRPId");

            migrationBuilder.CreateIndex(
                name: "IX_LaborRemunerationParametersOtherValues_StatuId",
                schema: "Lnpa",
                table: "LaborRemunerationParametersOtherValues",
                column: "StatuId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LaborRemunerationParametersOtherValues",
                schema: "Lnpa");
        }
    }
}
