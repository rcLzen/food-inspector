using FoodInspector.Models;
using Microsoft.EntityFrameworkCore;
using TriggerModel = FoodInspector.Models.Trigger;

namespace FoodInspector.Data;

public class FoodInspectorDbContext : DbContext
{
    public DbSet<Models.Trigger> Triggers { get; set; }
    public DbSet<TriggerSynonym> TriggerSynonyms { get; set; }
    public DbSet<EvidenceSource> EvidenceSources { get; set; }
    public DbSet<CrossReactivityRule> CrossReactivityRules { get; set; }
    public DbSet<ScanRecord> ScanRecords { get; set; }
    public DbSet<ScanMatch> ScanMatches { get; set; }
    public DbSet<AppSettings> AppSettings { get; set; }

    public FoodInspectorDbContext(DbContextOptions<FoodInspectorDbContext> options) : base(options)
    {
    }

    //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //{
    //    optionsBuilder.UseSqlite();
    //}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<TriggerSynonym>()
            .HasOne(x => x.Trigger)
            .WithMany()
            .HasForeignKey(x => x.TriggerId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<CrossReactivityRule>()
            .HasOne(x => x.SourceTrigger)
            .WithMany()
            .HasForeignKey(x => x.SourceTriggerId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<CrossReactivityRule>()
            .HasOne(x => x.TargetTrigger)
            .WithMany()
            .HasForeignKey(x => x.TargetTriggerId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<CrossReactivityRule>()
            .HasOne(x => x.EvidenceSource)
            .WithMany()
            .HasForeignKey(x => x.EvidenceSourceId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ScanMatch>()
            .HasOne(x => x.ScanRecord)
            .WithMany(x => x.Matches)
            .HasForeignKey(x => x.ScanRecordId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ScanMatch>()
            .HasOne(x => x.Trigger)
            .WithMany()
            .HasForeignKey(x => x.TriggerId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ScanMatch>()
            .HasOne(x => x.EvidenceSource)
            .WithMany()
            .HasForeignKey(x => x.EvidenceSourceId)
            .OnDelete(DeleteBehavior.Restrict);

        SeedData(modelBuilder);
    }

    private static void SeedData(ModelBuilder modelBuilder)
    {
        // ============================================
        // TRIGGERS - Comprehensive allergen coverage
        // ============================================
        modelBuilder.Entity<TriggerModel>().HasData(
            // Cereals / Gluten
            new TriggerModel { Id = 1, Name = "Gluten", Category = "Cereal", Severity = TriggerSeverity.High, Enabled = true },
            new TriggerModel { Id = 2, Name = "Wheat", Category = "Cereal", Severity = TriggerSeverity.High, Enabled = true },
            new TriggerModel { Id = 3, Name = "Malt", Category = "Cereal", Severity = TriggerSeverity.High, Enabled = true },
            new TriggerModel { Id = 4, Name = "Barley", Category = "Cereal", Severity = TriggerSeverity.High, Enabled = true },
            new TriggerModel { Id = 5, Name = "Rye", Category = "Cereal", Severity = TriggerSeverity.High, Enabled = true },
            new TriggerModel { Id = 6, Name = "Oats", Category = "Cereal", Severity = TriggerSeverity.Moderate, Enabled = true },
            
            // Legumes
            new TriggerModel { Id = 7, Name = "Soy", Category = "Legume", Severity = TriggerSeverity.High, Enabled = true },
            new TriggerModel { Id = 8, Name = "Peanuts", Category = "Legume", Severity = TriggerSeverity.High, Enabled = true },
            new TriggerModel { Id = 9, Name = "Legumes", Category = "Legume", Severity = TriggerSeverity.Moderate, Enabled = true },
            new TriggerModel { Id = 10, Name = "Lupine", Category = "Legume", Severity = TriggerSeverity.High, Enabled = true },
            
            // Dairy
            new TriggerModel { Id = 11, Name = "Dairy Proteins", Category = "Dairy", Severity = TriggerSeverity.High, Enabled = true },
            new TriggerModel { Id = 12, Name = "Lactose", Category = "Dairy", Severity = TriggerSeverity.Moderate, Enabled = true },
            
            // Tree Nuts
            new TriggerModel { Id = 13, Name = "Tree Nuts", Category = "Nut", Severity = TriggerSeverity.High, Enabled = true },
            new TriggerModel { Id = 14, Name = "Almonds", Category = "Nut", Severity = TriggerSeverity.High, Enabled = true },
            new TriggerModel { Id = 15, Name = "Cashews", Category = "Nut", Severity = TriggerSeverity.High, Enabled = true },
            new TriggerModel { Id = 16, Name = "Walnuts", Category = "Nut", Severity = TriggerSeverity.High, Enabled = true },
            new TriggerModel { Id = 17, Name = "Pistachios", Category = "Nut", Severity = TriggerSeverity.High, Enabled = true },
            new TriggerModel { Id = 18, Name = "Hazelnuts", Category = "Nut", Severity = TriggerSeverity.High, Enabled = true },
            new TriggerModel { Id = 19, Name = "Brazil Nuts", Category = "Nut", Severity = TriggerSeverity.High, Enabled = true },
            new TriggerModel { Id = 20, Name = "Macadamia", Category = "Nut", Severity = TriggerSeverity.High, Enabled = true },
            new TriggerModel { Id = 21, Name = "Pine Nuts", Category = "Nut", Severity = TriggerSeverity.Moderate, Enabled = true },
            
            // Seeds
            new TriggerModel { Id = 22, Name = "Sesame", Category = "Seed", Severity = TriggerSeverity.High, Enabled = true },
            new TriggerModel { Id = 23, Name = "Mustard", Category = "Seed", Severity = TriggerSeverity.Moderate, Enabled = true },
            new TriggerModel { Id = 24, Name = "Sunflower Seeds", Category = "Seed", Severity = TriggerSeverity.Low, Enabled = true },
            
            // Seed Oils
            new TriggerModel { Id = 25, Name = "Canola Oil", Category = "Seed Oil", Severity = TriggerSeverity.High, Enabled = true },
            new TriggerModel { Id = 26, Name = "Soybean Oil", Category = "Seed Oil", Severity = TriggerSeverity.Moderate, Enabled = true },
            
            // Eggs
            new TriggerModel { Id = 27, Name = "Eggs", Category = "Egg", Severity = TriggerSeverity.High, Enabled = true },
            
            // Seafood
            new TriggerModel { Id = 28, Name = "Fish", Category = "Seafood", Severity = TriggerSeverity.High, Enabled = true },
            new TriggerModel { Id = 29, Name = "Shellfish", Category = "Seafood", Severity = TriggerSeverity.High, Enabled = true },
            new TriggerModel { Id = 30, Name = "Crustaceans", Category = "Seafood", Severity = TriggerSeverity.High, Enabled = true },
            new TriggerModel { Id = 31, Name = "Mollusks", Category = "Seafood", Severity = TriggerSeverity.High, Enabled = true },
            
            // Fungi/Yeast
            new TriggerModel { Id = 32, Name = "Yeast Extract", Category = "Fungi/Yeast", Severity = TriggerSeverity.Moderate, Enabled = true },
            new TriggerModel { Id = 33, Name = "Fungi/Yeast", Category = "Fungi/Yeast", Severity = TriggerSeverity.Moderate, Enabled = true },
            new TriggerModel { Id = 34, Name = "Mushrooms", Category = "Fungi/Yeast", Severity = TriggerSeverity.Low, Enabled = true },
            
            // Nightshades
            new TriggerModel { Id = 35, Name = "Potato/Nightshade", Category = "Nightshade", Severity = TriggerSeverity.Moderate, Enabled = true },
            new TriggerModel { Id = 36, Name = "Tomato", Category = "Nightshade", Severity = TriggerSeverity.Moderate, Enabled = true },
            new TriggerModel { Id = 37, Name = "Peppers", Category = "Nightshade", Severity = TriggerSeverity.Moderate, Enabled = true },
            new TriggerModel { Id = 38, Name = "Eggplant", Category = "Nightshade", Severity = TriggerSeverity.Moderate, Enabled = true },
            
            // Fruits (Oral Allergy Syndrome)
            new TriggerModel { Id = 39, Name = "Stone Fruits", Category = "Fruit", Severity = TriggerSeverity.Moderate, Enabled = true },
            new TriggerModel { Id = 40, Name = "Latex-Fruit", Category = "Fruit", Severity = TriggerSeverity.Moderate, Enabled = true },
            new TriggerModel { Id = 41, Name = "Citrus", Category = "Fruit", Severity = TriggerSeverity.Low, Enabled = true },
            
            // Sulfites & Additives
            new TriggerModel { Id = 42, Name = "Sulfites", Category = "Additive", Severity = TriggerSeverity.Moderate, Enabled = true },
            new TriggerModel { Id = 43, Name = "MSG", Category = "Additive", Severity = TriggerSeverity.Low, Enabled = true },
            new TriggerModel { Id = 44, Name = "Nitrates", Category = "Additive", Severity = TriggerSeverity.Low, Enabled = true },
            
            // Other
            new TriggerModel { Id = 45, Name = "Celery", Category = "Vegetable", Severity = TriggerSeverity.Moderate, Enabled = true },
            new TriggerModel { Id = 46, Name = "Latex", Category = "Other", Severity = TriggerSeverity.High, Enabled = true });

        // ============================================
        // SYNONYMS - Alternative ingredient names
        // ============================================
        modelBuilder.Entity<TriggerSynonym>().HasData(
            // Gluten (1)
            new TriggerSynonym { Id = 1, TriggerId = 1, SynonymText = "gliadin", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 2, TriggerId = 1, SynonymText = "gluten", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 3, TriggerId = 1, SynonymText = "seitan", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 4, TriggerId = 1, SynonymText = "vital wheat gluten", MatchType = TriggerMatchType.WordBoundaryContains },
            
            // Wheat (2)
            new TriggerSynonym { Id = 5, TriggerId = 2, SynonymText = "wheat", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 6, TriggerId = 2, SynonymText = "flour", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 7, TriggerId = 2, SynonymText = "semolina", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 8, TriggerId = 2, SynonymText = "durum", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 9, TriggerId = 2, SynonymText = "spelt", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 10, TriggerId = 2, SynonymText = "kamut", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 11, TriggerId = 2, SynonymText = "farro", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 12, TriggerId = 2, SynonymText = "triticale", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 13, TriggerId = 2, SynonymText = "einkorn", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 14, TriggerId = 2, SynonymText = "bulgur", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 15, TriggerId = 2, SynonymText = "couscous", MatchType = TriggerMatchType.WordBoundaryContains },
            
            // Malt (3)
            new TriggerSynonym { Id = 16, TriggerId = 3, SynonymText = "malt", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 17, TriggerId = 3, SynonymText = "malt extract", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 18, TriggerId = 3, SynonymText = "malt syrup", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 19, TriggerId = 3, SynonymText = "malt vinegar", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 20, TriggerId = 3, SynonymText = "malted", MatchType = TriggerMatchType.WordBoundaryContains },
            
            // Barley (4)
            new TriggerSynonym { Id = 21, TriggerId = 4, SynonymText = "barley", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 22, TriggerId = 4, SynonymText = "pearl barley", MatchType = TriggerMatchType.WordBoundaryContains },
            
            // Rye (5)
            new TriggerSynonym { Id = 23, TriggerId = 5, SynonymText = "rye", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 24, TriggerId = 5, SynonymText = "pumpernickel", MatchType = TriggerMatchType.WordBoundaryContains },
            
            // Oats (6)
            new TriggerSynonym { Id = 25, TriggerId = 6, SynonymText = "oats", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 26, TriggerId = 6, SynonymText = "oatmeal", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 27, TriggerId = 6, SynonymText = "oat flour", MatchType = TriggerMatchType.WordBoundaryContains },
            
            // Soy (7)
            new TriggerSynonym { Id = 28, TriggerId = 7, SynonymText = "soy", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 29, TriggerId = 7, SynonymText = "soya", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 30, TriggerId = 7, SynonymText = "soybean", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 31, TriggerId = 7, SynonymText = "soy lecithin", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 32, TriggerId = 7, SynonymText = "tofu", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 33, TriggerId = 7, SynonymText = "tempeh", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 34, TriggerId = 7, SynonymText = "edamame", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 35, TriggerId = 7, SynonymText = "miso", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 36, TriggerId = 7, SynonymText = "soy sauce", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 37, TriggerId = 7, SynonymText = "tamari", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 38, TriggerId = 7, SynonymText = "textured vegetable protein", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 39, TriggerId = 7, SynonymText = "tvp", MatchType = TriggerMatchType.WordBoundaryContains },
            
            // Peanuts (8)
            new TriggerSynonym { Id = 40, TriggerId = 8, SynonymText = "peanut", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 41, TriggerId = 8, SynonymText = "groundnut", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 42, TriggerId = 8, SynonymText = "arachis", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 43, TriggerId = 8, SynonymText = "goober", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 44, TriggerId = 8, SynonymText = "monkey nuts", MatchType = TriggerMatchType.WordBoundaryContains },
            
            // Legumes (9)
            new TriggerSynonym { Id = 45, TriggerId = 9, SynonymText = "beans", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 46, TriggerId = 9, SynonymText = "peas", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 47, TriggerId = 9, SynonymText = "lentils", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 48, TriggerId = 9, SynonymText = "chickpeas", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 49, TriggerId = 9, SynonymText = "garbanzo", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 50, TriggerId = 9, SynonymText = "hummus", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 51, TriggerId = 9, SynonymText = "fava", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 52, TriggerId = 9, SynonymText = "black beans", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 53, TriggerId = 9, SynonymText = "kidney beans", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 54, TriggerId = 9, SynonymText = "navy beans", MatchType = TriggerMatchType.WordBoundaryContains },
            
            // Lupine (10)
            new TriggerSynonym { Id = 55, TriggerId = 10, SynonymText = "lupine", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 56, TriggerId = 10, SynonymText = "lupin", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 57, TriggerId = 10, SynonymText = "lupini", MatchType = TriggerMatchType.WordBoundaryContains },
            
            // Dairy Proteins (11)
            new TriggerSynonym { Id = 58, TriggerId = 11, SynonymText = "casein", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 59, TriggerId = 11, SynonymText = "whey", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 60, TriggerId = 11, SynonymText = "milk solids", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 61, TriggerId = 11, SynonymText = "milk protein", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 62, TriggerId = 11, SynonymText = "lactalbumin", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 63, TriggerId = 11, SynonymText = "lactoglobulin", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 64, TriggerId = 11, SynonymText = "milk", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 65, TriggerId = 11, SynonymText = "cream", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 66, TriggerId = 11, SynonymText = "butter", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 67, TriggerId = 11, SynonymText = "cheese", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 68, TriggerId = 11, SynonymText = "yogurt", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 69, TriggerId = 11, SynonymText = "ghee", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 70, TriggerId = 11, SynonymText = "curd", MatchType = TriggerMatchType.WordBoundaryContains },
            
            // Lactose (12)
            new TriggerSynonym { Id = 71, TriggerId = 12, SynonymText = "lactose", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 72, TriggerId = 12, SynonymText = "milk sugar", MatchType = TriggerMatchType.WordBoundaryContains },
            
            // Tree Nuts (13)
            new TriggerSynonym { Id = 73, TriggerId = 13, SynonymText = "tree nuts", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 74, TriggerId = 13, SynonymText = "nut butter", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 75, TriggerId = 13, SynonymText = "nut oil", MatchType = TriggerMatchType.WordBoundaryContains },
            
            // Almonds (14)
            new TriggerSynonym { Id = 76, TriggerId = 14, SynonymText = "almond", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 77, TriggerId = 14, SynonymText = "marzipan", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 78, TriggerId = 14, SynonymText = "frangipane", MatchType = TriggerMatchType.WordBoundaryContains },
            
            // Cashews (15)
            new TriggerSynonym { Id = 79, TriggerId = 15, SynonymText = "cashew", MatchType = TriggerMatchType.WordBoundaryContains },
            
            // Walnuts (16)
            new TriggerSynonym { Id = 80, TriggerId = 16, SynonymText = "walnut", MatchType = TriggerMatchType.WordBoundaryContains },
            
            // Pistachios (17)
            new TriggerSynonym { Id = 81, TriggerId = 17, SynonymText = "pistachio", MatchType = TriggerMatchType.WordBoundaryContains },
            
            // Hazelnuts (18)
            new TriggerSynonym { Id = 82, TriggerId = 18, SynonymText = "hazelnut", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 83, TriggerId = 18, SynonymText = "filbert", MatchType = TriggerMatchType.WordBoundaryContains },
            
            // Brazil Nuts (19)
            new TriggerSynonym { Id = 84, TriggerId = 19, SynonymText = "brazil nut", MatchType = TriggerMatchType.WordBoundaryContains },
            
            // Macadamia (20)
            new TriggerSynonym { Id = 85, TriggerId = 20, SynonymText = "macadamia", MatchType = TriggerMatchType.WordBoundaryContains },
            
            // Pine Nuts (21)
            new TriggerSynonym { Id = 86, TriggerId = 21, SynonymText = "pine nut", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 87, TriggerId = 21, SynonymText = "pignoli", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 88, TriggerId = 21, SynonymText = "pinon", MatchType = TriggerMatchType.WordBoundaryContains },
            
            // Sesame (22)
            new TriggerSynonym { Id = 89, TriggerId = 22, SynonymText = "sesame", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 90, TriggerId = 22, SynonymText = "tahini", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 91, TriggerId = 22, SynonymText = "halvah", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 92, TriggerId = 22, SynonymText = "benne", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 93, TriggerId = 22, SynonymText = "gingelly", MatchType = TriggerMatchType.WordBoundaryContains },
            
            // Mustard (23)
            new TriggerSynonym { Id = 94, TriggerId = 23, SynonymText = "mustard", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 95, TriggerId = 23, SynonymText = "mustard seed", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 96, TriggerId = 23, SynonymText = "mustard oil", MatchType = TriggerMatchType.WordBoundaryContains },
            
            // Sunflower Seeds (24)
            new TriggerSynonym { Id = 97, TriggerId = 24, SynonymText = "sunflower seed", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 98, TriggerId = 24, SynonymText = "sunflower oil", MatchType = TriggerMatchType.WordBoundaryContains },
            
            // Canola Oil (25)
            new TriggerSynonym { Id = 99, TriggerId = 25, SynonymText = "canola oil", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 100, TriggerId = 25, SynonymText = "rapeseed oil", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 101, TriggerId = 25, SynonymText = "canola", MatchType = TriggerMatchType.WordBoundaryContains },
            
            // Soybean Oil (26)
            new TriggerSynonym { Id = 102, TriggerId = 26, SynonymText = "soybean oil", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 103, TriggerId = 26, SynonymText = "vegetable oil", MatchType = TriggerMatchType.WordBoundaryContains },
            
            // Eggs (27)
            new TriggerSynonym { Id = 104, TriggerId = 27, SynonymText = "egg", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 105, TriggerId = 27, SynonymText = "albumin", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 106, TriggerId = 27, SynonymText = "ovalbumin", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 107, TriggerId = 27, SynonymText = "ovomucoid", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 108, TriggerId = 27, SynonymText = "lysozyme", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 109, TriggerId = 27, SynonymText = "mayonnaise", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 110, TriggerId = 27, SynonymText = "meringue", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 111, TriggerId = 27, SynonymText = "globulin", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 112, TriggerId = 27, SynonymText = "lecithin", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 113, TriggerId = 27, SynonymText = "livetin", MatchType = TriggerMatchType.WordBoundaryContains },
            
            // Fish (28)
            new TriggerSynonym { Id = 114, TriggerId = 28, SynonymText = "fish", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 115, TriggerId = 28, SynonymText = "salmon", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 116, TriggerId = 28, SynonymText = "tuna", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 117, TriggerId = 28, SynonymText = "cod", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 118, TriggerId = 28, SynonymText = "anchovy", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 119, TriggerId = 28, SynonymText = "fish sauce", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 120, TriggerId = 28, SynonymText = "fish oil", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 121, TriggerId = 28, SynonymText = "omega-3", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 122, TriggerId = 28, SynonymText = "surimi", MatchType = TriggerMatchType.WordBoundaryContains },
            
            // Shellfish (29)
            new TriggerSynonym { Id = 123, TriggerId = 29, SynonymText = "shellfish", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 124, TriggerId = 29, SynonymText = "seafood", MatchType = TriggerMatchType.WordBoundaryContains },
            
            // Crustaceans (30)
            new TriggerSynonym { Id = 125, TriggerId = 30, SynonymText = "shrimp", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 126, TriggerId = 30, SynonymText = "prawn", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 127, TriggerId = 30, SynonymText = "crab", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 128, TriggerId = 30, SynonymText = "lobster", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 129, TriggerId = 30, SynonymText = "crawfish", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 130, TriggerId = 30, SynonymText = "crayfish", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 131, TriggerId = 30, SynonymText = "langoustine", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 132, TriggerId = 30, SynonymText = "scampi", MatchType = TriggerMatchType.WordBoundaryContains },
            
            // Mollusks (31)
            new TriggerSynonym { Id = 133, TriggerId = 31, SynonymText = "oyster", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 134, TriggerId = 31, SynonymText = "clam", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 135, TriggerId = 31, SynonymText = "mussel", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 136, TriggerId = 31, SynonymText = "scallop", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 137, TriggerId = 31, SynonymText = "squid", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 138, TriggerId = 31, SynonymText = "calamari", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 139, TriggerId = 31, SynonymText = "octopus", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 140, TriggerId = 31, SynonymText = "snail", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 141, TriggerId = 31, SynonymText = "escargot", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 142, TriggerId = 31, SynonymText = "abalone", MatchType = TriggerMatchType.WordBoundaryContains },
            
            // Yeast Extract (32)
            new TriggerSynonym { Id = 143, TriggerId = 32, SynonymText = "yeast extract", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 144, TriggerId = 32, SynonymText = "autolyzed yeast", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 145, TriggerId = 32, SynonymText = "hydrolyzed yeast", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 146, TriggerId = 32, SynonymText = "vegemite", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 147, TriggerId = 32, SynonymText = "marmite", MatchType = TriggerMatchType.WordBoundaryContains },
            
            // Fungi/Yeast (33)
            new TriggerSynonym { Id = 148, TriggerId = 33, SynonymText = "candida", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 149, TriggerId = 33, SynonymText = "aspergillus", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 150, TriggerId = 33, SynonymText = "yeast", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 151, TriggerId = 33, SynonymText = "brewer's yeast", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 152, TriggerId = 33, SynonymText = "baker's yeast", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 153, TriggerId = 33, SynonymText = "nutritional yeast", MatchType = TriggerMatchType.WordBoundaryContains },
            
            // Mushrooms (34)
            new TriggerSynonym { Id = 154, TriggerId = 34, SynonymText = "mushroom", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 155, TriggerId = 34, SynonymText = "shiitake", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 156, TriggerId = 34, SynonymText = "portobello", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 157, TriggerId = 34, SynonymText = "truffle", MatchType = TriggerMatchType.WordBoundaryContains },
            
            // Potato/Nightshade (35)
            new TriggerSynonym { Id = 158, TriggerId = 35, SynonymText = "potato", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 159, TriggerId = 35, SynonymText = "potato starch", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 160, TriggerId = 35, SynonymText = "nightshade", MatchType = TriggerMatchType.WordBoundaryContains },
            
            // Tomato (36)
            new TriggerSynonym { Id = 161, TriggerId = 36, SynonymText = "tomato", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 162, TriggerId = 36, SynonymText = "tomato paste", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 163, TriggerId = 36, SynonymText = "tomato sauce", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 164, TriggerId = 36, SynonymText = "ketchup", MatchType = TriggerMatchType.WordBoundaryContains },
            
            // Peppers (37)
            new TriggerSynonym { Id = 165, TriggerId = 37, SynonymText = "pepper", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 166, TriggerId = 37, SynonymText = "bell pepper", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 167, TriggerId = 37, SynonymText = "capsicum", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 168, TriggerId = 37, SynonymText = "chili", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 169, TriggerId = 37, SynonymText = "jalapeno", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 170, TriggerId = 37, SynonymText = "paprika", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 171, TriggerId = 37, SynonymText = "cayenne", MatchType = TriggerMatchType.WordBoundaryContains },
            
            // Eggplant (38)
            new TriggerSynonym { Id = 172, TriggerId = 38, SynonymText = "eggplant", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 173, TriggerId = 38, SynonymText = "aubergine", MatchType = TriggerMatchType.WordBoundaryContains },
            
            // Stone Fruits (39)
            new TriggerSynonym { Id = 174, TriggerId = 39, SynonymText = "peach", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 175, TriggerId = 39, SynonymText = "plum", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 176, TriggerId = 39, SynonymText = "cherry", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 177, TriggerId = 39, SynonymText = "apricot", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 178, TriggerId = 39, SynonymText = "nectarine", MatchType = TriggerMatchType.WordBoundaryContains },
            
            // Latex-Fruit (40)
            new TriggerSynonym { Id = 179, TriggerId = 40, SynonymText = "banana", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 180, TriggerId = 40, SynonymText = "avocado", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 181, TriggerId = 40, SynonymText = "kiwi", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 182, TriggerId = 40, SynonymText = "chestnut", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 183, TriggerId = 40, SynonymText = "papaya", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 184, TriggerId = 40, SynonymText = "mango", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 185, TriggerId = 40, SynonymText = "passion fruit", MatchType = TriggerMatchType.WordBoundaryContains },
            
            // Citrus (41)
            new TriggerSynonym { Id = 186, TriggerId = 41, SynonymText = "citrus", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 187, TriggerId = 41, SynonymText = "orange", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 188, TriggerId = 41, SynonymText = "lemon", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 189, TriggerId = 41, SynonymText = "lime", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 190, TriggerId = 41, SynonymText = "grapefruit", MatchType = TriggerMatchType.WordBoundaryContains },
            
            // Sulfites (42)
            new TriggerSynonym { Id = 191, TriggerId = 42, SynonymText = "sulfite", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 192, TriggerId = 42, SynonymText = "sulphite", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 193, TriggerId = 42, SynonymText = "sulfur dioxide", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 194, TriggerId = 42, SynonymText = "sodium sulfite", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 195, TriggerId = 42, SynonymText = "metabisulfite", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 196, TriggerId = 42, SynonymText = "E220", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 197, TriggerId = 42, SynonymText = "E221", MatchType = TriggerMatchType.WordBoundaryContains },
            
            // MSG (43)
            new TriggerSynonym { Id = 198, TriggerId = 43, SynonymText = "msg", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 199, TriggerId = 43, SynonymText = "monosodium glutamate", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 200, TriggerId = 43, SynonymText = "glutamate", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 201, TriggerId = 43, SynonymText = "E621", MatchType = TriggerMatchType.WordBoundaryContains },
            
            // Nitrates (44)
            new TriggerSynonym { Id = 202, TriggerId = 44, SynonymText = "nitrate", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 203, TriggerId = 44, SynonymText = "nitrite", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 204, TriggerId = 44, SynonymText = "sodium nitrate", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 205, TriggerId = 44, SynonymText = "sodium nitrite", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 206, TriggerId = 44, SynonymText = "E250", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 207, TriggerId = 44, SynonymText = "E251", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 208, TriggerId = 44, SynonymText = "curing salt", MatchType = TriggerMatchType.WordBoundaryContains },
            
            // Celery (45)
            new TriggerSynonym { Id = 209, TriggerId = 45, SynonymText = "celery", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 210, TriggerId = 45, SynonymText = "celeriac", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 211, TriggerId = 45, SynonymText = "celery salt", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 212, TriggerId = 45, SynonymText = "celery seed", MatchType = TriggerMatchType.WordBoundaryContains },
            
            // Latex (46)
            new TriggerSynonym { Id = 213, TriggerId = 46, SynonymText = "latex", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 214, TriggerId = 46, SynonymText = "natural rubber", MatchType = TriggerMatchType.WordBoundaryContains });

        // ============================================
        // EVIDENCE SOURCES - Scientific citations
        // ============================================
        modelBuilder.Entity<EvidenceSource>().HasData(
            new EvidenceSource
            {
                Id = 1,
                CitationShort = "Cox et al. 2021 (PubMed:33429724)",
                CitationFull = "Cox AL, Sicherer SH, Eigenmann PA. Clinical Relevance of Cross-Reactivity in Food Allergy. J Allergy Clin Immunol Pract. 2021.",
                Url = "https://pubmed.ncbi.nlm.nih.gov/33429724/",
                Summary = "Review emphasizes that immunologic cross-reactivity does not always imply clinical reactions; context and patient history matter.",
                ScopeTag = "Cross-reactivity overview"
            },
            new EvidenceSource
            {
                Id = 2,
                CitationShort = "Bublin & Breiteneder 2014 (PMCID:PMC3962743)",
                CitationFull = "Bublin M, Breiteneder H. Cross-reactivity of peanut allergens. 2014.",
                Url = "https://pmc.ncbi.nlm.nih.gov/articles/PMC3962743/",
                Summary = "Describes legume family relationships and cross-reactive allergen structures, including relevance among peanut, soy, and related legumes.",
                ScopeTag = "Legume cross-reactivity"
            },
            new EvidenceSource
            {
                Id = 3,
                CitationShort = "Jones et al. 1995 (PubMed:7560636)",
                CitationFull = "Jones SM et al. Immunologic cross-reactivity among cereal grains and grasses. J Allergy Clin Immunol. 1995.",
                Url = "https://pubmed.ncbi.nlm.nih.gov/7560636/",
                Summary = "Demonstrates immunologic overlap among cereal grains and grasses, supporting cautious grain-group mapping.",
                ScopeTag = "Cereal grain mapping"
            },
            new EvidenceSource
            {
                Id = 4,
                CitationShort = "Xing et al. 2022 (PMCID:PMC9568318)",
                CitationFull = "Xing H et al. Allergic Cross-Reactivity between Fungi and Foods. 2022.",
                Url = "https://pmc.ncbi.nlm.nih.gov/articles/PMC9568318/",
                Summary = "Reviews fungal-food cross-reactivity pathways and clinically relevant patterns for yeast/fungi-associated food sensitivity.",
                ScopeTag = "Fungi-food cross-reactivity"
            },
            new EvidenceSource
            {
                Id = 5,
                CitationShort = "Sicherer 2001 (PubMed:11242452)",
                CitationFull = "Sicherer SH. Clinical implications of cross-reactive food allergens. J Allergy Clin Immunol. 2001.",
                Url = "https://pubmed.ncbi.nlm.nih.gov/11242452/",
                Summary = "Comprehensive review of clinical cross-reactivity between tree nuts, including cashew-pistachio and walnut-pecan relationships.",
                ScopeTag = "Tree nut cross-reactivity"
            },
            new EvidenceSource
            {
                Id = 6,
                CitationShort = "Matricardi et al. 2016 (PMCID:PMC5071574)",
                CitationFull = "Matricardi PM et al. EAACI Molecular Allergology User's Guide. Pediatr Allergy Immunol. 2016.",
                Url = "https://pmc.ncbi.nlm.nih.gov/articles/PMC5071574/",
                Summary = "EAACI guide covering Oral Allergy Syndrome (OAS) and pollen-food relationships including birch-apple, mugwort-celery.",
                ScopeTag = "Pollen-food syndrome"
            },
            new EvidenceSource
            {
                Id = 7,
                CitationShort = "Wagner & Breiteneder 2002 (PubMed:12440950)",
                CitationFull = "Wagner S, Breiteneder H. The latex-fruit syndrome. Biochem Soc Trans. 2002.",
                Url = "https://pubmed.ncbi.nlm.nih.gov/12440950/",
                Summary = "Describes the latex-fruit syndrome including cross-reactivity between latex and banana, avocado, kiwi, and chestnut.",
                ScopeTag = "Latex-fruit syndrome"
            },
            new EvidenceSource
            {
                Id = 8,
                CitationShort = "Sampson 2004 (PubMed:15131567)",
                CitationFull = "Sampson HA. Update on food allergy. J Allergy Clin Immunol. 2004.",
                Url = "https://pubmed.ncbi.nlm.nih.gov/15131567/",
                Summary = "Reviews shellfish cross-reactivity patterns, including crustacean-mollusk relationships mediated by tropomyosin.",
                ScopeTag = "Shellfish cross-reactivity"
            },
            new EvidenceSource
            {
                Id = 9,
                CitationShort = "Bernhisel-Broadbent 1992 (PubMed:1602915)",
                CitationFull = "Bernhisel-Broadbent J et al. Cross-allergenicity in the legume botanical family. J Allergy Clin Immunol. 1992.",
                Url = "https://pubmed.ncbi.nlm.nih.gov/1602915/",
                Summary = "Foundational study on legume cross-reactivity showing that while serological cross-reactivity is common, clinical reactions are less frequent.",
                ScopeTag = "Legume family"
            },
            new EvidenceSource
            {
                Id = 10,
                CitationShort = "Venter et al. 2021 (PubMed:33678073)",
                CitationFull = "Venter C et al. The maternal diet index in pregnancy is associated with offspring allergic diseases. Allergy. 2021.",
                Url = "https://pubmed.ncbi.nlm.nih.gov/33678073/",
                Summary = "Discusses sesame-tree nut cross-reactivity and emerging sesame allergy patterns relevant to dietary avoidance.",
                ScopeTag = "Sesame allergy"
            },
            new EvidenceSource
            {
                Id = 11,
                CitationShort = "Fiocchi et al. 2004 (PubMed:15153182)",
                CitationFull = "Fiocchi A et al. Clinical tolerance to lactose in children with cow's milk allergy. Pediatrics. 2004.",
                Url = "https://pubmed.ncbi.nlm.nih.gov/15153182/",
                Summary = "Distinguishes between cow's milk protein allergy and lactose intolerance, and discusses cross-reactivity with goat and sheep milk.",
                ScopeTag = "Dairy cross-reactivity"
            },
            new EvidenceSource
            {
                Id = 12,
                CitationShort = "Pascual et al. 1999 (PubMed:10217558)",
                CitationFull = "Pascual C et al. Fish allergy: evaluation of the importance of cross-reactivity. J Pediatr. 1999.",
                Url = "https://pubmed.ncbi.nlm.nih.gov/10217558/",
                Summary = "Demonstrates high cross-reactivity among different fish species due to parvalbumin, the major fish allergen.",
                ScopeTag = "Fish cross-reactivity"
            },
            new EvidenceSource
            {
                Id = 13,
                CitationShort = "Wensing et al. 2002 (PubMed:12464954)",
                CitationFull = "Wensing M et al. The distribution of individual threshold doses eliciting allergic reactions in a population with peanut allergy. J Allergy Clin Immunol. 2002.",
                Url = "https://pubmed.ncbi.nlm.nih.gov/12464954/",
                Summary = "Discusses peanut-lupine cross-reactivity and threshold doses for allergic reactions.",
                ScopeTag = "Peanut-lupine"
            });

        // ============================================
        // CROSS-REACTIVITY RULES - Clinical relationships
        // ============================================
        modelBuilder.Entity<CrossReactivityRule>().HasData(
            // Cereal cross-reactivity
            new CrossReactivityRule { Id = 1, SourceCategory = "Cereal", TargetCategory = "Cereal", Strength = CrossReactivityStrength.High, EvidenceSourceId = 3, Notes = "Immunologic overlap among gluten-containing grains (wheat, barley, rye). Patients with celiac disease should avoid all.", Enabled = true },
            new CrossReactivityRule { Id = 2, SourceTriggerId = 2, TargetTriggerId = 4, Strength = CrossReactivityStrength.High, EvidenceSourceId = 3, Notes = "Wheat and barley share prolamins causing cross-reactivity in celiac and wheat allergy.", Enabled = true },
            new CrossReactivityRule { Id = 3, SourceTriggerId = 2, TargetTriggerId = 5, Strength = CrossReactivityStrength.High, EvidenceSourceId = 3, Notes = "Wheat and rye share secalin/gliadin homology.", Enabled = true },
            new CrossReactivityRule { Id = 4, SourceTriggerId = 4, TargetTriggerId = 3, Strength = CrossReactivityStrength.High, EvidenceSourceId = 3, Notes = "Barley is the source of malt; direct relationship.", Enabled = true },
            
            // Legume cross-reactivity
            new CrossReactivityRule { Id = 5, SourceCategory = "Legume", TargetCategory = "Legume", Strength = CrossReactivityStrength.Medium, EvidenceSourceId = 2, Notes = "Conservative legume family expansion: peanut/soy/pea/beans share allergen structures but clinical cross-reactivity is less common than serological.", Enabled = true },
            new CrossReactivityRule { Id = 6, SourceTriggerId = 8, TargetTriggerId = 10, Strength = CrossReactivityStrength.High, EvidenceSourceId = 13, Notes = "Peanut-allergic patients have ~50% risk of lupine allergy. EU requires lupine labeling.", Enabled = true },
            new CrossReactivityRule { Id = 7, SourceTriggerId = 8, TargetTriggerId = 7, Strength = CrossReactivityStrength.Low, EvidenceSourceId = 9, Notes = "Peanut-soy serological cross-reactivity is common (~85%) but clinical reactions are rare (~5%).", Enabled = true },
            new CrossReactivityRule { Id = 8, SourceTriggerId = 8, TargetTriggerId = 9, Strength = CrossReactivityStrength.Low, EvidenceSourceId = 9, Notes = "Peanut cross-reacts with other legumes but clinical significance is low for most patients.", Enabled = true },
            
            // Tree nut cross-reactivity
            new CrossReactivityRule { Id = 9, SourceCategory = "Nut", TargetCategory = "Nut", Strength = CrossReactivityStrength.Medium, EvidenceSourceId = 5, Notes = "Tree nut allergies often co-occur but specific pairs have higher cross-reactivity.", Enabled = true },
            new CrossReactivityRule { Id = 10, SourceTriggerId = 15, TargetTriggerId = 17, Strength = CrossReactivityStrength.High, EvidenceSourceId = 5, Notes = "Cashew and pistachio belong to Anacardiaceae family with >90% cross-reactivity.", Enabled = true },
            new CrossReactivityRule { Id = 11, SourceTriggerId = 16, TargetTriggerId = 21, Strength = CrossReactivityStrength.Medium, EvidenceSourceId = 5, Notes = "Walnut and pecan are both Juglandaceae with significant cross-reactivity.", Enabled = true },
            new CrossReactivityRule { Id = 12, SourceTriggerId = 18, TargetTriggerId = 14, Strength = CrossReactivityStrength.Medium, EvidenceSourceId = 6, Notes = "Hazelnut and almond cross-react via PR-10 proteins (Bet v 1 homologs) in birch-pollen allergic patients.", Enabled = true },
            new CrossReactivityRule { Id = 13, SourceTriggerId = 8, TargetTriggerId = 13, Strength = CrossReactivityStrength.Low, EvidenceSourceId = 5, Notes = "Peanut (legume) has limited cross-reactivity with tree nuts but co-allergy is common.", Enabled = true },
            
            // Seafood cross-reactivity
            new CrossReactivityRule { Id = 14, SourceCategory = "Seafood", TargetCategory = "Seafood", Strength = CrossReactivityStrength.Medium, EvidenceSourceId = 8, Notes = "Shellfish and fish are distinct allergen groups; cross-reactivity within groups is more common than between.", Enabled = true },
            new CrossReactivityRule { Id = 15, SourceTriggerId = 30, TargetTriggerId = 31, Strength = CrossReactivityStrength.Medium, EvidenceSourceId = 8, Notes = "Crustacean-mollusk cross-reactivity via tropomyosin; ~75% of crustacean-allergic patients tolerate mollusks.", Enabled = true },
            new CrossReactivityRule { Id = 16, SourceTriggerId = 28, TargetTriggerId = 28, Strength = CrossReactivityStrength.High, EvidenceSourceId = 12, Notes = "High cross-reactivity between different fish species due to parvalbumin conservation.", Enabled = true },
            new CrossReactivityRule { Id = 17, SourceTriggerId = 30, TargetTriggerId = 30, Strength = CrossReactivityStrength.High, EvidenceSourceId = 8, Notes = "Crustacean species (shrimp, crab, lobster) show high mutual cross-reactivity.", Enabled = true },
            
            // Fungi/Yeast cross-reactivity
            new CrossReactivityRule { Id = 18, SourceCategory = "Fungi/Yeast", TargetCategory = "Fungi/Yeast", Strength = CrossReactivityStrength.Medium, EvidenceSourceId = 4, Notes = "Yeast extract and fungi/fermented food relationships can cross-react in sensitive individuals.", Enabled = true },
            new CrossReactivityRule { Id = 19, SourceTriggerId = 33, TargetTriggerId = 34, Strength = CrossReactivityStrength.Medium, EvidenceSourceId = 4, Notes = "Yeast-allergic individuals may react to mushrooms and other fungi.", Enabled = true },
            
            // Latex-Fruit Syndrome
            new CrossReactivityRule { Id = 20, SourceTriggerId = 46, TargetTriggerId = 40, Strength = CrossReactivityStrength.High, EvidenceSourceId = 7, Notes = "Latex-fruit syndrome: 30-50% of latex-allergic patients react to banana, avocado, kiwi, or chestnut.", Enabled = true },
            new CrossReactivityRule { Id = 21, SourceTriggerId = 40, TargetTriggerId = 46, Strength = CrossReactivityStrength.Medium, EvidenceSourceId = 7, Notes = "Patients allergic to latex-associated fruits should be evaluated for latex allergy.", Enabled = true },
            
            // Pollen-Food (Oral Allergy Syndrome)
            new CrossReactivityRule { Id = 22, SourceTriggerId = 39, TargetTriggerId = 18, Strength = CrossReactivityStrength.Medium, EvidenceSourceId = 6, Notes = "Birch pollen cross-reacts with stone fruits and hazelnut (PR-10 proteins). Most common OAS trigger.", Enabled = true },
            new CrossReactivityRule { Id = 23, SourceTriggerId = 45, TargetTriggerId = 37, Strength = CrossReactivityStrength.Medium, EvidenceSourceId = 6, Notes = "Celery-mugwort-spice syndrome: celery, pepper, and carrot share cross-reactive allergens.", Enabled = true },
            
            // Nightshade cross-reactivity
            new CrossReactivityRule { Id = 24, SourceCategory = "Nightshade", TargetCategory = "Nightshade", Strength = CrossReactivityStrength.Medium, EvidenceSourceId = 1, Notes = "Nightshade family members share glycoalkaloids and may cross-react in sensitive individuals.", Enabled = true },
            new CrossReactivityRule { Id = 25, SourceTriggerId = 35, TargetTriggerId = 36, Strength = CrossReactivityStrength.Low, EvidenceSourceId = 1, Notes = "Potato and tomato are both Solanaceae but clinical cross-reactivity is uncommon.", Enabled = true },
            
            // Dairy cross-reactivity
            new CrossReactivityRule { Id = 26, SourceTriggerId = 11, TargetTriggerId = 12, Strength = CrossReactivityStrength.Low, EvidenceSourceId = 11, Notes = "Milk protein allergy and lactose intolerance are distinct conditions; lactose-free products may still contain milk proteins.", Enabled = true },
            
            // Sesame cross-reactivity
            new CrossReactivityRule { Id = 27, SourceTriggerId = 22, TargetTriggerId = 13, Strength = CrossReactivityStrength.Low, EvidenceSourceId = 10, Notes = "Sesame and tree nut co-allergies are common but cross-reactivity mechanism is not well established.", Enabled = true },
            
            // Seed oil cross-reactivity
            new CrossReactivityRule { Id = 28, SourceTriggerId = 25, TargetTriggerId = 23, Strength = CrossReactivityStrength.Low, EvidenceSourceId = 1, Notes = "Canola (rapeseed) and mustard are both Brassicaceae; some patients with mustard allergy react to canola.", Enabled = true },
            
            // Egg considerations
            new CrossReactivityRule { Id = 29, SourceTriggerId = 27, TargetTriggerId = 28, Strength = CrossReactivityStrength.Low, EvidenceSourceId = 1, Notes = "Egg allergy does not typically cross-react with fish, but fish roe contains different allergens than fish muscle.", Enabled = true });

        modelBuilder.Entity<AppSettings>().HasData(new AppSettings { Id = 1, IsFlareMode = false, FlareModeThreshold = 5 });
    }
}
