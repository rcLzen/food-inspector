using FoodInspector.Models;

namespace FoodInspector.Services;

/// <summary>
/// Test-only interface definition. The production code has the same interface in DatabaseService.cs.
/// This minimal definition avoids bringing in EF Core and other dependencies that are not needed for unit tests.
/// </summary>
public interface IDatabaseService
{
    Task InitializeDatabaseAsync();
    Task<List<ScanRecord>> GetScanHistoryAsync();
    Task<ScanRecord> SaveScanHistoryAsync(ScanRecord scan);
    Task<List<Trigger>> GetAllTriggersAsync();
    Task<List<TriggerSynonym>> GetAllSynonymsAsync();
    Task<List<CrossReactivityRule>> GetCrossReactivityRulesAsync();
    Task<AppSettings> GetSettingsAsync();
    Task<AppSettings> SaveSettingsAsync(AppSettings settings);
    Task DeleteScanHistoryAsync(int id);
    Task ToggleCrossReactivityRuleAsync(int id, bool enabled);
}
