using System.Text.Json;
using FoodInspector.Models;

namespace FoodInspector.Services;

public interface IBarcodeScannerService
{
    Task<string?> ScanBarcodeAsync();
    bool IsNativeScanSupported { get; }
}

public class BarcodeScannerService : IBarcodeScannerService
{
    public bool IsNativeScanSupported =>
        DeviceInfo.Platform == DevicePlatform.Android ||
        DeviceInfo.Platform == DevicePlatform.iOS;

    public async Task<string?> ScanBarcodeAsync()
    {
        if (!IsNativeScanSupported)
            return null;

        try
        {
            var status = await Permissions.RequestAsync<Permissions.Camera>();
            if (status != PermissionStatus.Granted)
            {
                System.Diagnostics.Debug.WriteLine("Camera permission denied");
                return null;
            }

            var tcs = new TaskCompletionSource<string?>();

            MainThread.BeginInvokeOnMainThread(async () =>
            {
                try
                {
                    var scanPage = new Pages.ScanPage(tcs);

                    var currentPage = Application.Current?.Windows.FirstOrDefault()?.Page;
                    if (currentPage == null)
                    {
                        tcs.TrySetResult(null);
                        return;
                    }

                    // NavigationPage supports PushModalAsync reliably on Android
                    if (currentPage is NavigationPage navPage)
                    {
                        await navPage.Navigation.PushModalAsync(scanPage, animated: false);
                    }
                    else if (currentPage.Navigation != null)
                    {
                        await currentPage.Navigation.PushModalAsync(scanPage, animated: false);
                    }
                    else
                    {
                        tcs.TrySetResult(null);
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Push modal error: {ex.Message}");
                    tcs.TrySetResult(null);
                }
            });

            return await tcs.Task;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Barcode scan error: {ex.Message}");
            return null;
        }
    }
}

public interface IOcrService
{
    Task<string> ExtractTextFromImageAsync(string imagePath);
}

public class OcrService : IOcrService
{
    public async Task<string> ExtractTextFromImageAsync(string imagePath)
    {
        // Placeholder for OCR implementation
        await Task.CompletedTask;
        return string.Empty;
    }
}

public interface IOpenFoodFactsService
{
    Task<OpenFoodFactsProduct?> GetProductByBarcodeAsync(string barcode);
}

public class OpenFoodFactsProduct
{
    public string? ProductName { get; set; }
    public string? Ingredients { get; set; }
    public string? Brands { get; set; }
    public string? ImageUrl { get; set; }
}

public class OpenFoodFactsService : IOpenFoodFactsService
{
    private readonly HttpClient _httpClient;

    public OpenFoodFactsService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<OpenFoodFactsProduct?> GetProductByBarcodeAsync(string barcode)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/v2/product/{barcode}.json?fields=product_name,ingredients_text,brands,image_url");
            if (!response.IsSuccessStatusCode)
                return null;

            var json = await response.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            if (root.TryGetProperty("status", out var statusEl) && statusEl.GetInt32() != 1)
                return null;

            if (!root.TryGetProperty("product", out var product))
                return null;

            return new OpenFoodFactsProduct
            {
                ProductName = GetString(product, "product_name"),
                Ingredients = GetString(product, "ingredients_text"),
                Brands = GetString(product, "brands"),
                ImageUrl = GetString(product, "image_url")
            };
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Open Food Facts API error: {ex.Message}");
        }
        return null;
    }

    private static string? GetString(JsonElement el, string property)
    {
        if (el.TryGetProperty(property, out var value) && value.ValueKind == JsonValueKind.String)
        {
            var str = value.GetString();
            return string.IsNullOrWhiteSpace(str) ? null : str;
        }
        return null;
    }
}
