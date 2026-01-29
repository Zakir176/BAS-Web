using ZXing.Net.Maui;

namespace catv1.Views;

public partial class ScanPage : ContentPage
{
    public ScanPage()
    {
        InitializeComponent();
        cameraBarcodeReaderView.Options = new BarcodeReaderOptions
        {
            Formats = BarcodeFormats.All,
            AutoRotate = true,
            Multiple = false
        };
    }

    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}
