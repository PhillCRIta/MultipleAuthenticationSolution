using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MasterPlanProject_V2.API.Migrations.MasterPlanDataDb
{
    /// <inheritdoc />
    public partial class startupdbmpp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LocalitaPuglia",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Area = table.Column<string>(type: "varchar(150)", nullable: false),
                    Localita = table.Column<string>(type: "varchar(150)", nullable: false),
                    DataInserimento = table.Column<DateTime>(type: "DateTime2(0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocalitaPuglia", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LocalUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "varchar(50)", nullable: false),
                    Password = table.Column<string>(type: "varchar(50)", nullable: false),
                    Role = table.Column<string>(type: "varchar(50)", nullable: false),
                    Name = table.Column<string>(type: "varchar(50)", nullable: false),
                    Email = table.Column<string>(type: "varchar(150)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocalUsers", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "LocalitaPuglia",
                columns: new[] { "Id", "Area", "DataInserimento", "Localita" },
                values: new object[] { 1, "Salento", new DateTime(1983, 9, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "Porto Cesareo" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LocalitaPuglia");

            migrationBuilder.DropTable(
                name: "LocalUsers");
        }
    }
}
