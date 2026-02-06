using FoodInspector.Data;
using FoodInspector.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui;
using ZXing.Net.Maui.Controls;

namespace FoodInspector;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .UseBarcodeReader()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });

        builder.Services.AddMauiBlazorWebView();

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif

        // Register services
        builder.Services.AddSingleton<ISecureStorageService, SecureStorageService>();
        builder.Services.AddSingleton<IDatabaseService, DatabaseService>();
        builder.Services.AddSingleton<IBarcodeScannerService, BarcodeScannerService>();
        builder.Services.AddSingleton<IOcrService, OcrService>();
        builder.Services.AddSingleton<IIngredientAnalysisService, IngredientAnalysisService>();
        builder.Services.AddSingleton<IExportService, ExportService>();
        builder.Services.AddSingleton<ISettingsService, SettingsService>();
        
        // Register HttpClient and OpenFoodFactsService
        builder.Services.AddSingleton(sp => new HttpClient { BaseAddress = new Uri("https://world.openfoodfacts.org/") });
        builder.Services.AddSingleton<IOpenFoodFactsService, OpenFoodFactsService>();

        // Configure database
        var dbPath = Path.Combine(FileSystem.AppDataDirectory, "foodinspector.db");
        builder.Services.AddDbContext<FoodInspectorDbContext>(options =>
        {
            options.UseSqlite($"Data Source={dbPath}");
        });

        return builder.Build();
    }
}
