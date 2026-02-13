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
