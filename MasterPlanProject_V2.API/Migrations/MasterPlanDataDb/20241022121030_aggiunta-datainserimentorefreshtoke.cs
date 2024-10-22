using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MasterPlanProject_V2.API.Migrations.MasterPlanDataDb
{
    /// <inheritdoc />
    public partial class aggiuntadatainserimentorefreshtoke : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DataInserimento",
                table: "RefreshTokens",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "getdate()");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DataInserimento",
                table: "RefreshTokens");
        }
    }
}
