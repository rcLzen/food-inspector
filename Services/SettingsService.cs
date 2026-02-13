namespace FoodInspector.Services;

public interface ISettingsService
{
    Task<bool> GetFlareModeAsync();
    Task SetFlareModeAsync(bool enabled);
    Task<int> GetFlareModeThresholdAsync();
    Task SetFlareModeThresholdAsync(int threshold);
}

public class SettingsService : ISettingsService
{
    private readonly IDatabaseService _databaseService;

    public SettingsService(IDatabaseService databaseService)
    {
        _databaseService = databaseService;
    }

    public async Task<bool> GetFlareModeAsync()
    {
        var settings = await _databaseService.GetSettingsAsync();
        return settings.IsFlareMode;
    }

    public async Task SetFlareModeAsync(bool enabled)
    {
        var settings = await _databaseService.GetSettingsAsync();
        settings.IsFlareMode = enabled;
        await _databaseService.SaveSettingsAsync(settings);
    }

    public async Task<int> GetFlareModeThresholdAsync()
    {
        var settings = await _databaseService.GetSettingsAsync();
        return settings.FlareModeThreshold;
    }

    public async Task SetFlareModeThresholdAsync(int threshold)
    {
        var settings = await _databaseService.GetSettingsAsync();
        settings.FlareModeThreshold = threshold;
        await _databaseService.SaveSettingsAsync(settings);
    }
}
