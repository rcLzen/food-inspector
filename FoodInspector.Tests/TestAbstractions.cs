using FoodInspector.Models;

namespace FoodInspector.Tests;

public interface IDatabaseService
{
    Task InitializeDatabaseAsync();
    Task<List<ScanRecord>> GetScanHistoryAsync();
    Task<ScanRecord> SaveScanHistoryAsync(ScanRecord scan);
    Task<List<Models.Trigger>> GetAllTriggersAsync();
    Task<List<TriggerSynonym>> GetAllSynonymsAsync();
    Task<List<CrossReactivityRule>> GetCrossReactivityRulesAsync();
    Task<AppSettings> GetSettingsAsync();
    Task<AppSettings> SaveSettingsAsync(AppSettings settings);
    Task DeleteScanHistoryAsync(int id);
    Task ToggleCrossReactivityRuleAsync(int id, bool enabled);
}
