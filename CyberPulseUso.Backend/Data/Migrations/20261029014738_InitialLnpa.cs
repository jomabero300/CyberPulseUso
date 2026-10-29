using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CyberPulseUso.Backend.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialLnpa : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Lnpa");

            migrationBuilder.AddColumn<int>(
                name: "ChargeId",
                schema: "Admi",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Categories",
                schema: "Lnpa",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: false),
                    StatuId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Categories_Status_StatuId",
                        column: x => x.StatuId,
                        principalSchema: "Gene",
                        principalTable: "Status",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Conventions",
                schema: "Lnpa",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: false),
                    StatuId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Conventions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Conventions_Status_StatuId",
                        column: x => x.StatuId,
                        principalSchema: "Gene",
                        principalTable: "Status",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LaborRemunerationParameters",
                schema: "Lnpa",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "varchar(170)", maxLength: 170, nullable: false),
                    StatuId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LaborRemunerationParameters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LaborRemunerationParameters_Status_StatuId",
                        column: x => x.StatuId,
                        principalSchema: "Gene",
                        principalTable: "Status",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LogAuditorias",
                schema: "Admi",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TableName = table.Column<string>(type: "varchar(50)", maxLength: 100, nullable: false),
                    PrimaryKey = table.Column<string>(type: "varchar(3)", maxLength: 3, nullable: false),
                    Action = table.Column<string>(type: "char(1)", maxLength: 100, nullable: false),
                    UserId = table.Column<string>(type: "varchar(450)", maxLength: 450, nullable: false),
                    OldValues = table.Column<string>(type: "varchar(250)", maxLength: 250, nullable: false),
                    NewValues = table.Column<string>(type: "varchar(250)", maxLength: 250, nullable: true),
                    AffectedColumns = table.Column<string>(type: "varchar(250)", maxLength: 250, nullable: true),
                    StockActual = table.Column<int>(type: "int", nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogAuditorias", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Groups",
                schema: "Lnpa",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: false),
                    StatuId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Groups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Groups_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalSchema: "Lnpa",
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Groups_Status_StatuId",
                        column: x => x.StatuId,
                        principalSchema: "Gene",
                        principalTable: "Status",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ConventionValues",
                schema: "Lnpa",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ConventionId = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Prince = table.Column<double>(type: "float", nullable: false),
                    StatuId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConventionValues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConventionValues_Conventions_ConventionId",
                        column: x => x.ConventionId,
                        principalSchema: "Lnpa",
                        principalTable: "Conventions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ConventionValues_Status_StatuId",
                        column: x => x.StatuId,
                        principalSchema: "Gene",
                        principalTable: "Status",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PayrollConcepts",
                schema: "Lnpa",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: false),
                    StatuId = table.Column<int>(type: "int", nullable: false),
                    LRPId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PayrollConcepts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PayrollConcepts_LaborRemunerationParameters_LRPId",
                        column: x => x.LRPId,
                        principalSchema: "Lnpa",
                        principalTable: "LaborRemunerationParameters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PayrollConcepts_Status_StatuId",
                        column: x => x.StatuId,
                        principalSchema: "Gene",
                        principalTable: "Status",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Charges",
                schema: "Lnpa",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GroupId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: false),
                    StatuId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Charges", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Charges_Groups_GroupId",
                        column: x => x.GroupId,
                        principalSchema: "Lnpa",
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Charges_Status_StatuId",
                        column: x => x.StatuId,
                        principalSchema: "Gene",
                        principalTable: "Status",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LaborRemunerationParametersValues",
                schema: "Lnpa",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LRPId = table.Column<int>(type: "int", nullable: false),
                    GroupId = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Value = table.Column<double>(type: "float", nullable: false),
                    StatuId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LaborRemunerationParametersValues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LaborRemunerationParametersValues_Groups_GroupId",
                        column: x => x.GroupId,
                        principalSchema: "Lnpa",
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LaborRemunerationParametersValues_LaborRemunerationParameters_LRPId",
                        column: x => x.LRPId,
                        principalSchema: "Lnpa",
                        principalTable: "LaborRemunerationParameters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LaborRemunerationParametersValues_Status_StatuId",
                        column: x => x.StatuId,
                        principalSchema: "Gene",
                        principalTable: "Status",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PayrollSettlements",
                schema: "Lnpa",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ChargeId = table.Column<int>(type: "int", nullable: false),
                    PayrollConceptId = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<int>(type: "int", maxLength: 80, nullable: false),
                    UnitValue = table.Column<double>(type: "float", maxLength: 80, nullable: false),
                    AccruedValue = table.Column<double>(type: "float", maxLength: 80, nullable: false),
                    DeductedValue = table.Column<double>(type: "float", maxLength: 80, nullable: false),
                    StatuId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PayrollSettlements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PayrollSettlements_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "Admi",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PayrollSettlements_Charges_ChargeId",
                        column: x => x.ChargeId,
                        principalSchema: "Lnpa",
                        principalTable: "Charges",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PayrollSettlements_PayrollConcepts_PayrollConceptId",
                        column: x => x.PayrollConceptId,
                        principalSchema: "Lnpa",
                        principalTable: "PayrollConcepts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PayrollSettlements_Status_StatuId",
                        column: x => x.StatuId,
                        principalSchema: "Gene",
                        principalTable: "Status",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_ChargeId",
                schema: "Admi",
                table: "AspNetUsers",
                column: "ChargeId");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_Name",
                schema: "Lnpa",
                table: "Categories",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Categories_StatuId",
                schema: "Lnpa",
                table: "Categories",
                column: "StatuId");

            migrationBuilder.CreateIndex(
                name: "IX_Charges_GroupId_Name",
                schema: "Lnpa",
                table: "Charges",
                columns: new[] { "GroupId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Charges_StatuId",
                schema: "Lnpa",
                table: "Charges",
                column: "StatuId");

            migrationBuilder.CreateIndex(
                name: "IX_Conventions_Name",
                schema: "Lnpa",
                table: "Conventions",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Conventions_StatuId",
                schema: "Lnpa",
                table: "Conventions",
                column: "StatuId");

            migrationBuilder.CreateIndex(
                name: "IX_ConventionValues_ConventionId",
                schema: "Lnpa",
                table: "ConventionValues",
                column: "ConventionId");

            migrationBuilder.CreateIndex(
                name: "IX_ConventionValues_StatuId",
                schema: "Lnpa",
                table: "ConventionValues",
                column: "StatuId");

            migrationBuilder.CreateIndex(
                name: "IX_Groups_CategoryId_Name",
                schema: "Lnpa",
                table: "Groups",
                columns: new[] { "CategoryId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Groups_StatuId",
                schema: "Lnpa",
                table: "Groups",
                column: "StatuId");

            migrationBuilder.CreateIndex(
                name: "IX_LaborRemunerationParameters_Name",
                schema: "Lnpa",
                table: "LaborRemunerationParameters",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LaborRemunerationParameters_StatuId",
                schema: "Lnpa",
                table: "LaborRemunerationParameters",
                column: "StatuId");

            migrationBuilder.CreateIndex(
                name: "IX_LaborRemunerationParametersValues_GroupId",
                schema: "Lnpa",
                table: "LaborRemunerationParametersValues",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_LaborRemunerationParametersValues_LRPId",
                schema: "Lnpa",
                table: "LaborRemunerationParametersValues",
                column: "LRPId");

            migrationBuilder.CreateIndex(
                name: "IX_LaborRemunerationParametersValues_StatuId",
                schema: "Lnpa",
                table: "LaborRemunerationParametersValues",
                column: "StatuId");

            migrationBuilder.CreateIndex(
                name: "IX_PayrollConcepts_LRPId",
                schema: "Lnpa",
                table: "PayrollConcepts",
                column: "LRPId");

            migrationBuilder.CreateIndex(
                name: "IX_PayrollConcepts_Name",
                schema: "Lnpa",
                table: "PayrollConcepts",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PayrollConcepts_StatuId",
                schema: "Lnpa",
                table: "PayrollConcepts",
                column: "StatuId");

            migrationBuilder.CreateIndex(
                name: "IX_PayrollSettlements_ChargeId",
                schema: "Lnpa",
                table: "PayrollSettlements",
                column: "ChargeId");

            migrationBuilder.CreateIndex(
                name: "IX_PayrollSettlements_PayrollConceptId",
                schema: "Lnpa",
                table: "PayrollSettlements",
                column: "PayrollConceptId");

            migrationBuilder.CreateIndex(
                name: "IX_PayrollSettlements_StatuId",
                schema: "Lnpa",
                table: "PayrollSettlements",
                column: "StatuId");

            migrationBuilder.CreateIndex(
                name: "IX_PayrollSettlements_UserId",
                schema: "Lnpa",
                table: "PayrollSettlements",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Charges_ChargeId",
                schema: "Admi",
                table: "AspNetUsers",
                column: "ChargeId",
                principalSchema: "Lnpa",
                principalTable: "Charges",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Charges_ChargeId",
                schema: "Admi",
                table: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "ConventionValues",
                schema: "Lnpa");

            migrationBuilder.DropTable(
                name: "LaborRemunerationParametersValues",
                schema: "Lnpa");

            migrationBuilder.DropTable(
                name: "LogAuditorias",
                schema: "Admi");

            migrationBuilder.DropTable(
                name: "PayrollSettlements",
                schema: "Lnpa");

            migrationBuilder.DropTable(
                name: "Conventions",
                schema: "Lnpa");

            migrationBuilder.DropTable(
                name: "Charges",
                schema: "Lnpa");

            migrationBuilder.DropTable(
                name: "PayrollConcepts",
                schema: "Lnpa");

            migrationBuilder.DropTable(
                name: "Groups",
                schema: "Lnpa");

            migrationBuilder.DropTable(
                name: "LaborRemunerationParameters",
                schema: "Lnpa");

            migrationBuilder.DropTable(
                name: "Categories",
                schema: "Lnpa");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_ChargeId",
                schema: "Admi",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ChargeId",
                schema: "Admi",
                table: "AspNetUsers");
        }
    }
}
