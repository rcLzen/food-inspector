using FoodInspector.Models;
using FoodInspector.Services;
using Xunit;
using TriggerModel = FoodInspector.Models.Trigger;

namespace FoodInspector.Tests;

public class MatchingServiceTests
{
    [Fact]
    public async Task WordBoundary_DoesNotMatch_Maltodextrin()
    {
        var service = CreateService();
        var result = await service.MatchAsync("ingredients: maltodextrin", flareModeOn: false);
        Assert.DoesNotContain(result.DirectMatches, x => x.TriggerName == "Malt");
    }

    [Fact]
    public async Task CrossReact_AttachesEvidenceId()
    {
        var service = CreateService();
        var result = await service.MatchAsync("ingredients: soy", flareModeOn: false);
        Assert.Contains(result.CrossReactiveMatches, x => x.EvidenceSourceId == 2);
    }

    [Fact]
    public async Task FlareMode_EscalatesModerate_ToAvoid()
    {
        var service = CreateService();
        var result = await service.MatchAsync("ingredients: yeast extract", flareModeOn: true);
        Assert.Equal(SafetyLevel.Avoid, result.FinalStatus);
    }

    private static MatchingService CreateService()
    {
        return new MatchingService(new FakeDb(), new IngredientNormalizationService());
    }

    private class FakeDb : FoodInspector.Services.IDatabaseService
    {
        public Task<List<TriggerModel>> GetAllTriggersAsync() => Task.FromResult(new List<TriggerModel>
        {
            new() { Id = 3, Name = "Malt", Category = "Cereal", Severity = TriggerSeverity.High, Enabled = true },
            new() { Id = 4, Name = "Soy", Category = "Legume", Severity = TriggerSeverity.High, Enabled = true },
            new() { Id = 8, Name = "Yeast Extract", Category = "Fungi/Yeast", Severity = TriggerSeverity.Moderate, Enabled = true },
            new() { Id = 10, Name = "Legumes", Category = "Legume", Severity = TriggerSeverity.Moderate, Enabled = true }
        });

        public Task<List<TriggerSynonym>> GetAllSynonymsAsync() => Task.FromResult(new List<TriggerSynonym>
        {
            new() { Id = 1, TriggerId = 3, SynonymText = "malt", MatchType = TriggerMatchType.WordBoundaryContains, Trigger = new TriggerModel { Id = 3, Name = "Malt", Category = "Cereal", Severity = TriggerSeverity.High, Enabled = true } },
            new() { Id = 2, TriggerId = 4, SynonymText = "soy", MatchType = TriggerMatchType.WordBoundaryContains, Trigger = new TriggerModel { Id = 4, Name = "Soy", Category = "Legume", Severity = TriggerSeverity.High, Enabled = true } },
            new() { Id = 3, TriggerId = 8, SynonymText = "yeast extract", MatchType = TriggerMatchType.WordBoundaryContains, Trigger = new TriggerModel { Id = 8, Name = "Yeast Extract", Category = "Fungi/Yeast", Severity = TriggerSeverity.Moderate, Enabled = true } }
        });

        public Task<List<CrossReactivityRule>> GetCrossReactivityRulesAsync() => Task.FromResult(new List<CrossReactivityRule>
        {
            new() { Id = 1, SourceCategory = "Legume", TargetCategory = "Legume", Strength = CrossReactivityStrength.Medium, EvidenceSourceId = 2, Enabled = true, EvidenceSource = new EvidenceSource { Id = 2, CitationShort = "Bublin 2014" } }
        });

        public Task InitializeDatabaseAsync() => Task.CompletedTask;
        public Task<List<ScanRecord>> GetScanHistoryAsync() => Task.FromResult(new List<ScanRecord>());
        public Task<ScanRecord> SaveScanHistoryAsync(ScanRecord scan) => Task.FromResult(scan);
        public Task<AppSettings> GetSettingsAsync() => Task.FromResult(new AppSettings());
        public Task<AppSettings> SaveSettingsAsync(AppSettings settings) => Task.FromResult(settings);
        public Task DeleteScanHistoryAsync(int id) => Task.CompletedTask;
        public Task ToggleCrossReactivityRuleAsync(int id, bool enabled) => Task.CompletedTask;
    }
}
