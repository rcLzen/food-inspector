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

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite();
    }

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
        modelBuilder.Entity<TriggerModel>().HasData(
            new TriggerModel { Id = 1, Name = "Gluten", Category = "Cereal", Severity = TriggerSeverity.High, Enabled = true },
            new TriggerModel { Id = 2, Name = "Wheat", Category = "Cereal", Severity = TriggerSeverity.High, Enabled = true },
            new TriggerModel { Id = 3, Name = "Malt", Category = "Cereal", Severity = TriggerSeverity.High, Enabled = true },
            new TriggerModel { Id = 4, Name = "Soy", Category = "Legume", Severity = TriggerSeverity.High, Enabled = true },
            new TriggerModel { Id = 5, Name = "Dairy Proteins", Category = "Dairy", Severity = TriggerSeverity.High, Enabled = true },
            new TriggerModel { Id = 6, Name = "Tree Nuts", Category = "Nut", Severity = TriggerSeverity.High, Enabled = true },
            new TriggerModel { Id = 7, Name = "Canola Oil", Category = "Seed Oil", Severity = TriggerSeverity.High, Enabled = true },
            new TriggerModel { Id = 8, Name = "Yeast Extract", Category = "Fungi/Yeast", Severity = TriggerSeverity.Moderate, Enabled = true },
            new TriggerModel { Id = 9, Name = "Fungi/Yeast", Category = "Fungi/Yeast", Severity = TriggerSeverity.Moderate, Enabled = true },
            new TriggerModel { Id = 10, Name = "Legumes", Category = "Legume", Severity = TriggerSeverity.Moderate, Enabled = true },
            new TriggerModel { Id = 11, Name = "Potato/Nightshade", Category = "Nightshade", Severity = TriggerSeverity.Moderate, Enabled = true });

        modelBuilder.Entity<TriggerSynonym>().HasData(
            new TriggerSynonym { Id = 1, TriggerId = 1, SynonymText = "gliadin", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 2, TriggerId = 1, SynonymText = "gluten", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 3, TriggerId = 2, SynonymText = "wheat", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 4, TriggerId = 3, SynonymText = "malt", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 5, TriggerId = 4, SynonymText = "soy", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 6, TriggerId = 4, SynonymText = "soybean", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 7, TriggerId = 5, SynonymText = "casein", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 8, TriggerId = 5, SynonymText = "whey", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 9, TriggerId = 5, SynonymText = "milk solids", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 10, TriggerId = 6, SynonymText = "almond", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 11, TriggerId = 6, SynonymText = "cashew", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 12, TriggerId = 6, SynonymText = "hazelnut", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 13, TriggerId = 6, SynonymText = "walnut", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 14, TriggerId = 7, SynonymText = "canola oil", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 15, TriggerId = 7, SynonymText = "rapeseed oil", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 16, TriggerId = 8, SynonymText = "yeast extract", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 17, TriggerId = 9, SynonymText = "candida", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 18, TriggerId = 9, SynonymText = "aspergillus", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 19, TriggerId = 10, SynonymText = "beans", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 20, TriggerId = 10, SynonymText = "peas", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 21, TriggerId = 11, SynonymText = "potato", MatchType = TriggerMatchType.WordBoundaryContains },
            new TriggerSynonym { Id = 22, TriggerId = 11, SynonymText = "nightshade", MatchType = TriggerMatchType.WordBoundaryContains });

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
            });

        modelBuilder.Entity<CrossReactivityRule>().HasData(
            new CrossReactivityRule { Id = 1, SourceCategory = "Legume", TargetCategory = "Legume", Strength = CrossReactivityStrength.Medium, EvidenceSourceId = 2, Notes = "Conservative legume family expansion: peanut/soy/pea/beans relationships.", Enabled = true },
            new CrossReactivityRule { Id = 2, SourceCategory = "Fungi/Yeast", TargetCategory = "Fungi/Yeast", Strength = CrossReactivityStrength.Medium, EvidenceSourceId = 4, Notes = "Yeast extract and fungi/fermented food relationships can cross-react in sensitive individuals.", Enabled = true },
            new CrossReactivityRule { Id = 3, SourceCategory = "Cereal", TargetCategory = "Cereal", Strength = CrossReactivityStrength.High, EvidenceSourceId = 1, Notes = "Diet policy: cereal/gluten mapping. Clinical relevance varies by patient.", Enabled = true },
            new CrossReactivityRule { Id = 4, SourceCategory = "Cereal", TargetCategory = "Cereal", Strength = CrossReactivityStrength.High, EvidenceSourceId = 3, Notes = "Supporting cereal-grain immunologic cross-reactivity evidence.", Enabled = true });

        modelBuilder.Entity<AppSettings>().HasData(new AppSettings { Id = 1, IsFlareMode = false, FlareModeThreshold = 5 });
    }
}
