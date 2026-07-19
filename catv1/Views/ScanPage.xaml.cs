using catv1.ViewModels;
using ZXing.Net.Maui;

namespace catv1.Views;

public partial class ScanPage : ContentPage
{
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

        // Request camera permission at runtime (required on Android 6+)
        var status = await Permissions.RequestAsync<Permissions.Camera>();
        if (status != PermissionStatus.Granted)
        {
            await DisplayAlertAsync("Camera Permission",
                "Camera access is required to scan student IDs. Please grant the permission in Settings.",
                "OK");
            return;
        }

        // Start the camera preview whenever the page is visible.
        // The VM's OnBarcodeDetected ignores results when no session is active,
        // so enabling the preview here is safe and prevents the black-frame issue
        // that occurred when IsDetecting was bound to IsSessionActive via XAML.
        cameraBarcodeReaderView.IsDetecting = true;
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        // Pause the camera when leaving the page to release the hardware resource
        cameraBarcodeReaderView.IsDetecting = false;
    }

    private void CameraBarcodeReaderView_BarcodesDetected(object sender, BarcodeDetectionEventArgs e)
    {
        if (BindingContext is ScanViewModel viewModel)
        {
            viewModel.OnBarcodeDetected(e.Results);
        }
    }
}
