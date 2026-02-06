using FoodInspector.Models;

namespace FoodInspector.Services;

/// <summary>
/// Test-specific copy of IDatabaseService interface.
/// This avoids pulling in the full DatabaseService.cs with all its EF Core and MAUI dependencies.
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
