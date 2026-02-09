using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FoodInspector.Data.Migrations;

/// <summary>
/// Schema is created via EnsureCreated for MAUI local SQLite.
/// This migration file exists as a placeholder so EF tooling remains happy.
/// Indexes, enum-as-string conversions, and seed data are handled by
/// FoodInspectorDbContext.OnModelCreating and SeedDataService respectively.
/// </summary>
public partial class AddCrossReactivityOffline : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // No-op: EnsureCreated handles schema creation for MAUI SQLite.
        // SeedDataService handles idempotent data seeding from JSON resources.
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        // No-op
    }
}
