using FoodInspector.Models;
using FoodInspector.Services;

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
    public async Task EmptyInput_ReturnsSafe()
    {
        var service = CreateService();
        var result = await service.MatchAsync("", flareModeOn: false);
        Assert.Equal(SafetyLevel.Safe, result.FinalStatus);
        Assert.Empty(result.DirectMatches);
        Assert.Empty(result.CrossReactiveMatches);
    }

    [Fact]
    public async Task MultipleDirectMatches_AllDetected()
    {
        var service = CreateService();
        var result = await service.MatchAsync("ingredients: soy, malt, yeast extract", flareModeOn: false);
        Assert.Contains(result.DirectMatches, x => x.TriggerName == "Soy");
        Assert.Contains(result.DirectMatches, x => x.TriggerName == "Malt");
        Assert.Contains(result.DirectMatches, x => x.TriggerName == "Yeast Extract");
    }

    [Fact]
    public async Task DisabledTrigger_NotMatched()
    {
        var service = CreateServiceWithDisabledTrigger();
        var result = await service.MatchAsync("ingredients: malt", flareModeOn: false);
        Assert.DoesNotContain(result.DirectMatches, x => x.TriggerName == "Malt");
    }

    [Fact]
    public async Task DisabledRule_NoExpansion()
    {
        var service = CreateServiceWithDisabledRule();
        var result = await service.MatchAsync("ingredients: soy", flareModeOn: false);
        Assert.Empty(result.CrossReactiveMatches);
    }

    [Fact]
    public async Task CaseSensitivity_HandledCorrectly()
    {
        var service = CreateService();
        var result = await service.MatchAsync("ingredients: SOY, MALT", flareModeOn: false);
        Assert.Contains(result.DirectMatches, x => x.TriggerName == "Soy");
        Assert.Contains(result.DirectMatches, x => x.TriggerName == "Malt");
    }

    [Fact]
    public async Task SpecialCharacters_Normalized()
    {
        var service = CreateService();
        var result = await service.MatchAsync("ingredients: (soy); malt", flareModeOn: false);
        Assert.Contains(result.DirectMatches, x => x.TriggerName == "Soy");
        Assert.Contains(result.DirectMatches, x => x.TriggerName == "Malt");
    }

    [Fact]
    public async Task MultiWordSynonym_Matched()
    {
        var service = CreateService();
        var result = await service.MatchAsync("ingredients: contains yeast extract", flareModeOn: false);
        Assert.Contains(result.DirectMatches, x => x.TriggerName == "Yeast Extract");
    }

    [Fact]
    public async Task OverlappingCrossReactive_NoDuplicates()
    {
        var service = CreateService();
        // Soy triggers cross-reactive Legumes, which is also triggered by soy
        var result = await service.MatchAsync("ingredients: soy", flareModeOn: false);
        var legumesMatches = result.CrossReactiveMatches.Where(x => x.TriggerName == "Legumes").ToList();
        Assert.Single(legumesMatches); // Should only appear once
    }

    [Fact]
    public async Task DirectMatchNotInCrossReactive()
    {
        var service = CreateService();
        // If soy is directly matched, it should not appear in cross-reactive matches
        var result = await service.MatchAsync("ingredients: soy", flareModeOn: false);
        Assert.DoesNotContain(result.CrossReactiveMatches, x => x.TriggerName == "Soy");
    }

    [Fact]
    public async Task LowSeverityOnly_ReturnsSafe()
    {
        var service = CreateServiceWithLowSeverity();
        var result = await service.MatchAsync("ingredients: lowrisk", flareModeOn: false);
        Assert.Equal(SafetyLevel.Safe, result.FinalStatus);
    }

    private static MatchingService CreateService()
    {
        return new MatchingService(new FakeDb(), new IngredientNormalizationService());
    }

    private static MatchingService CreateServiceWithDisabledTrigger()
    {
        return new MatchingService(new FakeDbWithDisabledTrigger(), new IngredientNormalizationService());
    }

    private static MatchingService CreateServiceWithDisabledRule()
    {
        return new MatchingService(new FakeDbWithDisabledRule(), new IngredientNormalizationService());
    }

    private static MatchingService CreateServiceWithLowSeverity()
    {
        return new MatchingService(new FakeDbWithLowSeverity(), new IngredientNormalizationService());
    }

    private class FakeDb : IDatabaseService
    {
        public Task<List<Trigger>> GetAllTriggersAsync() => Task.FromResult(new List<Trigger>
        {
            new() { Id = 3, Name = "Malt", Category = "Cereal", Severity = TriggerSeverity.High, Enabled = true },
            new() { Id = 4, Name = "Soy", Category = "Legume", Severity = TriggerSeverity.High, Enabled = true },
            new() { Id = 8, Name = "Yeast Extract", Category = "Fungi/Yeast", Severity = TriggerSeverity.Moderate, Enabled = true },
            new() { Id = 10, Name = "Legumes", Category = "Legume", Severity = TriggerSeverity.Moderate, Enabled = true }
        });

        public Task<List<TriggerSynonym>> GetAllSynonymsAsync() => Task.FromResult(new List<TriggerSynonym>
        {
            new() { Id = 1, TriggerId = 3, SynonymText = "malt", MatchType = TriggerMatchType.WordBoundaryContains, Trigger = new Trigger { Id = 3, Name = "Malt", Category = "Cereal", Severity = TriggerSeverity.High, Enabled = true } },
            new() { Id = 2, TriggerId = 4, SynonymText = "soy", MatchType = TriggerMatchType.WordBoundaryContains, Trigger = new Trigger { Id = 4, Name = "Soy", Category = "Legume", Severity = TriggerSeverity.High, Enabled = true } },
            new() { Id = 3, TriggerId = 8, SynonymText = "yeast extract", MatchType = TriggerMatchType.WordBoundaryContains, Trigger = new Trigger { Id = 8, Name = "Yeast Extract", Category = "Fungi/Yeast", Severity = TriggerSeverity.Moderate, Enabled = true } }
        });

        public Task<List<CrossReactivityRule>> GetCrossReactivityRulesAsync() => Task.FromResult(new List<CrossReactivityRule>
        {
            new() { Id = 1, SourceCategory = "Legume", TargetCategory = "Legume", Strength = CrossReactivityStrength.Medium, EvidenceSourceId = 2, Enabled = true, EvidenceSource = new EvidenceSource { Id = 2, CitationShort = "Bublin 2014", CitationFull = "Full citation", Summary = "Summary" } }
        });

        public Task InitializeDatabaseAsync() => Task.CompletedTask;
        public Task<List<ScanRecord>> GetScanHistoryAsync() => Task.FromResult(new List<ScanRecord>());
        public Task<ScanRecord> SaveScanHistoryAsync(ScanRecord scan) => Task.FromResult(scan);
        public Task<AppSettings> GetSettingsAsync() => Task.FromResult(new AppSettings());
        public Task<AppSettings> SaveSettingsAsync(AppSettings settings) => Task.FromResult(settings);
        public Task DeleteScanHistoryAsync(int id) => Task.CompletedTask;
        public Task ToggleCrossReactivityRuleAsync(int id, bool enabled) => Task.CompletedTask;
    }

    private class FakeDbWithDisabledTrigger : IDatabaseService
    {
        public Task<List<Trigger>> GetAllTriggersAsync() => Task.FromResult(new List<Trigger>
        {
            new() { Id = 3, Name = "Malt", Category = "Cereal", Severity = TriggerSeverity.High, Enabled = false }
        });

        public Task<List<TriggerSynonym>> GetAllSynonymsAsync() => Task.FromResult(new List<TriggerSynonym>
        {
            new() { Id = 1, TriggerId = 3, SynonymText = "malt", MatchType = TriggerMatchType.WordBoundaryContains, Trigger = new Trigger { Id = 3, Name = "Malt", Category = "Cereal", Severity = TriggerSeverity.High, Enabled = false } }
        });

        public Task<List<CrossReactivityRule>> GetCrossReactivityRulesAsync() => Task.FromResult(new List<CrossReactivityRule>());

        public Task InitializeDatabaseAsync() => Task.CompletedTask;
        public Task<List<ScanRecord>> GetScanHistoryAsync() => Task.FromResult(new List<ScanRecord>());
        public Task<ScanRecord> SaveScanHistoryAsync(ScanRecord scan) => Task.FromResult(scan);
        public Task<AppSettings> GetSettingsAsync() => Task.FromResult(new AppSettings());
        public Task<AppSettings> SaveSettingsAsync(AppSettings settings) => Task.FromResult(settings);
        public Task DeleteScanHistoryAsync(int id) => Task.CompletedTask;
        public Task ToggleCrossReactivityRuleAsync(int id, bool enabled) => Task.CompletedTask;
    }

    private class FakeDbWithDisabledRule : IDatabaseService
    {
        public Task<List<Trigger>> GetAllTriggersAsync() => Task.FromResult(new List<Trigger>
        {
            new() { Id = 4, Name = "Soy", Category = "Legume", Severity = TriggerSeverity.High, Enabled = true },
            new() { Id = 10, Name = "Legumes", Category = "Legume", Severity = TriggerSeverity.Moderate, Enabled = true }
        });

        public Task<List<TriggerSynonym>> GetAllSynonymsAsync() => Task.FromResult(new List<TriggerSynonym>
        {
            new() { Id = 2, TriggerId = 4, SynonymText = "soy", MatchType = TriggerMatchType.WordBoundaryContains, Trigger = new Trigger { Id = 4, Name = "Soy", Category = "Legume", Severity = TriggerSeverity.High, Enabled = true } }
        });

        public Task<List<CrossReactivityRule>> GetCrossReactivityRulesAsync() => Task.FromResult(new List<CrossReactivityRule>
        {
            new() { Id = 1, SourceCategory = "Legume", TargetCategory = "Legume", Strength = CrossReactivityStrength.Medium, EvidenceSourceId = 2, Enabled = false, EvidenceSource = new EvidenceSource { Id = 2, CitationShort = "Bublin 2014" } }
        });

        public Task InitializeDatabaseAsync() => Task.CompletedTask;
        public Task<List<ScanRecord>> GetScanHistoryAsync() => Task.FromResult(new List<ScanRecord>());
        public Task<ScanRecord> SaveScanHistoryAsync(ScanRecord scan) => Task.FromResult(scan);
        public Task<AppSettings> GetSettingsAsync() => Task.FromResult(new AppSettings());
        public Task<AppSettings> SaveSettingsAsync(AppSettings settings) => Task.FromResult(settings);
        public Task DeleteScanHistoryAsync(int id) => Task.CompletedTask;
        public Task ToggleCrossReactivityRuleAsync(int id, bool enabled) => Task.CompletedTask;
    }

    private class FakeDbWithLowSeverity : IDatabaseService
    {
        public Task<List<Trigger>> GetAllTriggersAsync() => Task.FromResult(new List<Trigger>
        {
            new() { Id = 99, Name = "LowRisk", Category = "Test", Severity = TriggerSeverity.Low, Enabled = true }
        });

        public Task<List<TriggerSynonym>> GetAllSynonymsAsync() => Task.FromResult(new List<TriggerSynonym>
        {
            new() { Id = 99, TriggerId = 99, SynonymText = "lowrisk", MatchType = TriggerMatchType.Exact, Trigger = new Trigger { Id = 99, Name = "LowRisk", Category = "Test", Severity = TriggerSeverity.Low, Enabled = true } }
        });

        public Task<List<CrossReactivityRule>> GetCrossReactivityRulesAsync() => Task.FromResult(new List<CrossReactivityRule>());

        public Task InitializeDatabaseAsync() => Task.CompletedTask;
        public Task<List<ScanRecord>> GetScanHistoryAsync() => Task.FromResult(new List<ScanRecord>());
        public Task<ScanRecord> SaveScanHistoryAsync(ScanRecord scan) => Task.FromResult(scan);
        public Task<AppSettings> GetSettingsAsync() => Task.FromResult(new AppSettings());
        public Task<AppSettings> SaveSettingsAsync(AppSettings settings) => Task.FromResult(settings);
        public Task DeleteScanHistoryAsync(int id) => Task.CompletedTask;
        public Task ToggleCrossReactivityRuleAsync(int id, bool enabled) => Task.CompletedTask;
    }
}
