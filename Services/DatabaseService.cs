using FoodInspector.Data;
using FoodInspector.Models;
using Microsoft.EntityFrameworkCore;

namespace FoodInspector.Services;
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
public class DatabaseService : IDatabaseService
{
    private readonly FoodInspectorDbContext _context;
    private readonly ISecureStorageService _secureStorage;

    public DatabaseService(FoodInspectorDbContext context, ISecureStorageService secureStorage)
    {
        _context = context;
        _secureStorage = secureStorage;
    }

    public async Task InitializeDatabaseAsync()
    {
        try
        {
            // Get encryption key
            var key = await _secureStorage.GetEncryptionKeyAsync();
            
            // Ensure database is created with schema
            // Use EnsureCreated instead of Migrate for MAUI apps
            await _context.Database.EnsureCreatedAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Database initialization error: {ex.Message}");
            throw;
        }
    }

    public async Task<List<ScanRecord>> GetScanHistoryAsync()
    {
        return await _context.ScanRecords
            .Include(x => x.Matches)
            .OrderByDescending(s => s.CreatedUtc)
            .ToListAsync();
    }

    public async Task<ScanRecord> SaveScanHistoryAsync(ScanRecord scan)
    {
        if (scan.Id == 0)
        {
            _context.ScanRecords.Add(scan);
        }
        else
        {
            _context.ScanRecords.Update(scan);
        }

        await _context.SaveChangesAsync();
        return scan;
    }

    public Task<List<Models.Trigger>> GetAllTriggersAsync() =>
        _context.Triggers.Where(x => x.Enabled).ToListAsync();

    public async Task<List<TriggerSynonym>> GetAllSynonymsAsync()
    {
        return await _context.TriggerSynonyms
            .Include(s => s.Trigger)
            .ToListAsync();
    }

    public async Task<List<CrossReactivityRule>> GetCrossReactivityRulesAsync()
    {
        return await _context.CrossReactivityRules
            .Include(x => x.SourceTrigger)
            .Include(x => x.TargetTrigger)
            .Include(x => x.EvidenceSource)
            .ToListAsync();
    }

    public async Task<AppSettings> GetSettingsAsync()
    {
        var settings = await _context.AppSettings.FirstOrDefaultAsync();
        if (settings != null)
        {
            return settings;
        }

        settings = new AppSettings { IsFlareMode = false, FlareModeThreshold = 5 };
        _context.AppSettings.Add(settings);
        await _context.SaveChangesAsync();
        return settings;
    }

    public async Task<AppSettings> SaveSettingsAsync(AppSettings settings)
    {
        _context.AppSettings.Update(settings);
        await _context.SaveChangesAsync();
        return settings;
    }

    public async Task DeleteScanHistoryAsync(int id)
    {
        var scan = await _context.ScanRecords.FindAsync(id);
        if (scan == null)
        {
            return;
        }

        _context.ScanRecords.Remove(scan);
        await _context.SaveChangesAsync();
    }

    public async Task ToggleCrossReactivityRuleAsync(int id, bool enabled)
    {
        var rule = await _context.CrossReactivityRules.FindAsync(id);
        if (rule == null)
        {
            return;
        }

        rule.Enabled = enabled;
        await _context.SaveChangesAsync();
    }
}
