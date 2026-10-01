using catv1.ViewModels;
using ZXing.Net.Maui;

namespace catv1.Views;

public partial class ScanPage : ContentPage
{
    private CancellationTokenSource? _scanLineCts;
    private bool _permissionGranted;

    public ScanPage(ScanViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        cameraBarcodeReaderView.Options = new BarcodeReaderOptions
        {
            Formats = BarcodeFormats.All,
            AutoRotate = true,
            Multiple = false
        };
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // DES-006: Clear stale singleton state when no session is running.
        // Safe to call unconditionally — it's a no-op while a session is active.
        if (BindingContext is ScanViewModel vm)
            vm.ResetIfIdle();

        // Cancel any previous animation loop
        _scanLineCts?.Cancel();
        _scanLineCts = new CancellationTokenSource();

        // Delay permission request + camera start so the Android activity window
        // is fully focused and the permission dialog will actually be shown.
        // Without this delay, RequestAsync silently returns Denied on most devices.
        await Task.Delay(350);

        // Guard: page might have disappeared during the delay (e.g. rapid back press)
        if (!IsVisible) return;

        if (!_permissionGranted)
        {
            var status = await Permissions.RequestAsync<Permissions.Camera>();
            if (status != PermissionStatus.Granted)
            {
                await DisplayAlertAsync(
                    "Camera Permission",
                    "Camera access is required to scan student IDs. Please enable it in your device Settings.",
                    "OK");
                return;
            }
            _permissionGranted = true;
        }

        // Brief additional pause so ZXing's native camera surface is ready to accept
        // IsDetecting = true. Without this the preview stays black on many Android devices.
        await Task.Delay(150);
        if (!IsVisible) return;

        cameraBarcodeReaderView.IsDetecting = true;

        // Start scan-line animation (tied to the current appearance cycle)
        _ = RunScanLineAnimationAsync(_scanLineCts.Token);
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        // Stop animation loop cleanly
        _scanLineCts?.Cancel();
        scanLine.CancelAnimations();

        // Release the camera hardware
        cameraBarcodeReaderView.IsDetecting = false;
    }

    private async Task RunScanLineAnimationAsync(CancellationToken ct)
    {
        try
        {
            while (!ct.IsCancellationRequested)
            {
                await scanLine.TranslateToAsync(0, 80, 1500, Easing.SinInOut);
                if (ct.IsCancellationRequested) break;
                await scanLine.TranslateToAsync(0, -80, 1500, Easing.SinInOut);
            }
        }
        catch (TaskCanceledException) { }
    }

    private void CameraBarcodeReaderView_BarcodesDetected(object sender, BarcodeDetectionEventArgs e)
    {
        if (BindingContext is ScanViewModel viewModel)
        {
            viewModel.OnBarcodeDetected(e.Results);
        }
    }
}
