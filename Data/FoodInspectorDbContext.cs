using Microsoft.EntityFrameworkCore;
using FoodInspector.Models;

namespace FoodInspector.Data;

public class FoodInspectorDbContext : DbContext
{
    public DbSet<FoodScanHistory> ScanHistory { get; set; }
    public DbSet<IngredientTrigger> IngredientTriggers { get; set; }
    public DbSet<IngredientSynonym> IngredientSynonyms { get; set; }
    public DbSet<CrossReactivity> CrossReactivities { get; set; }
    public DbSet<AppSettings> AppSettings { get; set; }

    public FoodInspectorDbContext(DbContextOptions<FoodInspectorDbContext> options)
        : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Configure SQLCipher encryption
        optionsBuilder.UseSqlite(options =>
        {
            // SQLCipher will be configured via connection string with password
        });
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure relationships
        modelBuilder.Entity<IngredientSynonym>()
            .HasOne(s => s.IngredientTrigger)
            .WithMany()
            .HasForeignKey(s => s.IngredientTriggerId);

        modelBuilder.Entity<CrossReactivity>()
            .HasOne(c => c.PrimaryTrigger)
            .WithMany()
            .HasForeignKey(c => c.PrimaryTriggerId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<CrossReactivity>()
            .HasOne(c => c.RelatedTrigger)
            .WithMany()
            .HasForeignKey(c => c.RelatedTriggerId)
            .OnDelete(DeleteBehavior.Restrict);

        // Seed data
        SeedData(modelBuilder);
    }

    private void SeedData(ModelBuilder modelBuilder)
    {
        // Common allergens and triggers
        var triggers = new List<IngredientTrigger>
        {
            new() { Id = 1, Name = "Peanuts", SafetyLevel = SafetyLevel.Avoid, Description = "Common allergen", SeverityScore = 10, IsCommonAllergen = true },
            new() { Id = 2, Name = "Tree Nuts", SafetyLevel = SafetyLevel.Avoid, Description = "Common allergen", SeverityScore = 10, IsCommonAllergen = true },
            new() { Id = 3, Name = "Milk", SafetyLevel = SafetyLevel.Avoid, Description = "Common allergen", SeverityScore = 8, IsCommonAllergen = true },
            new() { Id = 4, Name = "Eggs", SafetyLevel = SafetyLevel.Avoid, Description = "Common allergen", SeverityScore = 8, IsCommonAllergen = true },
            new() { Id = 5, Name = "Soy", SafetyLevel = SafetyLevel.Caution, Description = "Common allergen", SeverityScore = 7, IsCommonAllergen = true },
            new() { Id = 6, Name = "Wheat", SafetyLevel = SafetyLevel.Avoid, Description = "Contains gluten", SeverityScore = 9, IsCommonAllergen = true },
            new() { Id = 7, Name = "Fish", SafetyLevel = SafetyLevel.Avoid, Description = "Common allergen", SeverityScore = 9, IsCommonAllergen = true },
            new() { Id = 8, Name = "Shellfish", SafetyLevel = SafetyLevel.Avoid, Description = "Common allergen", SeverityScore = 10, IsCommonAllergen = true },
            new() { Id = 9, Name = "Sesame", SafetyLevel = SafetyLevel.Caution, Description = "Common allergen", SeverityScore = 7, IsCommonAllergen = true },
            new() { Id = 10, Name = "MSG", SafetyLevel = SafetyLevel.Caution, Description = "May cause sensitivity", SeverityScore = 5, IsCommonAllergen = false },
            new() { Id = 11, Name = "Sulfites", SafetyLevel = SafetyLevel.Caution, Description = "Preservative", SeverityScore = 6, IsCommonAllergen = false },
            new() { Id = 12, Name = "Artificial Colors", SafetyLevel = SafetyLevel.Caution, Description = "May cause sensitivity", SeverityScore = 4, IsCommonAllergen = false },
            new() { Id = 13, Name = "High Fructose Corn Syrup", SafetyLevel = SafetyLevel.Caution, Description = "Added sugar", SeverityScore = 3, IsCommonAllergen = false },
            new() { Id = 14, Name = "Trans Fats", SafetyLevel = SafetyLevel.Avoid, Description = "Unhealthy fats", SeverityScore = 8, IsCommonAllergen = false },
            new() { Id = 15, Name = "Gluten", SafetyLevel = SafetyLevel.Avoid, Description = "Protein in wheat, barley, rye", SeverityScore = 9, IsCommonAllergen = true }
        };

        // Synonyms for better matching
        var synonyms = new List<IngredientSynonym>
        {
            // Peanuts
            new() { Id = 1, IngredientTriggerId = 1, Synonym = "groundnuts" },
            new() { Id = 2, IngredientTriggerId = 1, Synonym = "arachis oil" },
            new() { Id = 3, IngredientTriggerId = 1, Synonym = "peanut oil" },
            
            // Tree Nuts
            new() { Id = 4, IngredientTriggerId = 2, Synonym = "almonds" },
            new() { Id = 5, IngredientTriggerId = 2, Synonym = "walnuts" },
            new() { Id = 6, IngredientTriggerId = 2, Synonym = "cashews" },
            new() { Id = 7, IngredientTriggerId = 2, Synonym = "pecans" },
            new() { Id = 8, IngredientTriggerId = 2, Synonym = "hazelnuts" },
            new() { Id = 9, IngredientTriggerId = 2, Synonym = "macadamia" },
            
            // Milk
            new() { Id = 10, IngredientTriggerId = 3, Synonym = "dairy" },
            new() { Id = 11, IngredientTriggerId = 3, Synonym = "lactose" },
            new() { Id = 12, IngredientTriggerId = 3, Synonym = "casein" },
            new() { Id = 13, IngredientTriggerId = 3, Synonym = "whey" },
            new() { Id = 14, IngredientTriggerId = 3, Synonym = "butter" },
            new() { Id = 15, IngredientTriggerId = 3, Synonym = "cheese" },
            new() { Id = 16, IngredientTriggerId = 3, Synonym = "cream" },
            
            // Eggs
            new() { Id = 17, IngredientTriggerId = 4, Synonym = "albumin" },
            new() { Id = 18, IngredientTriggerId = 4, Synonym = "egg white" },
            new() { Id = 19, IngredientTriggerId = 4, Synonym = "egg yolk" },
            
            // Soy
            new() { Id = 20, IngredientTriggerId = 5, Synonym = "soya" },
            new() { Id = 21, IngredientTriggerId = 5, Synonym = "soybean" },
            new() { Id = 22, IngredientTriggerId = 5, Synonym = "edamame" },
            new() { Id = 23, IngredientTriggerId = 5, Synonym = "tofu" },
            new() { Id = 24, IngredientTriggerId = 5, Synonym = "lecithin" },
            
            // Wheat
            new() { Id = 25, IngredientTriggerId = 6, Synonym = "wheat flour" },
            new() { Id = 26, IngredientTriggerId = 6, Synonym = "whole wheat" },
            new() { Id = 27, IngredientTriggerId = 6, Synonym = "semolina" },
            new() { Id = 28, IngredientTriggerId = 6, Synonym = "durum" },
            
            // Fish
            new() { Id = 29, IngredientTriggerId = 7, Synonym = "anchovy" },
            new() { Id = 30, IngredientTriggerId = 7, Synonym = "salmon" },
            new() { Id = 31, IngredientTriggerId = 7, Synonym = "tuna" },
            new() { Id = 32, IngredientTriggerId = 7, Synonym = "cod" },
            
            // Shellfish
            new() { Id = 33, IngredientTriggerId = 8, Synonym = "shrimp" },
            new() { Id = 34, IngredientTriggerId = 8, Synonym = "crab" },
            new() { Id = 35, IngredientTriggerId = 8, Synonym = "lobster" },
            new() { Id = 36, IngredientTriggerId = 8, Synonym = "prawns" },
            new() { Id = 37, IngredientTriggerId = 8, Synonym = "clams" },
            new() { Id = 38, IngredientTriggerId = 8, Synonym = "oysters" },
            
            // MSG
            new() { Id = 39, IngredientTriggerId = 10, Synonym = "monosodium glutamate" },
            new() { Id = 40, IngredientTriggerId = 10, Synonym = "glutamate" },
            new() { Id = 41, IngredientTriggerId = 10, Synonym = "yeast extract" },
            
            // Sulfites
            new() { Id = 42, IngredientTriggerId = 11, Synonym = "sulfur dioxide" },
            new() { Id = 43, IngredientTriggerId = 11, Synonym = "sodium bisulfite" },
            new() { Id = 44, IngredientTriggerId = 11, Synonym = "potassium metabisulfite" },
            
            // Artificial Colors
            new() { Id = 45, IngredientTriggerId = 12, Synonym = "red 40" },
            new() { Id = 46, IngredientTriggerId = 12, Synonym = "yellow 5" },
            new() { Id = 47, IngredientTriggerId = 12, Synonym = "blue 1" },
            new() { Id = 48, IngredientTriggerId = 12, Synonym = "tartrazine" },
            
            // HFCS
            new() { Id = 49, IngredientTriggerId = 13, Synonym = "corn syrup" },
            new() { Id = 50, IngredientTriggerId = 13, Synonym = "fructose" },
            
            // Trans Fats
            new() { Id = 51, IngredientTriggerId = 14, Synonym = "partially hydrogenated oil" },
            new() { Id = 52, IngredientTriggerId = 14, Synonym = "hydrogenated vegetable oil" },
            
            // Gluten
            new() { Id = 53, IngredientTriggerId = 15, Synonym = "barley" },
            new() { Id = 54, IngredientTriggerId = 15, Synonym = "rye" },
            new() { Id = 55, IngredientTriggerId = 15, Synonym = "malt" },
            new() { Id = 56, IngredientTriggerId = 15, Synonym = "wheat" }
        };

        // Cross-reactivities
        var crossReactivities = new List<CrossReactivity>
        {
            new() { Id = 1, PrimaryTriggerId = 1, RelatedTriggerId = 2, Description = "Peanut allergy may cross-react with tree nuts" },
            new() { Id = 2, PrimaryTriggerId = 6, RelatedTriggerId = 15, Description = "Wheat contains gluten" },
            new() { Id = 3, PrimaryTriggerId = 3, RelatedTriggerId = 4, Description = "Milk and egg proteins may cross-react" }
        };

        // Default settings
        var settings = new AppSettings
        {
            Id = 1,
            IsFlareMode = false,
            FlareModeThreshold = 5
        };

        modelBuilder.Entity<IngredientTrigger>().HasData(triggers);
        modelBuilder.Entity<IngredientSynonym>().HasData(synonyms);
        modelBuilder.Entity<CrossReactivity>().HasData(crossReactivities);
        modelBuilder.Entity<AppSettings>().HasData(settings);
    }
}
