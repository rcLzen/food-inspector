using FoodInspector.Models;
using FoodInspector.Services;
using Xunit;

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

    [Fact]
    public async Task EmptyIngredients_ReturnsNoMatches()
    {
        var service = CreateService();
        var result = await service.MatchAsync("", flareModeOn: false);
        Assert.Empty(result.DirectMatches);
        Assert.Empty(result.CrossReactiveMatches);
        Assert.Equal(SafetyLevel.Safe, result.FinalStatus);
    }

    [Fact]
    public async Task MultipleDirectMatches_AllDetected()
    {
        var service = CreateService();
        var result = await service.MatchAsync("ingredients: soy, malt, yeast extract", flareModeOn: false);
        Assert.Equal(3, result.DirectMatches.Count);
        Assert.Contains(result.DirectMatches, x => x.TriggerName == "Soy");
        Assert.Contains(result.DirectMatches, x => x.TriggerName == "Malt");
        Assert.Contains(result.DirectMatches, x => x.TriggerName == "Yeast Extract");
    }

    [Fact]
    public async Task DisabledTrigger_NotMatched()
    {
        var service = CreateService();
        var result = await service.MatchAsync("ingredients: disabled-trigger", flareModeOn: false);
        Assert.DoesNotContain(result.DirectMatches, x => x.TriggerName == "Disabled Trigger");
    }

    [Fact]
    public async Task CaseInsensitive_MatchesCorrectly()
    {
        var service = CreateService();
        var result = await service.MatchAsync("ingredients: SOY, MaLt, YEAST EXTRACT", flareModeOn: false);
        Assert.Equal(3, result.DirectMatches.Count);
    }

    [Fact]
    public async Task SpecialCharacters_HandledCorrectly()
    {
        var service = CreateService();
        var result = await service.MatchAsync("ingredients: soy (lecithin); malt [extract]", flareModeOn: false);
        Assert.Contains(result.DirectMatches, x => x.TriggerName == "Soy");
        Assert.Contains(result.DirectMatches, x => x.TriggerName == "Malt");
    }

    [Fact]
    public async Task MultiWordSynonym_MatchesCorrectly()
    {
        var service = CreateService();
        var result = await service.MatchAsync("ingredients: yeast extract, other stuff", flareModeOn: false);
        Assert.Contains(result.DirectMatches, x => x.TriggerName == "Yeast Extract");
    }

    [Fact]
    public async Task OverlappingCrossReactiveMatches_NoDuplicates()
    {
        var service = CreateService();
        var result = await service.MatchAsync("ingredients: soy", flareModeOn: false);
        // Soy triggers cross-reactivity to Legumes
        var legumesMatches = result.CrossReactiveMatches.Where(x => x.TriggerName == "Legumes").ToList();
        Assert.Single(legumesMatches); // Should only appear once
    }

    [Fact]
    public async Task DirectMatch_NotDuplicatedInCrossReactive()
    {
        var service = CreateService();
        var result = await service.MatchAsync("ingredients: soy, legumes", flareModeOn: false);
        // Both Soy and Legumes are direct matches, Legumes should not appear in cross-reactive
        Assert.Contains(result.DirectMatches, x => x.TriggerName == "Legumes");
        Assert.DoesNotContain(result.CrossReactiveMatches, x => x.TriggerName == "Legumes");
    }

    private static MatchingService CreateService()
    {
        return new MatchingService(new FakeDb(), new IngredientNormalizationService());
    }

    private class FakeDb : IDatabaseService
    {
        public Task<List<Trigger>> GetAllTriggersAsync() => Task.FromResult(new List<Trigger>
        {
            new() { Id = 3, Name = "Malt", Category = "Cereal", Severity = TriggerSeverity.High, Enabled = true },
            new() { Id = 4, Name = "Soy", Category = "Legume", Severity = TriggerSeverity.High, Enabled = true },
            new() { Id = 8, Name = "Yeast Extract", Category = "Fungi/Yeast", Severity = TriggerSeverity.Moderate, Enabled = true },
            new() { Id = 10, Name = "Legumes", Category = "Legume", Severity = TriggerSeverity.Moderate, Enabled = true },
            new() { Id = 99, Name = "Disabled Trigger", Category = "Test", Severity = TriggerSeverity.High, Enabled = false }
        });

        public Task<List<TriggerSynonym>> GetAllSynonymsAsync() => Task.FromResult(new List<TriggerSynonym>
        {
            new() { Id = 1, TriggerId = 3, SynonymText = "malt", MatchType = TriggerMatchType.WordBoundaryContains, Trigger = new Trigger { Id = 3, Name = "Malt", Category = "Cereal", Severity = TriggerSeverity.High, Enabled = true } },
            new() { Id = 2, TriggerId = 4, SynonymText = "soy", MatchType = TriggerMatchType.WordBoundaryContains, Trigger = new Trigger { Id = 4, Name = "Soy", Category = "Legume", Severity = TriggerSeverity.High, Enabled = true } },
            new() { Id = 3, TriggerId = 8, SynonymText = "yeast extract", MatchType = TriggerMatchType.WordBoundaryContains, Trigger = new Trigger { Id = 8, Name = "Yeast Extract", Category = "Fungi/Yeast", Severity = TriggerSeverity.Moderate, Enabled = true } }
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
