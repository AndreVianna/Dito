using Microsoft.EntityFrameworkCore.Migrations;
using System.Diagnostics.CodeAnalysis;

#nullable disable

namespace VivaVoz.Migrations;

/// <inheritdoc />
[ExcludeFromCodeCoverage]
public partial class AddOverlayPosition : Migration {
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder) {
        migrationBuilder.AddColumn<double>(
            name: "OverlayX",
            table: "Settings",
            type: "REAL",
            nullable: true);

        migrationBuilder.AddColumn<double>(
            name: "OverlayY",
            table: "Settings",
            type: "REAL",
            nullable: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder) {
        migrationBuilder.DropColumn(name: "OverlayX", table: "Settings");
        migrationBuilder.DropColumn(name: "OverlayY", table: "Settings");
    }
}
