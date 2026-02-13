using BarcodeScanner.Mobile;

namespace FoodInspector.Pages;

public partial class ScanPage : ContentPage
{
    private readonly TaskCompletionSource<string?> _tcs;
    private bool _hasResult;

    public ScanPage(TaskCompletionSource<string?> tcs)
    {
        InitializeComponent();
        _tcs = tcs;
    }

    private void CameraView_OnDetected(object? sender, OnDetectedEventArg e)
    {
        if (_hasResult)
            return;

        var results = e.BarcodeResults;
        if (results == null || results.Count == 0)
            return;

        var barcode = results[0].DisplayValue;
        if (string.IsNullOrWhiteSpace(barcode))
            return;

        _hasResult = true;

        MainThread.BeginInvokeOnMainThread(async () =>
        {
            try
            {
                CameraView.IsScanning = false;
                await Navigation.PopModalAsync(animated: false);
            }
            catch
            {
                // Page may already be removed
            }
            finally
            {
                _tcs.TrySetResult(barcode);
            }
        });
    }

    private async void CancelButton_Clicked(object? sender, EventArgs e)
    {
        if (_hasResult)
            return;

        _hasResult = true;

        try
        {
            CameraView.IsScanning = false;
            await Navigation.PopModalAsync(animated: false);
        }
        catch
        {
            // Page may already be removed
        }
        finally
        {
            _tcs.TrySetResult(null);
        }
    }

    private void TorchButton_Clicked(object? sender, EventArgs e)
    {
        CameraView.TorchOn = !CameraView.TorchOn;
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        // Only touch the camera if we haven't already cleaned up
        if (!_hasResult)
        {
            _hasResult = true;
            try { CameraView.IsScanning = false; } catch { }
            _tcs.TrySetResult(null);
        }
    }
}
