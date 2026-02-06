using FoodInspector.Models;
using System.Text;
using System.Text.Json;
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;

namespace FoodInspector.Services;

public interface IExportService
{
    Task<string> ExportToCsvAsync(List<ScanRecord> scans);
    Task<string> ExportToJsonAsync(List<ScanRecord> scans);
}

public class ExportService : IExportService
{
    public async Task<string> ExportToCsvAsync(List<ScanRecord> scans)
    {
        var filePath = Path.Combine(FileSystem.CacheDirectory, $"food_history_{DateTime.Now:yyyyMMdd_HHmmss}.csv");
        
        using var writer = new StreamWriter(filePath);
        using var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture));
        
        await csv.WriteRecordsAsync(scans);
        
        return filePath;
    }

    public async Task<string> ExportToJsonAsync(List<ScanRecord> scans)
    {
        var filePath = Path.Combine(FileSystem.CacheDirectory, $"food_history_{DateTime.Now:yyyyMMdd_HHmmss}.json");
        
        var json = JsonSerializer.Serialize(scans, new JsonSerializerOptions 
        { 
            WriteIndented = true 
        });
        
        await File.WriteAllTextAsync(filePath, json);
        
        return filePath;
    }
}

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
