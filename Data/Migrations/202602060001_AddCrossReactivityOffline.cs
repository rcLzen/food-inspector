using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FoodInspector.Data.Migrations;

/// <summary>
/// This migration is intentionally empty because the MAUI app uses EnsureCreated for local SQLite databases.
/// EnsureCreated and migrations are mutually exclusive in EF Core - once a database is created with EnsureCreated,
/// migrations cannot be applied to it. This approach is acceptable for mobile apps with local user databases
/// that can be recreated if the schema changes. For production scenarios requiring schema evolution on existing
/// databases, consider switching to a migrations-only approach by removing EnsureCreated and implementing
/// proper Up/Down methods in migrations.
/// </summary>
public partial class AddCrossReactivityOffline : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // Schema is created via EnsureCreated for MAUI local SQLite in this build.
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
    }
}
