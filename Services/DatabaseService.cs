using FoodInspector.Data;
using FoodInspector.Models;
using Microsoft.EntityFrameworkCore;

namespace FoodInspector.Services;

public interface IDatabaseService
{
    Task InitializeDatabaseAsync();
    Task<List<FoodScanHistory>> GetScanHistoryAsync();
    Task<FoodScanHistory> SaveScanHistoryAsync(FoodScanHistory scan);
    Task<List<IngredientTrigger>> GetAllTriggersAsync();
    Task<List<IngredientSynonym>> GetAllSynonymsAsync();
    Task<List<CrossReactivity>> GetAllCrossReactivitiesAsync();
    Task<AppSettings> GetSettingsAsync();
    Task<AppSettings> SaveSettingsAsync(AppSettings settings);
    Task DeleteScanHistoryAsync(int id);
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

    public async Task<List<FoodScanHistory>> GetScanHistoryAsync()
    {
        return await _context.ScanHistory
            .OrderByDescending(s => s.ScanDate)
            .ToListAsync();
    }

    public async Task<FoodScanHistory> SaveScanHistoryAsync(FoodScanHistory scan)
    {
        if (scan.Id == 0)
        {
            _context.ScanHistory.Add(scan);
        }
        else
        {
            _context.ScanHistory.Update(scan);
        }
        await _context.SaveChangesAsync();
        return scan;
    }

    public async Task<List<IngredientTrigger>> GetAllTriggersAsync()
    {
        return await _context.IngredientTriggers.ToListAsync();
    }

    public async Task<List<IngredientSynonym>> GetAllSynonymsAsync()
    {
        return await _context.IngredientSynonyms
            .Include(s => s.IngredientTrigger)
            .ToListAsync();
    }

    public async Task<List<CrossReactivity>> GetAllCrossReactivitiesAsync()
    {
        return await _context.CrossReactivities
            .Include(c => c.PrimaryTrigger)
            .Include(c => c.RelatedTrigger)
            .ToListAsync();
    }

    public async Task<AppSettings> GetSettingsAsync()
    {
        var settings = await _context.AppSettings.FirstOrDefaultAsync();
        if (settings == null)
        {
            settings = new AppSettings { IsFlareMode = false, FlareModeThreshold = 5 };
            _context.AppSettings.Add(settings);
            await _context.SaveChangesAsync();
        }
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
        var scan = await _context.ScanHistory.FindAsync(id);
        if (scan != null)
        {
            _context.ScanHistory.Remove(scan);
            await _context.SaveChangesAsync();
        }
    }
}
