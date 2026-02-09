using FoodInspector.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TriggerModel = FoodInspector.Models.Trigger;

namespace FoodInspector.Data;

public class FoodInspectorDbContext : DbContext
{
    public DbSet<TriggerModel> Triggers { get; set; }
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

        // ?? Trigger ?????????????????????????????????????????????????????????
        modelBuilder.Entity<TriggerModel>(e =>
        {
            e.HasIndex(t => t.Name).IsUnique();

            e.Property(t => t.Severity)
                .HasConversion(new EnumToStringConverter<TriggerSeverity>());
        });

        // ?? TriggerSynonym ??????????????????????????????????????????????????
        modelBuilder.Entity<TriggerSynonym>(e =>
        {
            e.HasOne(s => s.Trigger)
                .WithMany()
                .HasForeignKey(s => s.TriggerId)
                .OnDelete(DeleteBehavior.Cascade);

            e.HasIndex(s => new { s.TriggerId, s.SynonymText }).IsUnique();

            e.Property(s => s.MatchType)
                .HasConversion(new EnumToStringConverter<TriggerMatchType>());
        });

        // ?? EvidenceSource ??????????????????????????????????????????????????
        modelBuilder.Entity<EvidenceSource>(e =>
        {
            e.HasIndex(ev => ev.CitationShort).IsUnique();
        });

        // ?? CrossReactivityRule ?????????????????????????????????????????????
        modelBuilder.Entity<CrossReactivityRule>(e =>
        {
            e.HasOne(r => r.SourceTrigger)
                .WithMany()
                .HasForeignKey(r => r.SourceTriggerId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(r => r.TargetTrigger)
                .WithMany()
                .HasForeignKey(r => r.TargetTriggerId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(r => r.EvidenceSource)
                .WithMany()
                .HasForeignKey(r => r.EvidenceSourceId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasIndex(r => new
            {
                r.SourceTriggerId,
                r.SourceCategory,
                r.TargetTriggerId,
                r.TargetCategory,
                r.EvidenceSourceId
            }).HasDatabaseName("IX_CrossReactivityRule_Composite");

            e.Property(r => r.Strength)
                .HasConversion(new EnumToStringConverter<CrossReactivityStrength>());
        });

        // ?? ScanRecord ??????????????????????????????????????????????????????
        modelBuilder.Entity<ScanRecord>(e =>
        {
            e.Property(s => s.FinalStatus)
                .HasConversion(new EnumToStringConverter<SafetyLevel>());
        });

        // ?? ScanMatch ???????????????????????????????????????????????????????
        modelBuilder.Entity<ScanMatch>(e =>
        {
            e.HasOne(m => m.ScanRecord)
                .WithMany(s => s.Matches)
                .HasForeignKey(m => m.ScanRecordId)
                .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(m => m.Trigger)
                .WithMany()
                .HasForeignKey(m => m.TriggerId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(m => m.EvidenceSource)
                .WithMany()
                .HasForeignKey(m => m.EvidenceSourceId)
                .OnDelete(DeleteBehavior.Restrict);

            e.Property(m => m.Reason)
                .HasConversion(new EnumToStringConverter<ScanMatchReason>());
        });

        // No hardcoded HasData — seeding is handled by SeedDataService
    }
}
